// ================================================================================================
// <summary>
//      早押しゲームクラスソース</summary>
//
// <copyright file="ReactionGame.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Collections.Immutable;
using Honememo.MatchingApiExample.Exceptions;
using Honememo.MatchingApiExample.Protos;

namespace Honememo.MatchingApiExample.Entities;

/// <summary>
/// 早押しゲームのデータ等を扱うクラス。
/// </summary>
/// <remarks>
/// 早押しゲームクラスはゲームのルールなどをモデル化したもの。
/// メモリ上で管理される。エンティティにあるがDBとは紐づかない。
/// </remarks>
public class ReactionGame : IGame
{
    /// <summary>
    /// 合図 (SUBMITABLE) までの最小遅延（ミリ秒）。
    /// </summary>
    private const int MinCueDelayMs = 800;

    /// <summary>
    /// 合図 (SUBMITABLE) までの最大遅延（ミリ秒）。
    /// </summary>
    private const int MaxCueDelayMs = 3500;

    /// <summary>
    /// ロックオブジェクト。
    /// </summary>
    private readonly object lockObj = new object();

    /// <summary>
    /// 乱数ジェネレータ。
    /// </summary>
    private readonly Random rand = new Random();

    /// <summary>
    /// ゲームイベントの履歴。
    /// </summary>
    private readonly IList<GameEventArgs> events = new List<GameEventArgs>();

    /// <summary>
    /// 各プレイヤーの押下記録 (playerId -> press time)。
    /// </summary>
    private readonly Dictionary<int, DateTimeOffset> submissions = new Dictionary<int, DateTimeOffset>();

    /// <summary>
    /// 合図が出た日時（これ以降の押下が有効）。
    /// </summary>
    private DateTimeOffset? submitableAt;

    /// <summary>
    /// 勝者プレイヤーID（最初に有効押下した者）。
    /// </summary>
    private int? winnerId;

    /// <summary>
    /// 渡されたプレイヤーで早押しゲームを開始する。
    /// </summary>
    /// <param name="playerIds">ゲームを行うプレイヤーのID配列。</param>
    /// <exception cref="InvalidArgumentException">プレイヤーが二人未満の場合。</exception>
    public ReactionGame(ICollection<int> playerIds)
    {
        if (playerIds.Count <= 1)
        {
            throw new InvalidArgumentException($"Players must be greater than 1 (count={playerIds.Count})");
        }

        // スタートは全Ready後（ScheduleSubmitableでランダム合図）
        this.PlayerIds = playerIds.ToImmutableList();
    }

    /// <summary>
    /// ゲームイベント。
    /// </summary>
    public event EventHandler<GameEventArgs>? OnGameEvent;

