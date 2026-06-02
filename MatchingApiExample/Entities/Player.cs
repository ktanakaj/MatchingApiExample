// ================================================================================================
// <summary>
//      プレイヤーエンティティクラスソース</summary>
//
// <copyright file="Player.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Honememo.MatchingApiExample.Entities;

/// <summary>
/// プレイヤーエンティティクラス。
/// </summary>
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(LastLogin))]
[Index(nameof(CreatedAt))]
public class Player : IHasCreatedAt, IHasUpdatedAt
{
    /// <summary>
    /// プレイヤーID。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// プレイヤー名。
    /// </summary>
    [Required]
    [MaxLength(255)]
    public required string Name { get; set; }

    /// <summary>
    /// 端末トークン。
    /// </summary>
    /// <remarks>※ サンプルなので一旦平文。本来はハッシュ化した値を格納する。</remarks>
    [MaxLength(255)]
    public required string Token { get; set; }

    /// <summary>
    /// レーティング値。
    /// </summary>
    public ushort Rating { get; set; }

    /// <summary>
    /// 最終ログイン日時。
    /// </summary>
    public DateTimeOffset? LastLogin { get; set; }

    /// <summary>
    /// 登録日時。
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 更新日時。
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
