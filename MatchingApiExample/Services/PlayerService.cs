// ================================================================================================
// <summary>
//      プレイヤー関連サービスクラスソース</summary>
//
// <copyright file="PlayerService.cs">
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
    /// プレイヤー関連サービス。
    /// </summary>
    public class PlayerService : Player.PlayerBase
    {
        #region メンバー変数

        /// <summary>
        /// ロガー。
        /// </summary>
        private readonly ILogger<PlayerService> logger;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 渡されたインスタンスを使用してサービスを生成する。
        /// </summary>
        /// <param name="logger">ロガー。</param>
        public PlayerService(ILogger<PlayerService> logger)
        {
            this.logger = logger;
        }

        #endregion

        #region gRPCメソッド

        /// <summary>
        /// プレイヤーを登録する。
        /// </summary>
        /// <param name="request">端末情報。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>登録したプレイヤー情報。</returns>
        public override async Task<PlayerInfo> SignUp(SignUpRequest request, ServerCallContext context)
        {
            // TODO: 未実装
            return new PlayerInfo
            {
                Id = 1
            };
        }

        /// <summary>
        /// プレイヤーを認証する。
        /// </summary>
        /// <param name="request">認証情報。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>認証したプレイヤー情報。</returns>
        public override async Task<PlayerInfo> SignIn(SignInRequest request, ServerCallContext context)
        {
            // TODO: 未実装
            return new PlayerInfo
            {
                Id = 1
            };
        }

        /// <summary>
        /// 認証中のプレイヤー情報を取得する
        /// </summary>
        /// <param name="request">空パラメータ。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>取得したプレイヤー情報。</returns>
        public override async Task<PlayerInfo> FindMe(Empty request, ServerCallContext context)
        {
            // TODO: 未実装
            return new PlayerInfo
            {
                Id = 1
            };
        }

        /// <summary>
        /// 認証中のプレイヤーの情報を変更する。
        /// </summary>
        /// <param name="request">変更するプレイヤー情報。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <returns>更新したプレイヤー情報。</returns>
        public override async Task<PlayerInfo> ChangeMe(ChangeMeRequest request, ServerCallContext context)
        {
            // TODO: 未実装
            return new PlayerInfo
            {
                Id = 1
            };
        }

        #endregion
    }
}
