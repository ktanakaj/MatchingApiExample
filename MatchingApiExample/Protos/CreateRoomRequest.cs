// ================================================================================================
// <summary>
//      部屋作成の引数クラスソース</summary>
//
// <copyright file="CreateRoomRequest.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Protos
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 部屋作成の引数。
    /// </summary>
    /// <remarks>gRPCモデルクラスのバリデーション定義用。</remarks>
    public sealed partial class CreateRoomRequest : IValidatableObject
    {
        #region メソッド

        /// <summary>
        /// モデルのバリデーションを実行する。
        /// </summary>
        /// <param name="validationContext">バリデーションコンテキスト。</param>
        /// <returns>バリデーション結果。</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.MaxPlayers < 2)
            {
                yield return new ValidationResult("Max Players must be greater than 2", new[] { nameof(this.MaxPlayers) });
            }
        }

        #endregion
    }
}
