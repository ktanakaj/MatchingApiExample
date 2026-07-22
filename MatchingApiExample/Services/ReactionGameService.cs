// ================================================================================================
// <summary>
//      早押しゲームサービスクラスソース</summary>
//
// <copyright file="ReactionGameService.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Honememo.MatchingApiExample.Entities;
using Honememo.MatchingApiExample.Exceptions;
using Honememo.MatchingApiExample.Protos;
using Honememo.MatchingApiExample.Repositories;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Player = Honememo.MatchingApiExample.Entities.Player;
using ReactionGame = Honememo.MatchingApiExample.Entities.ReactionGame;

namespace Honememo.MatchingApiExample.Services;

/// <summary>
/// 早押しゲームサービス。
/// </summary>
/// <remarks>ランダムな開始の合図からの、ボタンを押す速さを競うゲームのロジックを扱う。</remarks>
[Authorize]
public class ReactionGameService : Protos.ReactionGame.ReactionGameBase
{
    /// <summary>
    /// サービススコープファクトリ。
    /// </summary>
    private readonly IServiceScopeFactory serviceScopeFactory;

    /// <summary>
    /// ロガー。
    /// </summary>
    private readonly ILogger<ReactionGameService> logger;

    /// <summary>
    /// Mapsterインスタンス。
    /// </summary>
    private readonly IMapper mapper;

    /// <summary>
    /// ゲームリポジトリ。
    /// </summary>
    private readonly GameRepository gameRepository;

    /// <summary>
    /// ルームリポジトリ。
    /// </summary>
    private readonly RoomRepository roomRepository;

    /// <summary>
    /// プレイヤーリポジトリ。
    /// </summary>
    private readonly PlayerRepository playerRepository;

