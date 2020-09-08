// ================================================================================================
// <summary>
//      ゲームサービスクラスソース</summary>
//
// <copyright file="GameService.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Service
{
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
    /// ゲームサービスサービス。
    /// </summary>
    [Authorize]
    public class GameService : Game.GameBase
    {
        #region メンバー変数

        /// <summary>
        /// ロガー。
        /// </summary>
        private readonly ILogger<GameService> logger;

        /// <summary>
        /// AutoMapperインスタンス。
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// プレイヤーリポジトリ。
        /// </summary>
        private readonly PlayerRepository playerRepository;

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
        /// <param name="playerRepository">プレイヤーリポジトリ。</param>
        /// <param name="roomRepository">ルームリポジトリ。</param>
        public GameService(ILogger<GameService> logger, IMapper mapper, PlayerRepository playerRepository, RoomRepository roomRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.playerRepository = playerRepository;
            this.roomRepository = roomRepository;
        }

        #endregion

        #region gRPCメソッド

        /// <summary>
        /// コマンドを送信する。
        /// </summary>
        /// <param name="request">コマンド情報。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>空レスポンス。</returns>
        public override async Task<Empty> SendCommand(SendCommandRequest request, ServerCallContext context)
        {
            // TODO: 未実装
            return new Empty();
        }

        /// <summary>
        /// 部屋の情報を取得する。
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>入室中の部屋情報。</returns>
        public override async Task<GetRoomStatusReply> GetRoomStatus(Empty request, ServerCallContext context)
        {
            var playerId = context.GetPlayerId();
            if (!this.roomRepository.TryGetRoomByPlayerId(playerId, out Room room))
            {
                throw new FailedPreconditionException($"Player ID={playerId} is not joined any room");
            }

            var reply = this.mapper.Map<GetRoomStatusReply>(room);
            foreach (var id in room.PlayerIds)
            {
                reply.Players.Add(this.mapper.Map<PlayerInfo>(await this.playerRepository.FindOrFail(id)));
            }

            return reply;
        }

        #endregion
    }
}
