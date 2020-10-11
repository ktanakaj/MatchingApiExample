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
    using System.Text.RegularExpressions;
    using Microsoft.VisualBasic;
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
        #region 定数

        /// <summary>
        /// ゲーム開始時に最初の文字の候補。
        /// </summary>
        private const string FirstChars ="あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわ";

        /// <summary>
        /// ひらがなの小文字/大文字対応表。
        /// </summary>
        private readonly IDictionary<char, char> HiraganaLowerAndUpper = new Dictionary<char, char>()
        {
            {'ぁ', 'あ'},
            {'ぃ', 'い'},
            {'ぅ', 'う'},
            {'ぇ', 'え'},
            {'ぉ', 'お'},
            {'っ', 'つ'},
            {'ゃ', 'や'},
            {'ゅ', 'ゆ'},
            {'ょ', 'よ'},
            {'ゎ', 'わ'},
        };

        #endregion

        #region メンバー変数

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
        /// <remarks>
        /// 単純なゲーム的には履歴で持つより現在の情報だけ持った方が楽だが、
        /// 今回は異議申し立ての仕組みがあるので、履歴で持つ。
        /// </remarks>
        private IList<GameEventArgs> events = new List<GameEventArgs>();

        /// <summary>
        /// 回答済みの単語セット。
        /// </summary>
        /// <remarks>正規化した単語を記録する。</remarks>
        private ISet<string> usedWords = new HashSet<string>();

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
        public bool Disposed => this.events.LastOrDefault(e => e.Type == ShiritoriEventType.End || e.Type == ShiritoriEventType.Abort) != null;

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
                    this.FireGameEvent(new GameEventArgs(ShiritoriEventType.Input) { PlayerId = this.PlayerIds.First(), Word = this.RandomFirstChar().ToString() });
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
        /// <exception cref="InvalidArgumentException">回答が空の場合。</exception>
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

                // 単語をひらがなに正規化する
                var w = this.ToNormalize(word);

                // 引数のシステム的なバリデーション
                this.ValidateWord(w);

                // 答えをチェックして結果通知
                var result = ShiritoriResult.Ng;
                if (w.StartsWith(input.Word) && !this.usedWords.Contains(w))
                {
                    result = w.EndsWith('ん') ? ShiritoriResult.Gameover : ShiritoriResult.Ok;
                }

                this.FireGameEvent(new GameEventArgs(ShiritoriEventType.Answer) { PlayerId = playerId, Word = word, Result = result });

                if (result == ShiritoriResult.Ok)
                {
                    // 正解の場合、次のプレイヤーの手番にする
                    // TODO: limitになったらタイムアウトするようにする
                    this.usedWords.Add(w);
                    var i = this.PlayerIds.IndexOf(playerId);
                    var next = i + 1 < this.PlayerIds.Count() ? this.PlayerIds[i + 1] : this.PlayerIds[0];
                    this.FireGameEvent(new GameEventArgs(ShiritoriEventType.Input) { PlayerId = next, Word = this.GetLastChar(w).ToString(), Limit = DateTimeOffset.Now.AddSeconds(10), });
                }
                else if (result == ShiritoriResult.Gameover)
                {
                    // ゲームオーバーの場合、ゲームを終了する
                    this.FireGameEvent(new GameEventArgs(ShiritoriEventType.End));
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
                    this.FireGameEvent(new GameEventArgs(ShiritoriEventType.Abort));
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

        /// <summary>
        /// ゲームスタート時に使う最初の文字を抽選する。
        /// </summary>
        /// <returns>抽選した文字。</returns>
        private char RandomFirstChar()
        {
            // 候補のひらがなの中から、一文字を抽選する
            return FirstChars[this.rand.Next(0, FirstChars.Length)];
        }

        /// <summary>
        /// 入力された単語をバリデーションする。
        /// </summary>
        /// <param name="s">チェックする文字列。※正規化済</param>
        /// <exception cref="InvalidArgumentException">回答がバリデーションNGの場合。</exception>
        /// <remarks>回答としてNGではなく、そもそも引数として渡されるべきではないものをチェックする。</remarks>
        private void ValidateWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                throw new InvalidArgumentException("Word is null or empty");
            }
            else if (word.Length <= 1)
            {
                throw new InvalidArgumentException($"Word length must be longer than 1 (word={word})");
            }
            else if (!Regex.IsMatch(word, @"^(\p{IsHiragana}|ー|－)*$"))
            {
                // ひらがな／カタカナの他、延ばす記号も許容する
                throw new InvalidArgumentException($"Word must be hiragana or katakana only (word={word})");
            }
        }

        /// <summary>
        /// 文字列をしりとり処理用に正規化する。
        /// </summary>
        /// <param name="s">正規化する文字列。</param>
        /// <returns>正規化した文字列。</returns>
        private string ToNormalize(string s)
        {
            // 半角/全角カタカナをひらがなに
            return s != null ? Strings.StrConv(s, VbStrConv.Wide | VbStrConv.Hiragana) : null;
        }

        /// <summary>
        /// しりとり的な最後の文字を取得する。
        /// </summary>
        /// <param name="word">単語。※正規化済</param>
        /// <returns>最後の文字。</returns>
        private char GetLastChar(string word)
        {
            // 小文字を大文字に、「を」は「お」扱い、ハイフン以外で
            var last = word.Where(c => c != 'ー' && c != '－').Last();
            if (last == 'を')
            {
                return 'お';
            }
            else if (this.HiraganaLowerAndUpper.TryGetValue(last, out char upper))
            {
                return upper;
            }

            return last;
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
