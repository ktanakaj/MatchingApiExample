// ================================================================================================
// <summary>
//      ルームリポジトリクラスソース</summary>
//
// <copyright file="RoomRepository.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using Honememo.MatchingApiExample.Entities;
    using Honememo.MatchingApiExample.Exceptions;

    /// <summary>
    /// ルームリポジトリ。
    /// </summary>
    /// <remarks>
    /// ルームをメモリ上で管理するもの。
    /// メモリ上で処理するため、シングルトンなどで用いてください。
    /// </remarks>
    public class RoomRepository
    {
        #region 定数

        /// <summary>
        /// ルーム番号の最小値。
        /// </summary>
        private const int MinNumber = 10;

        /// <summary>
        /// ルーム番号の最大値。
        /// </summary>
        private const int MaxNumber = 9999;

        /// <summary>
        /// 最近使ったルーム番号の履歴件数。
        /// </summary>
        private const int MaxLatestNumbers = 100;

        #endregion

        #region メンバー変数

        /// <summary>
        /// 乱数ジェネレーター。
        /// </summary>
        private readonly Random rand = new Random();

        /// <summary>
        /// ルーム番号とインスタンスのマップ。
        /// </summary>
        private readonly IDictionary<uint, Room> rooms = new Dictionary<uint, Room>();

        /// <summary>
        /// プレイヤーID→ルーム番号の逆引きマップ。
        /// </summary>
        private readonly IDictionary<int, uint> roomNoByPlayerIds = new Dictionary<int, uint>();

        /// <summary>
        /// 最近使ったルーム番号リスト。
        /// </summary>
        /// <remarks>処理の都合上、並び順を持ったセットが欲しいが、標準では無いのでディクショナリで代用。</remarks>
        private readonly OrderedDictionary latestNumbers = new OrderedDictionary();

        #endregion

        #region イベント

        /// <summary>
        /// ルーム更新イベント。
        /// </summary>
        public event EventHandler<Room> OnUpdated;

        #endregion

        #region 公開メソッド

        /// <summary>
        /// 全ルームを取得する。
        /// </summary>
        /// <returns>ルームコレクション。</returns>
        public ICollection<Room> GetRooms()
        {
            return this.rooms.Values;
        }

        /// <summary>
        /// ルームを取得する。
        /// </summary>
        /// <param name="no">ルーム番号。</param>
        /// <param name="room">取得したルーム。</param>
        /// <returns>取得できた場合true。</returns>
        public bool TryGetRoom(uint no, out Room room)
        {
            return this.rooms.TryGetValue(no, out room);
        }

        /// <summary>
        /// ルームを取得する。
        /// </summary>
        /// <param name="no">ルーム番号。</param>
        /// <returns>取得したルーム。</returns>
        /// <exception cref="NotFoundException">ルームが登録されていない場合。</exception>
        public Room GetRoom(uint no)
        {
            if (!this.TryGetRoom(no, out Room room))
            {
                throw new NotFoundException($"Room No={no} is not found");
            }

            return room;
        }

        /// <summary>
        /// プレイヤーが入室中のルームを取得する。
        /// </summary>
        /// <param name="playerId">プレイヤーのID。</param>
        /// <param name="room">取得したルーム。</param>
        /// <returns>取得できた場合true。</returns>
        public bool TryGetRoomByPlayerId(int playerId, out Room room)
        {
            room = null;
            if (!this.roomNoByPlayerIds.TryGetValue(playerId, out uint no))
            {
                return false;
            }

            return this.TryGetRoom(no, out room);
        }

        /// <summary>
        /// ルームを新規作成する。
        /// </summary>
        /// <param name="maxPlayers">ルームの最大人数。</param>
        /// <returns>作成したルーム。</returns>
        public Room CreateRoom(uint maxPlayers)
        {
            Room room;
            lock (this.rooms)
            {
                // リポジトリ内の管理情報と同期させるためにイベントを登録する
                room = new Room(this.GenerateNumber(), maxPlayers);
                room.OnUpdated += (sender, e) => this.OnRoomUpdated((Room)sender, e);
                this.rooms[room.No] = room;
            }

            this.OnUpdated?.Invoke(this, room);
            return room;
        }

        /// <summary>
        /// ルームを取り除く。
        /// </summary>
        /// <param name="no">ルーム番号。</param>
        /// <returns>削除成功の場合true、存在しない場合false。</returns>
        public bool RemoveRoom(uint no)
        {
            lock (this.rooms)
            {
                if (!this.TryGetRoom(no, out Room room))
                {
                    return false;
                }

                // ルーム側のDisposeを呼んで、そこからイベントを呼ばせて削除する
                // （ルームは単独でも扱える想定なので、ルーム側のメソッドだけで処理を完結できるように。）
                room.Dispose();
                return true;
            }
        }

        /// <summary>
        /// ルームに入室中のプレイヤーの一覧を取得する。
        /// </summary>
        /// <returns>プレイヤーIDコレクション。</returns>
        public ICollection<int> GetPlayers()
        {
            return this.roomNoByPlayerIds.Keys;
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// ルーム番号を生成する。
        /// </summary>
        /// <returns>生成したルーム番号。</returns>
        private uint GenerateNumber()
        {
            // ランダムで特定範囲の番号を発行する。
            // ただし、使用中の番号の他、事故防止として最近使った番号や、また念のため分かり易い番号も除外する。
            // ※ 以下の処理は、ルーム番号の使用頻度が低いことを前提として実装。
            //    ルーム番号が同時に大量に発行される場合、無限ループのようになってしまう可能性あり。
            //    その場合は、連番をリストに入れてシャッフルしてそこから割り当てなど別の仕組みを考える。
            while (true)
            {
                var no = (uint)this.rand.Next(MinNumber, MaxNumber);
                if (this.IsGoodNumber(no) && !this.rooms.ContainsKey(no) && !this.latestNumbers.Contains(no))
                {
                    return no;
                }
            }
        }

        /// <summary>
        /// 渡されたルーム番号は良い番号か？
        /// </summary>
        /// <param name="no">チェックする番号。</param>
        /// <returns>良い番号の場合true。</returns>
        private bool IsGoodNumber(uint no)
        {
            // 同じ数字が3桁以上連続するものは除外
            var s = no.ToString();
            var count = 0;
            var lastChar = '0';
            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (lastChar != c)
                {
                    lastChar = c;
                    count = 0;
                }

                if (++count > 2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 最近使ったルーム番号を登録する。
        /// </summary>
        /// <param name="no">登録するルーム番号。</param>
        private void AddLatestNumbers(uint no)
        {
            // 最大件数を超えたら古い方から消す
            this.latestNumbers.Add(no, null);
            if (this.latestNumbers.Count > MaxLatestNumbers)
            {
                this.latestNumbers.RemoveAt(0);
            }
        }

        /// <summary>
        /// ルーム更新時の管理情報更新処理。
        /// </summary>
        /// <param name="room">更新されたルーム。</param>
        /// <param name="e">更新イベント情報。</param>
        private void OnRoomUpdated(Room room, Room.UpdatedEventArgs e)
        {
            // ルームの入室状況などを逆引きマップと同期する
            lock (this.rooms)
            {
                if (!this.rooms.ContainsKey(room.No))
                {
                    // イベント発生時点でルームが既に管理下から削除されている場合は何もしない
                    return;
                }

                // 部屋から退室した人
                foreach (var playerId in e.OldPlayerIds.Except(room.PlayerIds))
                {
                    // ※ 万が一、既に別のルームに入っていたら無視
                    if (this.roomNoByPlayerIds.TryGetValue(playerId, out uint no) && no == room.No)
                    {
                        this.roomNoByPlayerIds.Remove(playerId);
                    }
                }

                // 新しく入室した人
                foreach (var playerId in room.PlayerIds.Except(e.OldPlayerIds))
                {
                    // ※ 万が一、既に別のルームに入っていたら上書き
                    this.roomNoByPlayerIds[playerId] = room.No;
                }

                // ルームが破棄された場合、ルームを管理対象から削除
                if (room.Disposed)
                {
                    this.AddLatestNumbers(room.No);
                    this.rooms.Remove(room.No);
                }
            }

            // イベントを伝播する
            this.OnUpdated?.Invoke(this, room);
        }

        #endregion
    }
}
