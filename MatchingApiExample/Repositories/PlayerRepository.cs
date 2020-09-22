// ================================================================================================
// <summary>
//      プレイヤーリポジトリクラスソース</summary>
//
// <copyright file="PlayerRepository.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Honememo.MatchingApiExample.Entities;
    using Honememo.MatchingApiExample.Exceptions;

    /// <summary>
    /// プレイヤーリポジトリクラス。
    /// </summary>
    public class PlayerRepository
    {
        #region メンバー変数

        /// <summary>
        /// アプリケーションDBコンテキスト。
        /// </summary>
        private readonly AppDbContext context;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンテキストをDIしてリポジトリを生成する。
        /// </summary>
        /// <param name="context">アプリケーションDBコンテキスト。</param>
        public PlayerRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #endregion

        #region 参照系メソッド

        /// <summary>
        /// プレイヤーを全て取得する。
        /// </summary>
        /// <returns>プレイヤー。</returns>
        public async Task<IList<Player>> FindAll()
        {
            return await this.context.Players.OrderBy(b => b.Name).ThenBy(b => b.Id).ToListAsync();
        }

        /// <summary>
        /// プレイヤーIDでプレイヤーを取得する。
        /// </summary>
        /// <param name="id">プレイヤーID。</param>
        /// <returns>プレイヤー。</returns>
        public async Task<IList<Player>> Find(ICollection<int> ids)
        {
            return await this.context.Players.Where(p => ids.Contains(p.Id)).OrderBy(b => b.Name).ThenBy(b => b.Id).ToListAsync();
        }

        /// <summary>
        /// プレイヤーIDでプレイヤーを取得する。
        /// </summary>
        /// <param name="id">プレイヤーID。</param>
        /// <returns>プレイヤー。</returns>
        public async Task<Player> Find(int id)
        {
            return await this.context.Players.FindAsync(id);
        }

        /// <summary>
        /// プレイヤーIDでプレイヤーを取得する。
        /// </summary>
        /// <param name="id">プレイヤーID。</param>
        /// <returns>プレイヤー。</returns>
        /// <exception cref="NotFoundException">プレイヤーが存在しない場合。</exception>
        public async Task<Player> FindOrFail(int id)
        {
            var player = await this.Find(id);
            if (player == null)
            {
                throw new NotFoundException($"id={id} is not found");
            }

            return player;
        }

        #endregion

        #region 更新系メソッド

        /// <summary>
        /// プレイヤーを登録する。
        /// </summary>
        /// <param name="player">プレイヤー。</param>
        /// <returns>登録したプレイヤー。</returns>
        public async Task<Player> Create(Player player)
        {
            player.Id = 0;
            this.context.Players.Add(player);
            await this.context.SaveChangesAsync();
            return player;
        }

        /// <summary>
        /// プレイヤーを更新する。
        /// </summary>
        /// <param name="player">プレイヤー。</param>
        /// <returns>更新したプレイヤー。</returns>
        public async Task<Player> Update(Player player)
        {
            this.context.Entry(player).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
            return player;
        }

        /// <summary>
        /// プレイヤーを削除する。
        /// </summary>
        /// <param name="id">プレイヤーID。</param>
        /// <returns>削除したプレイヤー。</returns>
        /// <exception cref="NotFoundException">プレイヤーが存在しない場合。</exception>
        public async Task<Player> Delete(int id)
        {
            var player = await this.FindOrFail(id);
            this.context.Players.Remove(player);
            await this.context.SaveChangesAsync();
            return player;
        }

        #endregion
    }
}
