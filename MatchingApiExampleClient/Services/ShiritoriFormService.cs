// ================================================================================================
// <summary>
//      しりとりゲーム画面サービスクラスソース</summary>
//
// <copyright file="ShiritoriFormService.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Client.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Honememo.MatchingApiExample.Protos;

    /// <summary>
    /// しりとりゲーム画面のサービスを受け持つクラスです。
    /// </summary>
    /// <remarks>ShiritoriForm肥大化に伴いGUIの古典的MVCに置けるMを切り出したクラス。</remarks>
    public class ShiritoriFormService : IDisposable
    {
        #region メンバー変数

        /// <summary>
        /// gRPCチャネル。
        /// </summary>
        private GrpcChannel channel;

        /// <summary>
        /// しりとりゲームサービスのクライアント。
        /// </summary>
        private Shiritori.ShiritoriClient shiritoriService;

        /// <summary>
        /// マッチングサービスのクライアント。
        /// </summary>
        private Matching.MatchingClient matchingService;

        /// <summary>
        /// ゲームイベント通知用のキャンセルトークンソース。
        /// </summary>
        private CancellationTokenSource readySource;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 接続済みのgRPCチャネルを使用するサービスを生成する。
        /// </summary>
        public ShiritoriFormService(GrpcChannel channel)
        {
            this.channel = channel;
            this.shiritoriService = new Shiritori.ShiritoriClient(this.channel);
            this.matchingService = new Matching.MatchingClient(this.channel);
        }

        #endregion

        #region イベント

        /// <summary>
        /// ゲームイベント。
        /// </summary>
        public event EventHandler<GameEventReply> OnGameEvent;

        #endregion

        #region 公開メソッド

        /// <summary>
        /// 入室中のルームの状態を取得する。
        /// </summary>
        /// <returns>ルーム情報。</returns>
        public async Task<GetRoomReply> GetRoom()
        {
            return await this.matchingService.GetRoomAsync(new Empty());
        }

        /// <summary>
        /// ゲームを準備完了にする。
        /// </summary>
        /// <returns>処理状態。</returns>
        public async Task Ready()
        {
            this.UnsubscribeGameEventSource();
            using var call = this.shiritoriService.Ready(new Empty());
            try
            {
                this.readySource = new CancellationTokenSource();
                await foreach (var reply in call.ResponseStream.ReadAllAsync(this.readySource.Token))
                {
                    this.OnGameEvent?.Invoke(this, reply);
                }
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode != StatusCode.Cancelled)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 自分の手番に単語を回答する。
        /// </summary>
        /// <param name="word">単語。</param>
        /// <returns>処理状態。</returns>
        public async Task<AnswerReply> Answer(string word)
        {
            return await this.shiritoriService.AnswerAsync(new AnswerRequest { Word = word });
        }

        /// <summary>
        /// 直前の他人の回答に異議を送信する。
        /// </summary>
        /// <returns>処理状態。</returns>
        public async Task Claim()
        {
            await this.shiritoriService.ClaimAsync(new Empty());
        }

        /// <summary>
        /// サービスを解放する。
        /// </summary>
        public void Dispose()
        {
            this.UnsubscribeGameEventSource();
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// ゲームイベントの通知を解除する。
        /// </summary>
        private void UnsubscribeGameEventSource()
        {
            if (this.readySource != null)
            {
                this.readySource.Cancel();
                this.readySource = null;
            }
        }

        #endregion
    }
}
