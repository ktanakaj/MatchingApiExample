// ================================================================================================
// <summary>
//      しりとりゲームサービスクラスソース</summary>
//
// <copyright file="ShiritoriService.cs">
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
    using Honememo.MatchingApiExample.Protos;
    using Honememo.MatchingApiExample.Repositories;

    /// <summary>
    /// しりとりゲームサービス。
    /// </summary>
    [Authorize]
    public class ShiritoriService : Shiritori.ShiritoriBase
    {
        #region メンバー変数

        /// <summary>
        /// ロガー。
        /// </summary>
        private readonly ILogger<ShiritoriService> logger;

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
        public ShiritoriService(ILogger<ShiritoriService> logger, IMapper mapper, RoomRepository roomRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.roomRepository = roomRepository;
        }

        #endregion

        #region gRPCメソッド

        /// <summary>
        /// ゲームを準備完了にする。
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="responseStream">レスポンス用のストリーム。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>空レスポンス。</returns>
        public override async Task Ready(Empty request, IServerStreamWriter<GameEventReply> responseStream, ServerCallContext context)
        {
            // TODO: 未実装
        }

        /// <summary>
        /// 自分の手番に単語を回答する。
        /// </summary>
        /// <param name="request">回答。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>回答結果。</returns>
        public override async Task<AnswerReply> Answer(AnswerRequest request, ServerCallContext context)
        {
            // TODO: 未実装
            return new AnswerReply();
        }

        /// <summary>
        /// 直前の他人の回答に異議を送信する。
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>空レスポンス。</returns>
        public override async Task<Empty> Claim(Empty request, ServerCallContext context)
        {
            // TODO: 未実装
            return new Empty();
        }

        #endregion
    }
}
