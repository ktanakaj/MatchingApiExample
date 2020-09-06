// ================================================================================================
// <summary>
//      タイムスタンプを持つエンティティのインタフェースソース</summary>
//
// <copyright file="IHasTimestamp.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Entities
{
    using System;

    /// <summary>
    /// タイムスタンプを持つエンティティのインタフェース。
    /// </summary>
    public interface IHasTimestamp
    {
        /// <summary>
        /// 作成日時。
        /// </summary>
        DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// 更新日時。
        /// </summary>
        DateTimeOffset? UpdatedAt { get; set; }
    }
}
