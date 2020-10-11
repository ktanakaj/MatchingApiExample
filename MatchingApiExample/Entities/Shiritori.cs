// ================================================================================================
// <summary>
//      しりとりゲームクラスソース</summary>
//
// <copyright file="Shiritori.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Honememo.MatchingApiExample.Exceptions;
    using Honememo.MatchingApiExample.Protos;

    /// <summary>
    /// しりとりゲームのデータ等を扱うクラス。
    /// </summary>
    /// <remarks>
    /// しりとりゲームクラスはゲームのルールなどをモデル化したもの。
    /// メモリ上で管理される。エンティティにあるがDBとは紐づかない。
    /// </remarks>
    public class Shiritori : IGame, IDisposable
    {
        #region メンバー変数

        /// <summary>
        /// ロックオブジェクト。
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// ゲームイベントの履歴。
        /// </summary>
        /// <remarks>
        /// 単純なゲーム的には履歴で持つより現在の情報だけ持った方が楽だが、
        /// 今回は異議申し立ての仕組みがあるので、履歴で持つ。
        /// </remarks>
        private IList<GameEventArgs> events = new List<GameEventArgs>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 渡されたプレイヤーでしりとりゲームを開始する。
        /// </summary>
        /// <param name="playerIds">ゲームを行うプレイヤーのID配列。</param>
        /// <exception cref="InvalidArgumentException">プレイヤーが二人未満の場合。</exception>
        public Shiritori(ICollection<int> playerIds)
        {
            if (playerIds.Count <= 1)
            {
                throw new InvalidArgumentException($"Players must be greater than 2 (count={playerIds.Count})");
            }

            // ※ スタートイベントはコンストラクタなので通知しようがないが一応記録
            this.PlayerIds = playerIds.ToImmutableList();
            this.FireGameEvent(new GameEventArgs(ShiritoriEventType.Start));
        }

        #endregion

        #region イベント

        /// <summary>
        /// ゲームイベント。
        /// </summary>
        public event EventHandler<GameEventArgs> OnGameEvent;

        #endregion

        #region プロパティ

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
        public bool Disposed => this.events.LastOrDefault(e => e.Type == ShiritoriEventType.End) != null;

        #endregion

        #region 公開メソッド

        /// <summary>
        /// プレイヤーの準備を完了する。
        /// </summary>
        /// <param name="playerId">準備完了にするプレイヤー。</param>
        public void Ready(int playerId)
        {
            lock (this.lockObj)
            {
                // 念のため、既に準備完了済で呼ばれた場合は無視する
                this.ThrowExceptionIfDisposed();
                if (this.IsReady(playerId))
                {
                    return;
                }

                // 準備完了イベントを起こす
                this.FireGameEvent(new GameEventArgs(ShiritoriEventType.Ready) { PlayerId = playerId });

                // 全員が準備完了になったら、最初のプレイヤーに手番を振る
                if (this.IsReady())
                {
                    // TODO: 最初の文字はランダムで抽選する
                    this.FireGameEvent(new GameEventArgs(ShiritoriEventType.Input) { PlayerId = this.PlayerIds.First(), Word = "り" });
                }
            }
        }

        /// <summary>
        /// 自分の手番に単語を回答する。
        /// </summary>
        /// <param name="playerId">回答するプレイヤーのID。</param>
        /// <param name="word">回答した単語。</param>
        /// <returns>回答の結果。</returns>
        /// <exception cref="FailedPreconditionException">プレイヤーの手番でない場合。</exception>
        /// <remarks>回答結果は戻り値とイベントで返す。</remarks>
        public ShiritoriResult Answer(int playerId, string word)
        {
            lock (this.lockObj)
            {
                // 自分に手番が来ていない場合はエラー
                var input = this.GetInputTurn();
                if (input.PlayerId != playerId)
                {
                    throw new FailedPreconditionException($"Player ID={playerId} is not turn");
                }

                // 答えをチェックして結果通知
                // TODO: ひらがなカタカナのみや文字数のバリデーションとかもここ？
                // TODO: 使用済みのワードもチェック
                var result = ShiritoriResult.Ng;
                if (word.StartsWith(input.Word))
                {
                    result = word.EndsWith('ん') ? ShiritoriResult.Gameover : ShiritoriResult.Ok;
                }

                this.FireGameEvent(new GameEventArgs(ShiritoriEventType.Answer) { PlayerId = playerId, Word = word, Result = result });

                if (result == ShiritoriResult.Ok)
                {
                    // 正解の場合、次のプレイヤーの手番にする
                    // TODO: limitになったらタイムアウトするようにする
                    var i = this.PlayerIds.IndexOf(playerId);
                    var next = i + 1 < this.PlayerIds.Count() ? this.PlayerIds[i + 1] : this.PlayerIds[0];
                    this.FireGameEvent(new GameEventArgs(ShiritoriEventType.Input) { PlayerId = next, Word = word.Last().ToString(), Limit = DateTimeOffset.Now.AddSeconds(10), });
                }
                else if (result == ShiritoriResult.Gameover)
                {
                    // ゲームオーバーの場合、ゲームを終了する
                    this.Dispose();
                }

                return result;
            }
        }

        /// <summary>
        /// 直前の他人の回答へ異議を申し立てる。
        /// </summary>
        /// <param name="playerId">異議を唱えたプレイヤーのID。</param>
        public void Claim(int playerId)
        {
            lock (this.lockObj)
            {
                // 単語のチェックを行わない代償として、もし不満に思ったプレイヤーが居たら異議を申し立ててもらう。
                // 一定回数同じプレイヤーから異議が申し立てられたら、ゲーム不成立として引き分け。
                // TODO: 遡れる回数のチェックなど入れる
                this.FireGameEvent(new GameEventArgs(ShiritoriEventType.Claim) { PlayerId = playerId });

                // TODO: 前回の手番に戻す
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
                    this.FireGameEvent(new GameEventArgs(ShiritoriEventType.End));
                }
            }

            // イベント後にイベントハンドラーも消しておく
            this.OnGameEvent = null;
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// 現在の回答者の情報を取得する。
        /// </summary>
        /// <returns>回答者の情報。</returns>
        /// <exception cref="InvalidOperationException">ゲームが進行中でない場合。</exception>
        private GameEventArgs GetInputTurn()
        {
            foreach (var e in this.events.Reverse())
            {
                if (e.Type == ShiritoriEventType.End)
                {
                    break;
                }
                else if (e.Type == ShiritoriEventType.Input)
                {
                    return e;
                }
            }

            throw new InvalidOperationException("Game is not started");
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
        /// ゲームイベントを発生させる。
        /// </summary>
        /// <param name="e">発生させるイベント。nullの場合無視。</param>
        private void FireGameEvent(GameEventArgs e)
        {
            if (e != null)
            {
                this.events.Add(e);
                this.OnGameEvent?.Invoke(this, e);
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
            return this.events.FirstOrDefault(e => e.Type == ShiritoriEventType.Ready && e.PlayerId == playerId) != null;
        }

        /// <summary>
        /// ゲームが準備完了済みか？
        /// </summary>
        /// <returns>準備完了済の場合true。</returns>
        private bool IsReady()
        {
            // 全プレイヤーが準備完了済みならOK
            return this.events.Count(e => e.Type == ShiritoriEventType.Ready) >= this.PlayerIds.Count();
        }

        #endregion

        #region 内部クラス

        /// <summary>
        /// <see cref="OnGameEvent"/> のイベントパラメータクラス。
        /// </summary>
        public class GameEventArgs : EventArgs
        {
            #region コンストラクタ

            /// <summary>
            /// 指定された種類のゲームイベントを生成する。
            /// </summary>
            /// <param name="type">イベントの種類。</param>
            public GameEventArgs(ShiritoriEventType type)
            {
                this.Type = type;
            }

            #endregion

            #region プロパティ

            // ※ 使用するプロパティはイベントの種類ごとに異なる。

            /// <summary>
            /// ゲームイベントの種類。
            /// </summary>
            public ShiritoriEventType Type { get; }

            /// <summary>
            /// イベントが発生したプレイヤーのID。
            /// </summary>
            public int? PlayerId { get; set; }

            /// <summary>
            /// 回答された単語。
            /// </summary>
            public string Word { get; set; }

            /// <summary>
            /// 回答の結果。
            /// </summary>
            public ShiritoriResult? Result { get; set; }

            /// <summary>
            /// 回答の期限。
            /// </summary>
            public DateTimeOffset? Limit { get; set; }

            #endregion
        }

        #endregion
    }
}
