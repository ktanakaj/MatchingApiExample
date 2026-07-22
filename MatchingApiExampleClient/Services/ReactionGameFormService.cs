// ================================================================================================
// <summary>
//      早押しゲーム画面サービスクラスソース</summary>
//
// <copyright file="ReactionGameFormService.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Honememo.MatchingApiExample.Protos;

namespace Honememo.MatchingApiExample.Client.Services;

/// <summary>
/// 早押しゲーム画面のサービスを受け持つクラスです。
/// </summary>
/// <remarks>ReactionGameForm肥大化に伴いGUIの古典的MVCに置けるMを切り出したクラス。</remarks>
public class ReactionGameFormService : IDisposable
{
    /// <summary>
    /// gRPCチャネル。
    /// </summary>
    private GrpcChannel channel;

    /// <summary>
    /// 早押しゲームサービスのクライアント。
    /// </summary>
    private ReactionGame.ReactionGameClient reactionGameService;

    /// <summary>
    /// マッチングサービスのクライアント。
    /// </summary>
    private Matching.MatchingClient matchingService;

    /// <summary>
    /// ゲームイベント通知用のキャンセルトークンソース。
    /// </summary>
    private CancellationTokenSource readySource;

    /// <summary>
    /// 接続済みのgRPCチャネルを使用するサービスを生成する。
    /// </summary>
    /// <param name="channel">gRPCチャネル。</param>
    public ReactionGameFormService(GrpcChannel channel)
    {
        this.channel = channel;
        this.reactionGameService = new ReactionGame.ReactionGameClient(this.channel);
        this.matchingService = new Matching.MatchingClient(this.channel);
    }

    /// <summary>
    /// ゲームイベント。
    /// </summary>
    public event EventHandler<GameEventReply> GameEvent;

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
        using var call = this.reactionGameService.Ready(new Empty());
        try
        {
            this.readySource = new CancellationTokenSource();
            await foreach (var reply in call.ResponseStream.ReadAllAsync(this.readySource.Token))
            {
                this.GameEvent?.Invoke(this, reply);
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
    /// ボタンを押す。
    /// </summary>
    /// <param name="pressTime">押下日時。</param>
    /// <returns>処理状態。</returns>
    public async Task Submit(DateTimeOffset pressTime)
    {
        var ts = Timestamp.FromDateTimeOffset(pressTime);
        await this.reactionGameService.SubmitAsync(new SubmitRequest { Date = ts });
    }

    /// <summary>
    /// サービスを解放する。
    /// </summary>
    public void Dispose()
    {
        this.UnsubscribeGameEventSource();
    }

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
}