    /// <summary>
    /// 渡されたインスタンスを使用してサービスを生成する。
    /// </summary>
    /// <param name="serviceScopeFactory">サービススコープファクトリ。</param>
    /// <param name="logger">ロガー。</param>
    /// <param name="mapper">Mapsterインスタンス。</param>
    /// <param name="gameRepository">ゲームリポジトリ。</param>
    /// <param name="roomRepository">ルームリポジトリ。</param>
    /// <param name="playerRepository">プレイヤーリポジトリ。</param>
    public ReactionGameService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ReactionGameService> logger,
        IMapper mapper,
        GameRepository gameRepository,
        RoomRepository roomRepository,
        PlayerRepository playerRepository)
    {
        this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        this.roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        this.playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

    /// <summary>
    /// ゲームを準備完了にする。
    /// </summary>
    /// <param name="request">空パラメータ。</param>
    /// <param name="responseStream">レスポンス用のストリーム。</param>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>空レスポンス。</returns>
    /// <exception cref="FailedPreconditionException">ルームが定員でない場合。</exception>
    public override async Task Ready(Empty request, IServerStreamWriter<GameEventReply> responseStream, ServerCallContext context)
    {
        var room = this.GetRoom(context);
        if (!room.IsFull())
        {
            throw new FailedPreconditionException($"Room No={room.No} is not full");
        }

        ReactionGame game;
        lock (room)
        {
            if (room.GameId == null)
            {
                // 部屋がまだゲームを開始していない場合は、ゲームを開始する
                game = new ReactionGame(room.PlayerIds);
                this.gameRepository.AddGame(game);
                room.GameId = game.Id;

                // ゲーム終了時の処理を登録する
                game.GameEvent += this.OnGameEnded;
            }
            else
            {
                // それ以外は開催中のゲームを取得する
                game = this.GetGame(room.GameId);
            }
        }

        // ゲームイベントの監視を開始、プレイヤーの準備完了イベントを起こす
        var t = this.WatchGame(game, responseStream, context);
        game.Ready(context.GetPlayerId());
        await t;
    }

    /// <summary>
    /// ボタンを押す。
    /// </summary>
    /// <param name="request">押下日時。</param>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>空レスポンス。</returns>
    public override async Task<Empty> Submit(SubmitRequest request, ServerCallContext context)
    {
        var game = this.GetGame(context);
        var pressDate = request.Date?.ToDateTimeOffset() ?? DateTimeOffset.UtcNow;
        game.Submit(context.GetPlayerId(), pressDate);
        return new Empty();
    }

    /// <summary>
    /// ゲームイベントを監視する。
    /// </summary>
    /// <param name="game">監視するゲーム。</param>
    /// <param name="responseStream">レスポンス用のストリーム。</param>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>処理状態。</returns>
    private async Task WatchGame(ReactionGame game, IServerStreamWriter<GameEventReply> responseStream, ServerCallContext context)
    {
        var room = this.GetRoom(context);

        // ゲームイベントの監視を開始する
        EventHandler<ReactionGame.GameEventArgs> f = async (sender, e) =>
        {
            if (!context.CancellationToken.IsCancellationRequested)
            {
                await responseStream.WriteAsync(this.mapper.Map<GameEventReply>(e));
            }
        };
        game.GameEvent += f;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(500);
        }

        game.GameEvent -= f;
    }

    /// <summary>
    /// 認証中のプレイヤーが参加中のゲームを取得する。
    /// </summary>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>参加中のゲーム。</returns>
    /// <exception cref="FailedPreconditionException">早押しゲームをプレイしていない場合。</exception>
    private ReactionGame GetGame(ServerCallContext context)
    {
        var room = this.GetRoom(context);
        if (room.GameId == null)
        {
            throw new FailedPreconditionException($"Room No={room.No} is not started any game");
        }

        return this.GetGame(room.GameId);
    }

    /// <summary>
    /// ゲームIDからゲームを取得する。
    /// </summary>
    /// <param name="gameId">ゲームID。</param>
    /// <returns>参加中のゲーム。</returns>
    /// <exception cref="FailedPreconditionException">早押しゲームのIDでない場合。</exception>
    private ReactionGame GetGame(string gameId)
    {
        var game = this.gameRepository.GetGame(gameId);
        if (game is ReactionGame s)
        {
            return s;
        }

        throw new FailedPreconditionException($"Game ID={gameId} is not reaction game");
    }

    /// <summary>
    /// 認証中のプレイヤーが入室中の部屋の情報を取得する。
    /// </summary>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>入室中の部屋情報。</returns>
    /// <exception cref="FailedPreconditionException">入室していない場合。</exception>
    private Room GetRoom(ServerCallContext context)
    {
        var playerId = context.GetPlayerId();
        if (!this.roomRepository.TryGetRoomByPlayerId(playerId, out Room room))
        {
            throw new FailedPreconditionException($"Player ID={playerId} is not joined any room");
        }

        return room;
    }

    /// <summary>
    /// ゲーム終了時の処理。
    /// </summary>
    /// <param name="sender">ゲームオブジェクト。</param>
    /// <param name="e">ゲームイベント引数。</param>
    private async void OnGameEnded(object? sender, ReactionGame.GameEventArgs e)
    {
        // 他のイベントも来るので、ゲーム終了イベント以外は無視する
        if (e.Type != ReactionGameEventType.End)
        {
            return;
        }

        // イベント到達時はインスタンスは破棄されている可能性があるので、スコープを作ってサービスを再取得して処理
        try
        {
            using var scope = this.serviceScopeFactory.CreateScope();
            await scope.ServiceProvider.GetRequiredService<ReactionGameService>().OnGameEndedImpl((ReactionGame)sender!);
        }
        catch (Exception ex)
        {
            this.logger.LogWarning(ex, "Failed to handle game ended event.");
        }
    }

    /// <summary>
    /// ゲーム終了時の処理の実装。
    /// </summary>
    /// <param name="game">ゲームオブジェクト。</param>
    /// <returns>処理状態。</returns>
    private async Task OnGameEndedImpl(ReactionGame game)
    {
        // ゲームに参加していたメンバーのレーティングを更新
        await this.UpdateRatings(game);

        // ゲームを削除して、ルームを未プレイ中の状態に戻す
        if (this.roomRepository.TryGetRoomByPlayerId(game.PlayerIds[0], out var room) && room.GameId == game.Id)
        {
            room.GameId = null;
        }

        this.gameRepository.RemoveGame(game.Id);
    }

    /// <summary>
    /// ゲーム終了時のレーティング更新。
    /// </summary>
    /// <param name="game">終了したゲーム。</param>
    private async Task UpdateRatings(ReactionGame game)
    {
        // 勝者のレーティングを上げて、敗者のレーティングを下げる
        // TODO: 計算式は現状てきとう。イロレーティングとかグリコレーティングとかいろいろアルゴリズムがあるので、本当はちゃんとやるべき。
        //       それだとたぶん引き分けもレーティングが変わる。
        if (game.WinnerId == null)
        {
            return;
        }

        var winner = game.WinnerId.Value;
        var players = new List<Player>();
        foreach (var playerId in game.PlayerIds)
        {
            var player = await this.playerRepository.FindOrFail(playerId);
            ushort newRating = playerId == winner
                ? (ushort)Math.Min(ushort.MaxValue, player.Rating + 12)
                : (ushort)Math.Max(0, player.Rating - 8);
            if (player.Rating != newRating)
            {
                player.Rating = newRating;
                players.Add(player);
            }
        }

        await this.playerRepository.UpdateMany(players);
    }
}
