// ================================================================================================
// <summary>
//      ルームエンティティクラスソース</summary>
//
// <copyright file="Room.cs">
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

    /// <summary>
    /// プレイヤーが入室するルームを表すクラス。
    /// </summary>
    /// <remarks>
    /// ルームは現状メモリ上で管理するため、エンティティにあるがDBとは紐づかない。
    /// </remarks>
    public class Room : IDisposable
    {
        #region メンバー変数

        /// <summary>
        /// ロックオブジェクト。
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// ルームに入室中のプレイヤーのリスト。
        /// </summary>
        private readonly IList<int> playerIds = new List<int>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定された条件のルームのインスタンスを生成する。
        /// </summary>
        /// <param name="no">ルーム番号。</param>
        /// <param name="maxPlayers">ルームの最大人数。</param>
        public Room(uint no, ushort maxPlayers)
        {
            this.No = no;
            this.MaxPlayers = maxPlayers;
        }

        #endregion

        #region イベント

        /// <summary>
        /// ルーム更新イベント。
        /// </summary>
        public event EventHandler<UpdatedEventArgs> OnUpdated;

        #endregion

        #region プロパティ

        /// <summary>
        /// ルーム番号。
        /// </summary>
        public uint No { get; }

        /// <summary>
        /// ルームの最大人数。
        /// </summary>
        public ushort MaxPlayers { get; }

        /// <summary>
        /// レーティング値。
        /// </summary>
        public ushort Rating { get; private set; }

        /// <summary>
        /// ルームに入室中のプレイヤーのリスト。
        /// </summary>
        /// <remarks>
        /// リストは読み取り専用です。更新する場合は
        /// <see cref="AddPlayer(int)"/>, <see cref="RemovePlayer(int)"/>
        /// のメソッドを呼んでください。
        /// </remarks>
        public IList<int> PlayerIds
        {
            get { return this.playerIds.ToImmutableList(); }
        }

        /// <summary>
        /// ルームの作成日時。
        /// </summary>
        public DateTimeOffset CreatedAt { get; } = DateTimeOffset.Now;

        /// <summary>
        /// ルーム削除済みか？
        /// </summary>
        public bool Disposed { get; private set; }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// ルームにプレイヤーを入室させる。
        /// </summary>
        /// <param name="player">入室するプレイヤー。</param>
        /// <exception cref="InvalidOperationException">既に入室済／満員の場合。</exception>
        /// <exception cref="ObjectDisposedException">ルームが破棄済みの場合。</exception>
        public void AddPlayer(Player player)
        {
            UpdatedEventArgs e;
            lock (this.lockObj)
            {
                this.ThrowExceptionIfDisposed();
                if (this.IsFull())
                {
                    throw new InvalidOperationException($"Room No={this.No} is full");
                }

                // 同一プレイヤーIDの場合に重複と判断
                if (this.playerIds.Contains(player.Id))
                {
                    throw new InvalidOperationException($"Player ID={player.Id} is already exists");
                }

                e = new UpdatedEventArgs(this);
                this.playerIds.Add(player.Id);
                this.UpdateRating((int)player.Rating);
            }

            this.FireUpdatedIfNeeded(e);
        }

        /// <summary>
        /// ルームからプレイヤーを退室させる。
        /// </summary>
        /// <param name="player">退室させるプレイヤーのID。</param>
        /// <returns>退室した場合true、プレイヤーが存在しない場合false。</returns>
        /// <exception cref="ObjectDisposedException">ルームが破棄済みの場合。</exception>
        public bool RemovePlayer(Player player)
        {
            UpdatedEventArgs e;
            lock (this.lockObj)
            {
                this.ThrowExceptionIfDisposed();
                e = new UpdatedEventArgs(this);
                if (this.playerIds.Remove(player.Id))
                {
                    this.UpdateRating((int)-player.Rating);
                }
                else
                {
                    e = null;
                }
            }

            this.FireUpdatedIfNeeded(e);
            return e != null;
        }

        /// <summary>
        /// ルームが満員か？
        /// </summary>
        /// <returns>満員の場合true。</returns>
        public bool IsFull()
        {
            return this.playerIds.Count() >= this.MaxPlayers;
        }

        /// <summary>
        /// ルームを破棄する。
        /// </summary>
        public void Dispose()
        {
            // ルームは単体でも使われうるので、念のためこのルームは再使用できないと分かるようにフラグを立てる。
            // またプレイヤーなども一応全員追い出しておく。
            UpdatedEventArgs e = null;
            lock (this.lockObj)
            {
                if (!this.Disposed)
                {
                    e = new UpdatedEventArgs(this);
                    this.Disposed = true;
                    this.playerIds.Clear();
                }
            }

            // イベント後にイベントハンドラーも消しておく
            this.FireUpdatedIfNeeded(e);
            this.OnUpdated = null;
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// ルームが破棄済みの場合例外を投げる。
        /// </summary>
        /// <exception cref="ObjectDisposedException">ルームが破棄済みの場合。</exception>
        private void ThrowExceptionIfDisposed()
        {
            if (this.Disposed)
            {
                throw new ObjectDisposedException($"Room No={this.No} is disposed");
            }
        }

        /// <summary>
        /// ルーム更新イベントを発生させる。
        /// </summary>
        /// <param name="e">発生させるイベント。nullの場合無視。</param>
        private void FireUpdatedIfNeeded(UpdatedEventArgs e)
        {
            if (e != null)
            {
                this.OnUpdated?.Invoke(this, e);
            }
        }

        /// <summary>
        /// プレイヤー入退室による部屋のレーティング値を更新する。
        /// </summary>
        /// <param name="diff">入退室により変動したレーティング値。退室はマイナス。</param>
        /// <remarks>
        /// 単純に入室中のプレイヤーのレーティング値の平均を部屋のレーティング値とする。
        /// <see cref="PlayerIds"/>を更新してから呼ぶこと。
        /// </remarks>
        private void UpdateRating(int diff)
        {
            var newPlayers = this.playerIds.Count;
            if (newPlayers == 0)
            {
                this.Rating = 0;
                return;
            }

            if (diff == 0)
            {
                return;
            }

            // 増減がある場合は、現在のレーティング値と人数から、元々のプレイヤーのレーティング値の合計を算出し、
            // そこに新しい人の分を加減算して、新しい人数で割る。
            // ※ この仕組みだと端数がずれるし、また入室後の増減も加味されないが、
            //    別に厳密な値が必要なわけじゃないので気にしない。
            var oldPlayers = newPlayers + (diff > 0 ? -1 : 1);
            this.Rating = (ushort)(((oldPlayers * this.Rating) + diff) / newPlayers);
        }

        #endregion

        #region 内部クラス

        /// <summary>
        /// <see cref="OnUpdated"/> のイベントパラメータクラス。
        /// </summary>
        public class UpdatedEventArgs : EventArgs
        {
            #region コンストラクタ

            /// <summary>
            /// 渡されたルーム用のイベントパラメータを生成する。
            /// </summary>
            /// <param name="room">イベントが発生したルーム。</param>
            /// <remarks>
            /// 主に更新前の情報を保持するので、更新前のルームを渡して生成する。
            /// </remarks>
            public UpdatedEventArgs(Room room)
            {
                this.OldPlayerIds = room.PlayerIds;
            }

            #endregion

            #region プロパティ

            /// <summary>
            /// 更新前のプレイヤーのリスト。
            /// </summary>
            public IList<int> OldPlayerIds { get; }

            #endregion
        }

        #endregion
    }
}
