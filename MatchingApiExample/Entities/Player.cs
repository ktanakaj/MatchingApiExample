// ================================================================================================
// <summary>
//      プレイヤーエンティティクラスソース</summary>
//
// <copyright file="Player.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// プレイヤーエンティティクラス。
    /// </summary>
    public class Player : IHasTimestamp
    {
        #region プロパティ

        /// <summary>
        /// プレイヤーID。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// プレイヤー名。
        /// </summary>
        [Required]
        [MaxLength(191)]
        public string Name { get; set; }

        /// <summary>
        /// 端末トークン。
        /// </summary>
        /// <remarks>※ サンプルなので一旦平文。本来はハッシュ化した値を格納する。</remarks>
        [MaxLength(191)]
        public string Token { get; set; }

        /// <summary>
        /// レーティング値。
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// 最終ログイン日時。
        /// </summary>
        public DateTimeOffset? LastLogin { get; set; }

        /// <summary>
        /// 登録日時。
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// 更新日時。
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        #endregion

        #region メソッド

        /// <summary>
        /// モデル構築時に呼ばれる処理。
        /// </summary>
        /// <param name="modelBuilder">モデルビルダー。</param>
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            // インデックスを設定
            modelBuilder.Entity<Player>()
                .HasIndex(u => u.Name).IsUnique();
            modelBuilder.Entity<Player>()
                .HasIndex(u => u.LastLogin);
            modelBuilder.Entity<Player>()
                .HasIndex(u => u.CreatedAt);
        }

        #endregion
    }
}
