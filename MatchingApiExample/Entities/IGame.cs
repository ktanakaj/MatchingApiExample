// ================================================================================================
// <summary>
//      各種ゲーム用の基底インタフェースソース</summary>
//
// <copyright file="IGame.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Entities
{
    /// <summary>
    /// 各種ゲーム用の基底インタフェース。
    /// </summary>
    public interface IGame
    {
        #region プロパティ

        /// <summary>
        /// ゲームごとに一意なID。
        /// </summary>
        public string Id { get; }

        #endregion
    }
}
