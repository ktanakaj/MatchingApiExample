// ================================================================================================
// <summary>
//      ゲームリポジトリクラスソース</summary>
//
// <copyright file="GameRepository.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Repositories
{
    using System.Collections.Generic;
    using Honememo.MatchingApiExample.Entities;
    using Honememo.MatchingApiExample.Exceptions;

    /// <summary>
    /// ゲームリポジトリ。
    /// </summary>
    /// <remarks>
    /// 各種ゲームをメモリ上で管理するもの。
    /// メモリ上で処理するため、シングルトンなどで用いてください。
    /// </remarks>
    public class GameRepository
    {
        #region メンバー変数

        /// <summary>
        /// ゲームIDとインスタンスのマップ。
        /// </summary>
        private readonly IDictionary<string, IGame> games = new Dictionary<string, IGame>();

        #endregion

        #region 公開メソッド

        /// <summary>
        /// 全ゲームを取得する。
        /// </summary>
        /// <returns>ゲームコレクション。</returns>
        public ICollection<IGame> GetGames()
        {
            return this.games.Values;
        }

        /// <summary>
        /// ゲームを取得する。
        /// </summary>
        /// <param name="id">ゲームID。</param>
        /// <param name="game">取得したゲーム。</param>
        /// <returns>取得できた場合true。</returns>
        public bool TryGetGame(string id, out IGame game)
        {
            return this.games.TryGetValue(id, out game);
        }

        /// <summary>
        /// ゲームを取得する。
        /// </summary>
        /// <param name="id">ゲームID。</param>
        /// <returns>取得したゲーム。</returns>
        /// <exception cref="NotFoundException">ゲームが登録されていない場合。</exception>
        public IGame GetGame(string id)
        {
            if (!this.TryGetGame(id, out IGame game))
            {
                throw new NotFoundException($"Game Id={id} is not found");
            }

            return game;
        }

        /// <summary>
        /// ゲームを登録する。
        /// </summary>
        /// <param name="game">登録するゲーム。</param>
        public void AddGame(IGame game)
        {
            this.games[game.Id] = game;
        }

        /// <summary>
        /// ゲームを取り除く。
        /// </summary>
        /// <param name="id">ゲームID。</param>
        /// <returns>削除成功の場合true、存在しない場合false。</returns>
        public bool RemoveGame(string id)
        {
            return this.games.Remove(id);
        }

        #endregion
    }
}
