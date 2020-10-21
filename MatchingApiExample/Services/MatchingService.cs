// ================================================================================================
// <summary>
//      マッチングサービスクラスソース</summary>
//
// <copyright file="MatchingService.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Service
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;
    using Honememo.MatchingApiExample.Entities;
    using Honememo.MatchingApiExample.Exceptions;
    using Honememo.MatchingApiExample.Protos;
    using Honememo.MatchingApiExample.Repositories;
    using Player = Entities.Player;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    /// <summary>
    /// マッチングサービス。
    /// </summary>
    [Authorize]
    public class MatchingService : Matching.MatchingBase
    {
        #region 定数

        /// <summary>
        /// 特に指定がない場合のルームの最大人数。
        /// </summary>
        private static readonly ushort DefaultMaxPlayers = 2;

        /// <summary>
        /// マッチング処理の処理条件配列。
        /// </summary>
        /// <remarks>5秒ごとに範囲を広げてマッチング実行。10秒経っても見つからなければ全ルームを対象にする。</remarks>
        private static readonly MatchingCondition[] MatchingConditions = new[]
            {
                new MatchingCondition { Range = 100, Next = 5000 },
                new MatchingCondition { Range = 100, Next = 0 },
                new MatchingCondition { Range = 200, Next = 5000 },
                new MatchingCondition { Range = 100, Next = 0 },
                new MatchingCondition { Range = 200, Next = 0 },
                new MatchingCondition { Range = ushort.MaxValue, Next = 0 },
            };

        #endregion

        #region メンバー変数

        /// <summary>
        /// 乱数。
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        /// ロガー。
        /// </summary>
        private readonly ILogger<MatchingService> logger;

        /// <summary>
        /// AutoMapperインスタンス。
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// ルームリポジトリ。
        /// </summary>
        private readonly RoomRepository roomRepository;

        /// <summary>
        /// プレイヤーリポジトリ。
        /// </summary>
        private readonly PlayerRepository playerRepository;

        /// <summary>
        /// ゲームリポジトリ。
        /// </summary>
        private readonly GameRepository gameRepository;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 渡されたインスタンスを使用してサービスを生成する。
        /// </summary>
        /// <param name="logger">ロガー。</param>
        /// <param name="mapper">AutoMapperインスタンス。</param>
        /// <param name="roomRepository">ルームリポジトリ。</param>
        /// <param name="playerRepository">プレイヤーリポジトリ。</param>
        /// <param name="gameRepository">ゲームリポジトリ。</param>
        public MatchingService(
            ILogger<MatchingService> logger,
            IMapper mapper,
            RoomRepository roomRepository,
            PlayerRepository playerRepository,
            GameRepository gameRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.roomRepository = roomRepository;
            this.playerRepository = playerRepository;
            this.gameRepository = gameRepository;
        }

        #endregion

        #region gRPC更新系メソッド

        /// <summary>
        /// 部屋を作成する。
        /// </summary>
        /// <param name="request">作成条件。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>作成した部屋情報。</returns>
        public override async Task<CreateRoomReply> CreateRoom(CreateRoomRequest request, ServerCallContext context)
        {
            var player = await this.playerRepository.FindOrFail(context.GetPlayerId());
            Validator.ValidateObject(request, new ValidationContext(request));
            if (this.roomRepository.TryGetRoomByPlayerId(player.Id, out Room room))
            {
                throw new AlreadyExistsException($"Player ID={player.Id} is already exists in the Room No={room.No}");
            }

            room = this.roomRepository.CreateRoom((ushort)request.MaxPlayers);
            room.AddPlayer(player);
            return this.mapper.Map<CreateRoomReply>(room);
        }

        /// <summary>
        /// 部屋に入る。
        /// </summary>
        /// <param name="request">部屋番号。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>空レスポンス。</returns>
        public override async Task<Empty> JoinRoom(JoinRoomRequest request, ServerCallContext context)
        {
            var player = await this.playerRepository.FindOrFail(context.GetPlayerId());
            if (!this.roomRepository.TryGetRoom(request.No, out Room room))
            {
                throw new NotFoundException($"Room No={request.No} is not found");
            }

            if (this.roomRepository.TryGetRoomByPlayerId(player.Id, out Room nowRoom))
            {
                throw new AlreadyExistsException($"Player ID={player.Id} is already exists in the Room No={nowRoom.No}");
            }

            room.AddPlayer(player);
            return new Empty();
        }

        /// <summary>
        /// 部屋を出る。
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>空レスポンス。</returns>
        public override async Task<Empty> LeaveRoom(Empty request, ServerCallContext context)
        {
            // プレイヤーが入室済かチェック
            var player = await this.playerRepository.FindOrFail(context.GetPlayerId());
            if (!this.roomRepository.TryGetRoomByPlayerId(player.Id, out Room room))
            {
                return new Empty();
            }

            // ゲーム開始中の場合は、ゲームも強制終了する
            if (room.GameId != null)
            {
                this.gameRepository.RemoveGame(room.GameId);
            }

            // 部屋から出る。部屋が0人になる場合は解散
            room.RemovePlayer(player);
            if (room.PlayerIds.Count == 0)
            {
                this.roomRepository.RemoveRoom(room.No);
            }

            return new Empty();
        }

        /// <summary>
        /// 部屋をマッチングする。
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>入室or作成した部屋情報。</returns>
        public override async Task<MatchRoomReply> MatchRoom(Empty request, ServerCallContext context)
        {
            // 時間を置きながら複数回マッチングを試行する
            var player = await this.playerRepository.FindOrFail(context.GetPlayerId());
            foreach (var condition in MatchingConditions)
            {
                var room = await this.MatchRoomInRange(player, condition.Range);
                if (room != null)
                {
                    return this.mapper.Map<MatchRoomReply>(room);
                }

                await Task.Delay(condition.Next);
            }

            // 無かったら新規作成
            var newRoom = this.roomRepository.CreateRoom(DefaultMaxPlayers);
            newRoom.AddPlayer(player);
            return this.mapper.Map<MatchRoomReply>(newRoom);
        }

        #endregion

        #region gRPC参照系メソッド

        /// <summary>
        /// 部屋の一覧を取得する。
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>部屋の一覧。</returns>
        public override async Task<FindRoomsReply> FindRooms(Empty request, ServerCallContext context)
        {
            var rooms = this.roomRepository.GetRooms();
            var reply = new FindRoomsReply { Count = (uint)rooms.Count };
            reply.Rooms.AddRange(this.mapper.Map<ICollection<RoomSummary>>(rooms));
            return reply;
        }

        /// <summary>
        /// 部屋の一覧を監視する。
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="responseStream">レスポンス用のストリーム。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>処理状態。</returns>
        public override async Task WatchRooms(Empty request, IServerStreamWriter<FindRoomsReply> responseStream, ServerCallContext context)
        {
            // 初回は普通に実行して、以後はイベントが起きたタイミングで実行
            await responseStream.WriteAsync(await this.FindRooms(new Empty(), context));

            EventHandler<Room> f = async (sender, room) =>
            {
                if (!context.CancellationToken.IsCancellationRequested)
                {
                    await responseStream.WriteAsync(await this.FindRooms(new Empty(), context));
                }
            };
            this.roomRepository.OnUpdated += f;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(500);
            }

            this.roomRepository.OnUpdated -= f;
        }

        /// <summary>
        /// 入室中の部屋の情報を取得する。
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>入室中の部屋情報。</returns>
        public override async Task<GetRoomReply> GetRoom(Empty request, ServerCallContext context)
        {
            var playerId = context.GetPlayerId();
            if (!this.roomRepository.TryGetRoomByPlayerId(playerId, out Room room))
            {
                throw new FailedPreconditionException($"Player ID={playerId} is not joined any room");
            }

            return await this.MakeRoomStatus(room);
        }

        /// <summary>
        /// 入室中の部屋の情報を監視する。
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="responseStream">レスポンス用のストリーム。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>処理状態。</returns>
        public override async Task WatchRoom(Empty request, IServerStreamWriter<GetRoomReply> responseStream, ServerCallContext context)
        {
            var playerId = context.GetPlayerId();
            if (!this.roomRepository.TryGetRoomByPlayerId(playerId, out Room room))
            {
                throw new FailedPreconditionException($"Player ID={playerId} is not joined any room");
            }

            // 初回は普通に実行して、以後はイベントが起きたタイミングで実行
            await responseStream.WriteAsync(await this.MakeRoomStatus(room));

            EventHandler<Room.UpdatedEventArgs> f = async (sender, e) =>
            {
                if (!context.CancellationToken.IsCancellationRequested)
                {
                    await responseStream.WriteAsync(await this.MakeRoomStatus(room));
                }
            };
            room.OnUpdated += f;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(500);
            }

            room.OnUpdated -= f;
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// 部屋を指定されたレーティング範囲でマッチングする。
        /// </summary>
        /// <param name="player">マッチングするプレイヤー。</param>
        /// <param name="range">マッチングするレーティング範囲。※±100の場合100</param>
        /// <returns>マッチングした場合、そのルーム。入室出来なかった場合はnull。</returns>
        /// <exception cref="AlreadyExistsException">既に入室中の場合。</exception>
        private async Task<Room> MatchRoomInRange(Player player, ushort range)
        {
            // 処理中に入室している可能性もあるのでここでチェック
            if (this.roomRepository.TryGetRoomByPlayerId(player.Id, out Room nowRoom))
            {
                throw new AlreadyExistsException($"Player ID={player.Id} is already exists in the Room No={nowRoom.No}");
            }

            // 指定された上下レーティング範囲のルームを取得
            var rooms = this.roomRepository.FindAvailableRooms(
                (ushort)Math.Max(player.Rating - range, 0),
                (ushort)Math.Min(player.Rating + range, ushort.MaxValue));
            if (rooms.Count == 0)
            {
                return null;
            }

            // 古いルームから順番に入室を試みる
            // （Room側で排他制御しているので、入れなかったら例外が飛ぶ）
            foreach (var room in rooms.OrderBy(r => r.CreatedAt))
            {
                try
                {
                    room.AddPlayer(player);
                    return room;
                }
                catch (InvalidOperationException e)
                {
                    this.logger.LogDebug(e, $"Room No={room.No} AddPlayer failed");
                }
            }

            // どこにも入れなかったら入室失敗
            return null;
        }

        /// <summary>
        /// 部屋情報の戻り値を生成する。
        /// </summary>
        /// <param name="room">元となるルーム。</param>
        /// <returns>部屋情報。</returns>
        private async Task<GetRoomReply> MakeRoomStatus(Room room)
        {
            var reply = this.mapper.Map<GetRoomReply>(room);
            reply.Players.Add(this.mapper.Map<ICollection<PlayerInfo>>(await this.playerRepository.Find(room.PlayerIds)));
            return reply;
        }

        #endregion

        #region 内部クラス

        /// <summary>
        /// マッチング処理のループ条件。
        /// </summary>
        private class MatchingCondition
        {
            #region プロパティ

            /// <summary>
            /// マッチングするレーティング範囲。
            /// </summary>
            public ushort Range { get; set; }

            /// <summary>
            /// 次のマッチング条件までのウェイト。
            /// </summary>
            public int Next { get; set; }

            #endregion
        }

        #endregion
    }
}
