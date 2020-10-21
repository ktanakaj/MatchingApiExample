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
    using System.Threading.Tasks;
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
    public class Shiritori : IGame
    {
        #region 定数

        /// <summary>
        /// 回答持ち時間（秒）。
        /// </summary>
        private const int InputLimit = 10;

        /// <summary>
        /// 一人のプレイヤーが可能な最大クレーム回数。
        /// </summary>
        private const int MaxClaims = 2;

        /// <summary>
        /// ゲーム開始時に最初の文字の候補。
        /// </summary>
        private const string FirstChars = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわ";

        /// <summary>
        /// ひらがなの小文字/大文字対応表。
        /// </summary>
        private readonly IDictionary<char, char> hiraganaLowerAndUpper = new Dictionary<char, char>()
        {
            { 'ぁ', 'あ' },
            { 'ぃ', 'い' },
            { 'ぅ', 'う' },
            { 'ぇ', 'え' },
            { 'ぉ', 'お' },
            { 'っ', 'つ' },
            { 'ゃ', 'や' },
            { 'ゅ', 'ゆ' },
            { 'ょ', 'よ' },
            { 'ゎ', 'わ' },
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
            this.FireStartEvent();
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

                // 全員が準備完了になったら、最初のプレイヤーに手番を振る
                if (this.IsReady())
                {
                    this.NextTurn(this.PlayerIds.First(), this.RandomFirstChar());
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
        /// <exception cref="InvalidArgumentException">回答が空や対象外の文字列の場合。</exception>
        /// <remarks>回答結果は戻り値とイベントで返す。</remarks>
        public ShiritoriResult Answer(int playerId, string word)
        {
            lock (this.lockObj)
            {
                // 自分に手番が来ていない場合はエラー
                if (!this.TryGetInputTurn(out GameEventArgs input) || input.PlayerId != playerId)
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

                this.FireAnswerEvent(playerId, word, result);

                if (result == ShiritoriResult.Ok)
                {
                    // 正解の場合、次のプレイヤーの手番にする
                    this.usedWords.Add(w);
                    var i = this.PlayerIds.IndexOf(playerId);
                    this.NextTurn(i + 1 < this.PlayerIds.Count() ? this.PlayerIds[i + 1] : this.PlayerIds[0], this.GetLastChar(w));
                }
                else if (result == ShiritoriResult.Gameover)
                {
                    // ゲームオーバーの場合、ゲームを終了する
                    this.FireEndEvent();
                }

                return result;
            }
        }

        /// <summary>
        /// 直前の他人の回答へ異議を申し立てる。
        /// </summary>
        /// <param name="playerId">異議を唱えたプレイヤーのID。</param>
        /// <exception cref="FailedPreconditionException">異議の対象が自分の回答の場合。</exception>
        /// <remarks>
        /// 単語のチェックを行わない代償として、もし不満に思ったプレイヤーが居たら異議を申し立ててもらう。
        /// 一定回数同じプレイヤーから異議が申し立てられたら、ゲーム不成立として引き分け。
        /// </remarks>
        public void Claim(int playerId)
        {
            lock (this.lockObj)
            {
                // 一つ前の手番を取得。対象かチェック
                if (!this.TryGetInputTurn(1, out GameEventArgs input) || input.PlayerId == playerId)
                {
                    throw new FailedPreconditionException($"Claim is not available");
                }

                // 異議を通知
                this.FireClaimEvent(playerId);

                // 一定回数同じプレイヤーから異議が申し立てられたら、ゲーム不成立として引き分け
                if (this.CountClaims(playerId) >= MaxClaims)
                {
                    this.FireAbortEvent();
                    return;
                }

                // それ以外は、前回の手番に戻す
                this.NextTurn(input.PlayerId.Value, input.Word.Last());
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

        #endregion

        #region ゲーム状態に関するメソッド

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
        /// プレイヤーの異議申立の回数を集計する。
        /// </summary>
        /// <param name="playerId">集計するプレイヤーのID。</param>
        /// <returns>合計申立回数。</returns>
        private int CountClaims(int playerId)
        {
            return this.events.Count(e => e.Type == ShiritoriEventType.Claim && e.PlayerId == playerId);
        }

        #endregion

        #region ターン操作用メソッド

        /// <summary>
        /// 現在の回答者の情報を取得する。
        /// </summary>
        /// <param name="input">回答者の情報。</param>
        /// <returns>取得できた場合true。</returns>
        private bool TryGetInputTurn(out GameEventArgs input)
        {
            return this.TryGetInputTurn(0, out input);
        }

        /// <summary>
        /// 回答者の情報を取得する。
        /// </summary>
        /// <param name="prev">過去の回答者の場合、何回前か。※0が最新</param>
        /// <param name="input">回答者の情報。</param>
        /// <returns>取得できた場合true。</returns>
        private bool TryGetInputTurn(int prev, out GameEventArgs input)
        {
            input = null;
            foreach (var e in this.events.Reverse())
            {
                if (e.Type == ShiritoriEventType.End)
                {
                    return false;
                }
                else if (e.Type == ShiritoriEventType.Claim)
                {
                    ++prev;
                }
                else if (e.Type == ShiritoriEventType.Input && prev-- <= 0)
                {
                    input = e;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 次のターンを開始する。
        /// </summary>
        /// <param name="playerId">回答するプレイヤーのID。</param>
        /// <param name="startChar">次の単語の先頭文字。</param>
        private void NextTurn(int playerId, char startChar)
        {
            var e = this.FireInputEvent(playerId, startChar, DateTimeOffset.Now.AddSeconds(InputLimit));
            this.CheckInputLimit(e);
        }

        /// <summary>
        /// 回答の制限時間をチェックする。
        /// </summary>
        /// <param name="e">入力イベント。</param>
        private async void CheckInputLimit(GameEventArgs e)
        {
            await Task.Delay(e.Limit.Value - DateTimeOffset.Now);
            lock (this.lockObj)
            {
                if (this.TryGetInputTurn(out GameEventArgs input) && input == e)
                {
                    this.FireAnswerEvent(e.PlayerId.Value, string.Empty, ShiritoriResult.Gameover);
                    this.FireEndEvent();
                }
            }
        }

        #endregion

        #region 単語処理系メソッド

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
        /// <param name="word">チェックする文字列。※正規化済</param>
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
            else if (this.hiraganaLowerAndUpper.TryGetValue(last, out char upper))
            {
                return upper;
            }

            return last;
        }

        #endregion

        #region イベント発生メソッド

        /// <summary>
        /// ゲーム開始イベントを発生させる。
        /// </summary>
        /// <returns>発生したイベント。</returns>
        private GameEventArgs FireStartEvent()
        {
            var e = new GameEventArgs(ShiritoriEventType.Start);
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
            var e = new GameEventArgs(ShiritoriEventType.Ready) { PlayerId = playerId };
            this.FireGameEvent(e);
            return e;
        }

        /// <summary>
        /// 入力イベントを発生させる。
        /// </summary>
        /// <param name="playerId">入力するプレイヤーのID。</param>
        /// <param name="startChar">次のしりとりの文字。</param>
        /// <param name="limit">制限時間。</param>
        /// <returns>発生したイベント。</returns>
        private GameEventArgs FireInputEvent(int playerId, char startChar, DateTimeOffset limit)
        {
            var e = new GameEventArgs(ShiritoriEventType.Input) { PlayerId = playerId, Word = startChar.ToString(), Limit = limit };
            this.FireGameEvent(e);
            return e;
        }

        /// <summary>
        /// 回答イベントを発生させる。
        /// </summary>
        /// <param name="playerId">回答したプレイヤーのID。</param>
        /// <param name="word">回答した単語。</param>
        /// <param name="result">回答の結果。</param>
        /// <returns>発生したイベント。</returns>
        private GameEventArgs FireAnswerEvent(int playerId, string word, ShiritoriResult result)
        {
            var e = new GameEventArgs(ShiritoriEventType.Answer) { PlayerId = playerId, Word = word, Result = result };
            this.FireGameEvent(e);
            return e;
        }

        /// <summary>
        /// 異議申立イベントを発生させる。
        /// </summary>
        /// <param name="playerId">異議を送信したプレイヤーのID。</param>
        /// <returns>発生したイベント。</returns>
        private GameEventArgs FireClaimEvent(int playerId)
        {
            var e = new GameEventArgs(ShiritoriEventType.Claim) { PlayerId = playerId };
            this.FireGameEvent(e);
            return e;
        }

        /// <summary>
        /// ゲーム終了イベントを発生させる。
        /// </summary>
        /// <returns>発生したイベント。</returns>
        private GameEventArgs FireEndEvent()
        {
            var e = new GameEventArgs(ShiritoriEventType.End);
            this.FireGameEvent(e);
            return e;
        }

        /// <summary>
        /// ゲーム中止イベントを発生させる。
        /// </summary>
        /// <returns>発生したイベント。</returns>
        private GameEventArgs FireAbortEvent()
        {
            var e = new GameEventArgs(ShiritoriEventType.Abort);
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
