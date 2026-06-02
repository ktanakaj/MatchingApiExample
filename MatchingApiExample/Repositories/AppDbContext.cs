// ================================================================================================
// <summary>
//      アプリケーションDBコンテキストクラスソース</summary>
//
// <copyright file="AppDbContext.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Honememo.MatchingApiExample.Entities;
using Microsoft.EntityFrameworkCore;

namespace Honememo.MatchingApiExample.Repositories;

/// <summary>
/// アプリケーションDBコンテキストクラス。
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// コンテキストを生成する。
    /// </summary>
    /// <param name="options">オプション。</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// プレイヤーテーブル。
    /// </summary>
    public DbSet<Player> Players { get; set; }
}