// ================================================================================================
// <summary>
//      しりとりゲームサービスクラスソース</summary>
//
// <copyright file="ShiritoriService.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Honememo.MatchingApiExample.Entities;
using Honememo.MatchingApiExample.Exceptions;
using Honememo.MatchingApiExample.Protos;
using Honememo.MatchingApiExample.Repositories;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Shiritori = Honememo.MatchingApiExample.Entities.Shiritori;

namespace Honememo.MatchingApiExample.Services;

/// <summary>
/// しりとりゲームサービス。
/// </summary>
[Authorize]
public class ShiritoriService : Protos.Shiritori.ShiritoriBase
{
    /// <summary>
    /// ロガー。
    /// </summary>
    private readonly ILogger<ShiritoriService> logger;

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
    /// 渡されたインスタンスを使用してサービスを生成する。
    /// </summary>
    /// <param name="logger">ロガー。</param>
    /// <param name="mapper">Mapsterインスタンス。</param>
    /// <param name="gameRepository">ゲームリポジトリ。</param>
    /// <param name="roomRepository">ルームリポジトリ。</param>
    public ShiritoriService(ILogger<ShiritoriService> logger, IMapper mapper, GameRepository gameRepository, RoomRepository roomRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        this.roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
    }

    //// TODO: ゲーム終了時にゲームを破棄する
    //// TODO: ゲーム終了時にプレイヤーのレーティングを更新する

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

        Shiritori game;
        lock (room)
        {
            if (room.GameId == null)
            {
                // 部屋がまだゲームを開始していない場合は、ゲームを開始する
                game = new Shiritori(room.PlayerIds);
                this.gameRepository.AddGame(game);
                room.GameId = game.Id;
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
    /// 自分の手番に単語を回答する。
    /// </summary>
    /// <param name="request">回答。</param>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>回答結果。</returns>
    /// <exception cref="FailedPreconditionException">プレイヤーの手番でない場合。</exception>
    /// <exception cref="InvalidArgumentException">回答が空や対象外の文字列の場合。</exception>
    public override async Task<AnswerReply> Answer(AnswerRequest request, ServerCallContext context)
    {
        return new AnswerReply { Result = this.GetGame(context).Answer(context.GetPlayerId(), request.Word) };
    }

    /// <summary>
    /// 直前の他人の回答に異議を送信する。
    /// </summary>
    /// <param name="request">空パラメータ。</param>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>空レスポンス。</returns>
    public override async Task<Empty> Claim(Empty request, ServerCallContext context)
    {
        this.GetGame(context).Claim(context.GetPlayerId());
        return new Empty();
    }

    /// <summary>
    /// ゲームイベントを監視する。
    /// </summary>
    /// <param name="game">監視するゲーム。</param>
    /// <param name="responseStream">レスポンス用のストリーム。</param>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>処理状態。</returns>
    private async Task WatchGame(Shiritori game, IServerStreamWriter<GameEventReply> responseStream, ServerCallContext context)
    {
        // ゲームイベントの監視を開始する
        EventHandler<Shiritori.GameEventArgs> f = async (sender, e) =>
        {
            if (!context.CancellationToken.IsCancellationRequested)
            {
                await responseStream.WriteAsync(this.mapper.Map<GameEventReply>(e));
            }
        };
        game.OnGameEvent += f;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(500);
        }

        game.OnGameEvent -= f;
    }

    /// <summary>
    /// 認証中のプレイヤーが参加中のゲームを取得する。
    /// </summary>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>参加中のゲーム。</returns>
    /// <exception cref="FailedPreconditionException">しりとりゲームをプレイしていない場合。</exception>
    private Shiritori GetGame(ServerCallContext context)
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
    /// <exception cref="FailedPreconditionException">しりとりゲームのIDでない場合。</exception>
    private Shiritori GetGame(string gameId)
    {
        var game = this.gameRepository.GetGame(gameId);
        if (game is Shiritori s)
        {
            return s;
        }

        throw new FailedPreconditionException($"Game ID={gameId} is not shiritori game");
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
}
