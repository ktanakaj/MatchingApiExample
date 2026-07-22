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
    /// 合図からタイムアウトまでの時間（ミリ秒）。
    /// </summary>
    private const int TimeoutMs = 5000;

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

        this.PlayerIds = playerIds.ToImmutableList();
    }

    /// <summary>
    /// ゲームイベント。
    /// </summary>
    public event EventHandler<GameEventArgs>? GameEvent;

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
            this.RaiseReadyEvent(playerId);

            // 全員が準備完了したらゲーム開始
            if (this.AllReady())
            {
                this.RaiseStartEvent();
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

            // 既に押下済みの場合は無視
            if (this.submissions.ContainsKey(playerId))
            {
                return;
            }

            this.submissions[playerId] = pressDate;
            this.RaiseSubmittedEvent(playerId, pressDate);

            // 最後の一人が押したらゲーム終了
            if (this.submissions.Count >= this.PlayerIds.Count)
            {
                this.End();
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
                this.RaiseAbortEvent();
            }
        }

        // イベントハンドラーも消しておく
        this.GameEvent = null;
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
    /// ゲームを終了する。
    /// </summary>
    private void End()
    {
        lock (this.lockObj)
        {
            // 押下時間を比較して、勝者を判定する。合図後に一番最初に押した人が勝者
            // ※ 全員合図前に押すなど、勝者は居ないこともあり得る。
            var winner = this.events.Where(e => e.Type == ReactionGameEventType.Submitted && e.Date >= this.submitableAt)
                .OrderBy(e => e.Date).FirstOrDefault();
            this.winnerId = winner?.PlayerId;
            this.RaiseEndEvent(this.winnerId, winner?.Date);
        }
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
                    this.RaiseSubmitableEvent();
                    this.ScheduleEnd();
                }
            }
        });
    }

    /// <summary>
    /// 一定時間後にENDを発火するスケジュール。
    /// </summary>
    private void ScheduleEnd()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(TimeoutMs);
            lock (this.lockObj)
            {
                if (!this.Disposed && this.submitableAt.HasValue)
                {
                    this.End();
                }
            }
        });
    }

    /// <summary>
    /// ゲーム開始イベントを発生させる。
    /// </summary>
    /// <returns>発生したイベント。</returns>
    private GameEventArgs RaiseStartEvent()
    {
        var e = new GameEventArgs(ReactionGameEventType.Start);
        this.RaiseGameEvent(e);
        return e;
    }

    /// <summary>
    /// 準備完了イベントを発生させる。
    /// </summary>
    /// <param name="playerId">準備が完了したプレイヤーのID。</param>
    /// <returns>発生したイベント。</returns>
    private GameEventArgs RaiseReadyEvent(int playerId)
    {
        var e = new GameEventArgs(ReactionGameEventType.Ready) { PlayerId = playerId };
        this.RaiseGameEvent(e);
        return e;
    }

    /// <summary>
    /// 押下開始イベントを発生させる。
    /// </summary>
    /// <returns>発生したイベント。</returns>
    private GameEventArgs RaiseSubmitableEvent()
    {
        var e = new GameEventArgs(ReactionGameEventType.Submitable) { Date = this.submitableAt };
        this.RaiseGameEvent(e);
        return e;
    }

    /// <summary>
    /// 押下イベントを発生させる。
    /// </summary>
    /// <param name="playerId">押下したプレイヤーのID。</param>
    /// <param name="pressDate">押下日時。</param>
    /// <returns>発生したイベント。</returns>
    private GameEventArgs RaiseSubmittedEvent(int playerId, DateTimeOffset pressDate)
    {
        var e = new GameEventArgs(ReactionGameEventType.Submitted) { PlayerId = playerId, Date = pressDate };
        this.RaiseGameEvent(e);
        return e;
    }

    /// <summary>
    /// ゲーム終了イベントを発生させる。
    /// </summary>
    /// <param name="playerId">勝利したプレイヤーのID。勝者無しはnull。</param>
    /// <param name="pressDate">押下日時。勝者無しはnull。</param>
    /// <returns>発生したイベント。</returns>
    private GameEventArgs RaiseEndEvent(int? playerId, DateTimeOffset? pressDate)
    {
        var e = new GameEventArgs(ReactionGameEventType.End) { PlayerId = playerId, Date = pressDate };
        this.RaiseGameEvent(e);
        return e;
    }

    /// <summary>
    /// ゲーム中止イベントを発生させる。
    /// </summary>
    /// <returns>発生したイベント。</returns>
    private GameEventArgs RaiseAbortEvent()
    {
        var e = new GameEventArgs(ReactionGameEventType.Abort);
        this.RaiseGameEvent(e);
        return e;
    }

    /// <summary>
    /// ゲームイベントを発生させる。
    /// </summary>
    /// <param name="e">発生させるイベント。nullの場合無視。</param>
    private void RaiseGameEvent(GameEventArgs e)
    {
        // 発火させるだけでなく、履歴にも登録する
        if (e != null)
        {
            this.events.Add(e);
            this.GameEvent?.Invoke(this, e);
        }
    }

    /// <summary>
    /// <see cref="GameEvent"/> のイベントパラメータクラス。
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
        /// イベント関連日時（合図時刻や押下時刻）。
        /// </summary>
        public DateTimeOffset? Date { get; set; }
    }
}
