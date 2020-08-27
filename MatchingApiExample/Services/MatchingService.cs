// ================================================================================================
// <summary>
//      マッチングサービスクラスソース</summary>
//
// <copyright file="MatchingService.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample
{
    using System.Threading.Tasks;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Microsoft.Extensions.Logging;
    using Honememo.MatchingApiExample.Protos;

    /// <summary>
    /// マッチングサービス。
    /// </summary>
    public class MatchingService : Matching.MatchingBase
    {
        #region メンバー変数

        /// <summary>
        /// ロガー。
        /// </summary>
        private readonly ILogger<MatchingService> logger;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 渡されたインスタンスを使用してサービスを生成する。
        /// </summary>
        /// <param name="logger">ロガー。</param>
        public MatchingService(ILogger<MatchingService> logger)
        {
            this.logger = logger;
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
            // TODO: 未実装
            return new CreateRoomReply
            {
                No = 1
            };
        }

        /// <summary>
        /// 部屋に入る。
        /// </summary>
        /// <param name="request">部屋番号。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>空レスポンス。</returns>
        public override async Task<Empty> JoinRoom(JoinRoomRequest request, ServerCallContext context)
        {
            // TODO: 未実装
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
            // TODO: 未実装
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
            // TODO: 未実装
            return new FindRoomsReply
            {
                Count = 0,
            };
        }

        /// <summary>
        /// 部屋をマッチングする。
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>入室or作成した部屋情報。</returns>
        public override async Task<MatchRoomReply> MatchRoom(Empty request, ServerCallContext context)
        {
            // TODO: 未実装
            return new MatchRoomReply
            {
                No = 1
            };
        }

        #endregion
    }
}
