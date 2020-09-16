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

    /// <summary>
    /// マッチングサービス。
    /// </summary>
    [Authorize]
    public class MatchingService : Matching.MatchingBase
    {
        #region メンバー変数

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

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 渡されたインスタンスを使用してサービスを生成する。
        /// </summary>
        /// <param name="logger">ロガー。</param>
        /// <param name="mapper">AutoMapperインスタンス。</param>
        /// <param name="roomRepository">ルームリポジトリ。</param>
        public MatchingService(ILogger<MatchingService> logger, IMapper mapper, RoomRepository roomRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.roomRepository = roomRepository;
        }

        #endregion

        #region gRPCメソッド

        /// <summary>
        /// 部屋を作成する。
        /// </summary>
        /// <param name="request">作成条件。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>作成した部屋情報。</returns>
        public override async Task<CreateRoomReply> CreateRoom(CreateRoomRequest request, ServerCallContext context)
        {
            // TODO: 有効値チェック
            var playerId = context.GetPlayerId();
            if (this.roomRepository.TryGetRoomByPlayerId(playerId, out Room room))
            {
                throw new AlreadyExistsException($"Player ID={playerId} is already exists in the Room No={room.No}");
            }

            room = this.roomRepository.CreateRoom(request.MaxPlayers);
            room.AddPlayer(playerId);
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
            if (!this.roomRepository.TryGetRoom(request.No, out Room room))
            {
                throw new NotFoundException($"Room No={request.No} is not found");
            }

            room.AddPlayer(context.GetPlayerId());
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
            var playerId = context.GetPlayerId();
            if (!this.roomRepository.TryGetRoomByPlayerId(playerId, out Room room))
            {
                return new Empty();
            }

            room.RemovePlayer(playerId);
            if (room.PlayerIds.Count == 0)
            {
                this.roomRepository.RemoveRoom(room.No);
            }

            return new Empty();
        }

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
        /// 部屋の一覧の更新を通知させる。
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="responseStream">レスポンス用のストリーム。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>処理状態。</returns>
        public override async Task FireRoomsUpdated(Empty request, IServerStreamWriter<FindRoomsReply> responseStream, ServerCallContext context)
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
        /// 部屋をマッチングする。
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>入室or作成した部屋情報。</returns>
        public override async Task<MatchRoomReply> MatchRoom(Empty request, ServerCallContext context)
        {
            // TODO: 仮実装。現在は空いてる先頭のルームに入れるだけ
            var playerId = context.GetPlayerId();
            var rooms = this.roomRepository.GetRooms();
            foreach (var room in rooms)
            {
                if (!room.IsFull())
                {
                    room.AddPlayer(playerId);
                    return this.mapper.Map<MatchRoomReply>(room);
                }
            }

            // 無かったら新規作成
            var newRoom = this.roomRepository.CreateRoom(2);
            newRoom.AddPlayer(playerId);
            return this.mapper.Map<MatchRoomReply>(newRoom);
        }

        #endregion
    }
}
