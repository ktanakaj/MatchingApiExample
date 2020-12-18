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
    using System;

    /// <summary>
    /// 各種ゲーム用の基底インタフェース。
    /// </summary>
    /// <remarks>
    /// ゲームロジックと部屋はモデル上で別々に分かれているが、
    /// あくまで実装を整理/抽象化するためのもので、
    /// 複数のゲームに対応しているわけではないので注意。
    /// </remarks>
    public interface IGame : IDisposable
    {
        #region プロパティ

        /// <summary>
        /// ゲームごとに一意なID。
        /// </summary>
        public string Id { get; }

        #endregion
    }
}
