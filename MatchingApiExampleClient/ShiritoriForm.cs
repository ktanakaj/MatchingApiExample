// ================================================================================================
// <summary>
//      しりとりゲーム画面クラスソース</summary>
//
// <copyright file="ShiritoriForm.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Client
{
    using System;
    using System.Windows.Forms;
    using Grpc.Net.Client;
    using Honememo.MatchingApiExample.Client.Properties;
    using Honememo.MatchingApiExample.Client.Services;
    using Honememo.MatchingApiExample.Protos;

    /// <summary>
    /// しりとりゲーム画面のクラスです。
    /// </summary>
    public partial class ShiritoriForm : Form
    {
        #region メンバー変数

        /// <summary>
        /// ゲーム画面のゲームロジックを扱うサービス。
        /// </summary>
        private readonly ShiritoriFormService service;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 画面を生成する。
        /// </summary>
        public ShiritoriForm(GrpcChannel channel)
        {
            this.InitializeComponent();
            this.service = new ShiritoriFormService(channel);
            this.service.OnGameEvent += this.DoGameEvent;
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
            this.labelResult.Text = string.Empty;
            this.labelCountdown.Text = string.Empty;

            var room = await this.service.GetRoom();
            this.Text = string.Format(this.Text, room.No);
            this.listViewMemberList.Items.Clear();
            foreach (var player in room.Players)
            {
                this.listViewMemberList.Items.Add(new ListViewItem(player.Name));
            }

            await this.service.Ready();
        }

        /// <summary>
        /// 画面クローズ時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void ShiritoriForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.service.Dispose();
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
            var reply = await this.service.Answer(this.textBoxWord.Text);
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
        /// 直近の他人の回答への異議ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ButtonClaim_Click(object sender, EventArgs e)
        {
            await this.service.Claim();
        }

        #endregion

        #region ゲームイベント用メソッド

        /// <summary>
        /// ルーム一覧を再描画する。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void DoGameEvent(object sender, GameEventReply e)
        {
            // TODO: ちゃんとしたログを出す
            this.textBoxLog.Text += $"Type={e.Type}, PlayerId={e.PlayerId}, Word={e.Word}, Result={e.Result}" + Environment.NewLine;
            if (e.Type == ShiritoriEventType.Input && e.PlayerId == Settings.Default.PlayerId)
            {
                this.textBoxWord.Enabled = true;
                this.buttonSubmit.Enabled = true;
            }
        }

        #endregion
    }
}