    /// <summary>
    /// ゲームごとに一意なID。
    /// </summary>
    public string Id { get; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ゲームをプレイ中のプレイヤーのリスト。
    /// </summary>
    public IList<int> PlayerIds { get; }

    /// <summary>
    /// ゲームの開始日時。
    /// </summary>
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.Now;

    /// <summary>
    /// ゲーム終了済みか？
    /// </summary>
    public bool Disposed => this.events.LastOrDefault(e => e.Type == ReactionGameEventType.End || e.Type == ReactionGameEventType.Abort) != null;

    /// <summary>
    /// 勝者プレイヤーID（有効な最初押下者）。
    /// </summary>
    public int? WinnerId => this.winnerId;

    /// <summary>
    /// プレイヤーの準備を完了する。
    /// </summary>
    /// <param name="playerId">準備完了にするプレイヤー。</param>
    /// <exception cref="FailedPreconditionException">プレイヤーがゲーム参加者ではない場合。</exception>
    public void Ready(int playerId)
    {
        lock (this.lockObj)
        {
            if (!this.PlayerIds.Contains(playerId))
            {
                throw new FailedPreconditionException($"Player ID={playerId} is not joined in Game ID={this.Id}");
            }

            // 念のため、既に準備完了済で呼ばれた場合は無視する
            this.ThrowExceptionIfDisposed();
            if (this.IsReady(playerId))
            {
                return;
            }

            // 準備完了イベントを起こす
            this.FireReadyEvent(playerId);

            if (this.AllReady())
            {
                this.FireStartEvent();
                this.ScheduleSubmitable();
            }
        }
    }

    /// <summary>
    /// ボタンを押す（早押し）。
    /// </summary>
    /// <param name="playerId">押したプレイヤーのID。</param>
    /// <param name="pressDate">クライアント報告の押下日時。</param>
    /// <exception cref="FailedPreconditionException">プレイヤーが参加者でない場合。</exception>
    public void Submit(int playerId, DateTimeOffset pressDate)
    {
        lock (this.lockObj)
        {
            this.ThrowExceptionIfDisposed();
            if (!this.PlayerIds.Contains(playerId))
            {
                throw new FailedPreconditionException($"Player ID={playerId} is not joined in Game ID={this.Id}");
            }

            if (!this.submitableAt.HasValue || this.Disposed)
            {
                this.FireSubmittedEvent(playerId, ReactionGameResult.Ng, pressDate);
                return;
            }

            if (this.submissions.ContainsKey(playerId))
            {
                return;
            }

            this.submissions[playerId] = pressDate;

            if (this.winnerId == null)
            {
                this.winnerId = playerId;
                this.FireSubmittedEvent(playerId, ReactionGameResult.Ok, pressDate);
                this.FireEndEvent();
            }
            else
            {
                this.FireSubmittedEvent(playerId, ReactionGameResult.Ng, pressDate);
            }
        }
    }

    /// <summary>
    /// ゲームを破棄する。
    /// </summary>
    public void Dispose()
    {
        lock (this.lockObj)
        {
            if (!this.Disposed)
            {
                this.FireAbortEvent();
            }
        }

        // イベント後にイベントハンドラーも消しておく
        this.OnGameEvent = null;
    }

    /// <summary>
    /// ゲームが破棄済みの場合例外を投げる。
    /// </summary>
    /// <exception cref="ObjectDisposedException">ゲームが破棄済みの場合。</exception>
    private void ThrowExceptionIfDisposed()
    {
        if (this.Disposed)
        {
            throw new ObjectDisposedException($"Game ID={this.Id} is disposed");
        }
    }

    /// <summary>
    /// プレイヤーが準備完了済みか？
    /// </summary>
    /// <param name="playerId">チェックするプレイヤー。</param>
    /// <returns>準備完了済の場合true。</returns>
    private bool IsReady(int playerId)
    {
        // 準備完了イベントは先頭にある筈なので先頭から見る
        return this.events.FirstOrDefault(e => e.Type == ReactionGameEventType.Ready && e.PlayerId == playerId) != null;
    }

    /// <summary>
    /// 全員が準備完了済みか？
    /// </summary>
    /// <returns>準備完了済の場合true。</returns>
    private bool AllReady()
    {
        // 全プレイヤーが準備完了済みならOK
        return this.events.Count(e => e.Type == ReactionGameEventType.Ready) >= this.PlayerIds.Count;
    }

    /// <summary>
    /// ランダム遅延後にSUBMITABLEを発火するスケジュール。
    /// </summary>
    private void ScheduleSubmitable()
    {
        var delayMs = this.rand.Next(MinCueDelayMs, MaxCueDelayMs + 1);
        _ = Task.Run(async () =>
        {
            await Task.Delay(delayMs);
            lock (this.lockObj)
            {
                if (!this.Disposed && !this.submitableAt.HasValue)
                {
                    this.submitableAt = DateTimeOffset.UtcNow;
                    this.FireSubmitableEvent();
                }
            }
        });
    }

    /// <summary>
    /// ゲーム開始イベントを発生させる。
    /// </summary>
    /// <returns>発生したイベント。</returns>
    private GameEventArgs FireStartEvent()
    {
        var e = new GameEventArgs(ReactionGameEventType.Start);
        this.FireGameEvent(e);
        return e;
    }

    /// <summary>
    /// 準備完了イベントを発生させる。
    /// </summary>
    /// <param name="playerId">準備が完了したプレイヤーのID。</param>
    /// <returns>発生したイベント。</returns>
    private GameEventArgs FireReadyEvent(int playerId)
    {
        var e = new GameEventArgs(ReactionGameEventType.Ready) { PlayerId = playerId };
        this.FireGameEvent(e);
        return e;
    }

    /// <summary>
    /// 押下開始イベントを発生させる。
    /// </summary>
    /// <returns>発生したイベント。</returns>
    private GameEventArgs FireSubmitableEvent()
    {
        var e = new GameEventArgs(ReactionGameEventType.Submitable) { Date = this.submitableAt };
        this.FireGameEvent(e);
        return e;
    }

    /// <summary>
    /// 押下イベントを発生させる。
    /// </summary>
    /// <param name="playerId">回答したプレイヤーのID。</param>
    /// <param name="result">押下の結果。</param>
    /// <param name="pressDate">押下日時。</param>
    /// <returns>発生したイベント。</returns>
    private GameEventArgs FireSubmittedEvent(int playerId, ReactionGameResult result, DateTimeOffset? pressDate = null)
    {
        var e = new GameEventArgs(ReactionGameEventType.Submitted) { PlayerId = playerId, Result = result, Date = pressDate };
        this.FireGameEvent(e);
        return e;
    }

    /// <summary>
    /// ゲーム終了イベントを発生させる。
    /// </summary>
    /// <returns>発生したイベント。</returns>
    private GameEventArgs FireEndEvent()
    {
        var e = new GameEventArgs(ReactionGameEventType.End);
        this.FireGameEvent(e);
        return e;
    }

    /// <summary>
    /// ゲーム中止イベントを発生させる。
    /// </summary>
    /// <returns>発生したイベント。</returns>
    private GameEventArgs FireAbortEvent()
    {
        var e = new GameEventArgs(ReactionGameEventType.Abort);
        this.FireGameEvent(e);
        return e;
    }

    /// <summary>
    /// ゲームイベントを発生させる。
    /// </summary>
    /// <param name="e">発生させるイベント。nullの場合無視。</param>
    private void FireGameEvent(GameEventArgs e)
    {
        // 発火させるだけでなく、履歴にも登録する
        if (e != null)
        {
            this.events.Add(e);
            this.OnGameEvent?.Invoke(this, e);
        }
    }

    /// <summary>
    /// <see cref="OnGameEvent"/> のイベントパラメータクラス。
    /// </summary>
    public class GameEventArgs : EventArgs
    {
        /// <summary>
        /// 指定された種類のゲームイベントを生成する。
        /// </summary>
        /// <param name="type">イベントの種類。</param>
        public GameEventArgs(ReactionGameEventType type)
        {
            this.Type = type;
        }

        // ※ 使用するプロパティはイベントの種類ごとに異なる。

        /// <summary>
        /// ゲームイベントの種類。
        /// </summary>
        public ReactionGameEventType Type { get; }

        /// <summary>
        /// イベントが発生したプレイヤーのID。
        /// </summary>
        public int? PlayerId { get; set; }

        /// <summary>
        /// 結果 (OK/NG)。
        /// </summary>
        public ReactionGameResult? Result { get; set; }

        /// <summary>
        /// イベント関連日時（合図時刻や押下時刻）。
        /// </summary>
        public DateTimeOffset? Date { get; set; }
    }
}
