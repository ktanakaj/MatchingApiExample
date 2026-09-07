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
    /// プレイヤーIDとプレイヤー名の対応表。
    /// </summary>
    private readonly IDictionary<int, string> playerNames = new Dictionary<int, string>();

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
            this.playerNames[player.Id] = player.Name;
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
    /// もう一度プレイボタンクリック時のイベント処理。
    /// </summary>
    /// <param name="sender">イベント発生元インスタンス。</param>
    /// <param name="e">イベントパラメータ。</param>
    private async void ButtonReady_Click(object sender, EventArgs e)
    {
        await this.service.Ready();
    }

    /// <summary>
    /// 閉じるボタンクリック時のイベント処理。
    /// </summary>
    /// <param name="sender">イベント発生元インスタンス。</param>
    /// <param name="e">イベントパラメータ。</param>
    private void ButtonClose_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    /// <summary>
    /// ゲームイベント受信時の処理。
    /// </summary>
    /// <param name="sender">イベント発生元インスタンス。</param>
    /// <param name="e">イベントパラメータ。</param>
    private void DoGameEvent(object sender, GameEventReply e)
    {
        // イベント種別に応じて画面表示を更新
        string logMessage = null;
        switch (e.Type)
        {
            case ReactionGameEventType.Ready:
                this.labelMessage.Text = Resources.ReactionGameWaiting;
                logMessage = string.Format(Resources.ReactionGameReady, this.GetPlayerName(e.PlayerId));
                break;
            case ReactionGameEventType.Start:
                this.labelMessage.Text = Resources.ReactionGameStarting;
                this.buttonReady.Visible = false;
                this.buttonClose.Visible = false;
                this.buttonSubmit.Visible = true;
                break;
            case ReactionGameEventType.Submitable:
                this.labelMessage.Text = Resources.ReactionGamePressNow;
                this.buttonSubmit.Enabled = true;
                break;
            case ReactionGameEventType.Submitted:
                if (e.PlayerId == Settings.Default.PlayerId)
                {
                    this.labelMessage.Text = Resources.ReactionGameResultWait;
                    this.buttonSubmit.Enabled = false;
                }

                logMessage = string.Format(Resources.ReactionGameSubmitted, this.GetPlayerName(e.PlayerId));
                break;
            case ReactionGameEventType.End:
                if (e.PlayerId == 0)
                {
                    this.labelMessage.Text = Resources.ReactionGameDraw;
                }
                else if (e.PlayerId == Settings.Default.PlayerId)
                {
                    this.labelMessage.Text = Resources.ReactionGameYouWin;
                }
                else
                {
                    this.labelMessage.Text = string.Format(Resources.ReactionGamePlayerWon, this.GetPlayerName(e.PlayerId));
                }

                this.buttonSubmit.Visible = false;
                this.buttonReady.Visible = true;
                break;
            case ReactionGameEventType.Abort:
                this.labelMessage.Text = Resources.ReactionGameAbort;
                this.buttonSubmit.Visible = false;
                this.buttonClose.Visible = true;
                break;
        }

        // ログダイアログにも出力（主に動作検証用）
        this.AddLog(logMessage ?? this.labelMessage.Text, e.Date?.ToDateTimeOffset());
    }

    /// <summary>
    /// ログを追加する。
    /// </summary>
    /// <param name="message">ログメッセージ。</param>
    /// <param name="date">ログ日時。未指定は現在日時。</param>
    private void AddLog(string message, DateTimeOffset? date = null)
    {
        this.textBoxLog.Text += string.Format("[{0:HH:mm:ss.fff}] {1}", date ?? DateTimeOffset.UtcNow, message) + Environment.NewLine;
    }

    /// <summary>
    /// プレイヤーIDからプレイヤー名を取得する。
    /// </summary>
    /// <param name="playerId">プレイヤーID。</param>
    /// <returns>プレイヤー名。</returns>
    private string GetPlayerName(int playerId)
    {
        if (this.playerNames.TryGetValue(playerId, out var playerName))
        {
            return playerName;
        }

        return playerId.ToString();
    }
}