// ================================================================================================
// <summary>
//      早押しゲーム画面クラスソース</summary>
//
// <copyright file="ReactionGameForm.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Grpc.Net.Client;
using Honememo.MatchingApiExample.Client.Properties;
using Honememo.MatchingApiExample.Client.Services;
using Honememo.MatchingApiExample.Protos;

namespace Honememo.MatchingApiExample.Client;

/// <summary>
/// 早押しゲーム画面のクラスです。
/// </summary>
public partial class ReactionGameForm : Form
{
    /// <summary>
    /// ゲーム画面のゲームロジックを扱うサービス。
    /// </summary>
    private readonly ReactionGameFormService service;

    /// <summary>
    /// 画面を生成する。
    /// </summary>
    /// <param name="channel">gRPCチャネル。</param>
    public ReactionGameForm(GrpcChannel channel)
    {
        this.InitializeComponent();
        this.service = new ReactionGameFormService(channel);
        this.service.GameEvent += this.DoGameEvent;
    }

    /// <summary>
    /// 画面ロード時のイベント処理。
    /// </summary>
    /// <param name="sender">イベント発生元インスタンス。</param>
    /// <param name="e">イベントパラメータ。</param>
    private async void ReactionGameForm_Load(object sender, EventArgs e)
    {
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
    private void ReactionGameForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        this.service.Dispose();
    }

    /// <summary>
    /// 早押しボタンクリック時のイベント処理。
    /// </summary>
    /// <param name="sender">イベント発生元インスタンス。</param>
    /// <param name="e">イベントパラメータ。</param>
    private async void ButtonSubmit_Click(object sender, EventArgs e)
    {
        // 結果はストリーム経由のDoGameEventで反映
        this.buttonSubmit.Enabled = false;
        await this.service.Submit(DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// ゲームイベント受信時の処理。
    /// </summary>
    /// <param name="sender">イベント発生元インスタンス。</param>
    /// <param name="e">イベントパラメータ。</param>
    private void DoGameEvent(object sender, GameEventReply e)
    {
        // TODO: ちゃんとしたログを出す
        var dateSuffix = e.Date != null ? $", Date={e.Date.ToDateTimeOffset():HH:mm:ss.fff}" : string.Empty;
        this.textBoxLog.Text += string.Format("Type={0}, PlayerId={1}, Result={2}", e.Type, e.PlayerId, dateSuffix) + Environment.NewLine;

        switch (e.Type)
        {
            case ReactionGameEventType.Start:
                this.labelMessage.Text = Resources.ReactionGameStarting;
                break;
            case ReactionGameEventType.Submitable:
                this.labelMessage.Text = Resources.ReactionGamePressNow;
                if (e.PlayerId == 0 || e.PlayerId == Settings.Default.PlayerId)
                {
                    this.buttonSubmit.Enabled = true;
                }

                break;
            case ReactionGameEventType.Submitted:
                this.buttonSubmit.Enabled = false;
                this.labelMessage.Text = (e.PlayerId == Settings.Default.PlayerId)
                                        ? Resources.ReactionGameYouWin
                                        : string.Format(Resources.ReactionGamePlayerWon, e.PlayerId);
                break;
            case ReactionGameEventType.End:
            case ReactionGameEventType.Abort:
                this.labelMessage.Text = Resources.ReactionGameEnded;
                this.buttonSubmit.Enabled = false;
                break;
        }
    }

    /// <summary>
    /// ログを追加する。
    /// </summary>
    /// <param name="message">ログメッセージ。</param>
    /// <param name="date">ログ日時。</param>
    private void AddLog(string message, DateTimeOffset date)
    {
        this.textBoxLog.Text += string.Format("[{0:HH:mm:ss.fff}] {1}", date, message) + Environment.NewLine;
    }
}