// ================================================================================================
// <summary>
//      gRPC勉強用マッチングAPIサンプルしりとりゲーム画面クラスソース</summary>
//
// <copyright file="ShiritoriForm.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Client
{
    using System;
    using System.Threading;
    using System.Windows.Forms;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Honememo.MatchingApiExample.Client.Properties;
    using Honememo.MatchingApiExample.Protos;

    /// <summary>
    /// gRPC勉強用マッチングAPIサンプルしりとりゲーム画面のクラスです。
    /// </summary>
    /// <remarks>TODO: 全体的に動けばいいやの仮実装。</remarks>
    public partial class ShiritoriForm : Form
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
        /// Ready用のキャンセルトークンソース。
        /// </summary>
        private CancellationTokenSource readySource;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 画面を生成する。
        /// </summary>
        public ShiritoriForm(GrpcChannel channel)
        {
            this.channel = channel;
            this.InitializeComponent();
        }

        #endregion

        #region フォームの各イベントのメソッド

        /// <summary>
        /// 画面ロード時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ShiritoriForm_Load(object sender, EventArgs e)
        {
            this.shiritoriService = new Shiritori.ShiritoriClient(this.channel);
            this.matchingService = new Matching.MatchingClient(this.channel);

            this.labelResult.Text = string.Empty;
            this.labelCountdown.Text = string.Empty;

            var room = await this.matchingService.GetRoomAsync(new Empty());
            this.listViewMemberList.Items.Clear();
            foreach (var player in room.Players)
            {
                this.listViewMemberList.Items.Add(new ListViewItem(player.Name));
            }

            using var call = this.shiritoriService.Ready(new Empty());
            try
            {
                this.readySource = new CancellationTokenSource();
                await foreach (var reply in call.ResponseStream.ReadAllAsync(this.readySource.Token))
                {
                    // TODO: ちゃんとしたログを出す
                    this.textBoxLog.Text += $"Type={reply.Type}, PlayerId={reply.PlayerId}, Word={reply.Word}, Result={reply.Result}" + Environment.NewLine;
                    if (reply.Type == ShiritoriEventType.Input && reply.PlayerId == Settings.Default.PlayerId)
                    {
                        this.textBoxWord.Enabled = true;
                        this.buttonSubmit.Enabled = true;
                    }
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
        /// 画面クローズ時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void ShiritoriForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // TODO: 画面クローズ時じゃなくてゲーム終了時にキャンセルする
            this.readySource.Cancel();
        }

        #endregion

        #region ゲーム操作の各イベントのメソッド

        /// <summary>
        /// しりとり入力決定ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ButtonSubmit_Click(object sender, EventArgs e)
        {
            // TODO: もっといろいろやる
            var reply = await this.shiritoriService.AnswerAsync(new AnswerRequest { Word = this.textBoxWord.Text });
            switch (reply.Result)
            {
                // TODO: テキストはみんなリソースから取る
                case ShiritoriResult.Ok:
                    this.labelResult.Text = "〇";
                    this.buttonSubmit.Enabled = false;
                    this.textBoxWord.Enabled = false;
                    break;
                case ShiritoriResult.Ng:
                    this.labelResult.Text = "×";
                    break;
                case ShiritoriResult.Gameover:
                    this.labelResult.Text = "×";
                    this.labelInput.Text = "GameOver";
                    this.buttonSubmit.Enabled = false;
                    this.textBoxWord.Enabled = false;
                    break;
            }
        }

        /// <summary>
        /// しりとり他人のコメントへの異議ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ButtonClaim_Click(object sender, EventArgs e)
        {
            await this.shiritoriService.ClaimAsync(new Empty());
        }

        #endregion

        #region フォーム部品メソッド

        /// <summary>
        /// 単純デザインのエラーダイアログ。
        /// </summary>
        /// <param name="msg">メッセージ。</param>
        private void ErrorDialog(string msg)
        {
            MessageBox.Show(msg, Resources.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}
