// ================================================================================================
// <summary>
//      プレイヤー変更の引数クラスソース</summary>
//
// <copyright file="ChangeMeRequest.cs">
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
    /// プレイヤー変更の引数。
    /// </summary>
    /// <remarks>gRPCモデルクラスのバリデーション定義用。</remarks>
    public sealed partial class ChangeMeRequest : IValidatableObject
    {
        #region メソッド

        /// <summary>
        /// モデルのバリデーションを実行する。
        /// </summary>
        /// <param name="validationContext">バリデーションコンテキスト。</param>
        /// <returns>バリデーション結果。</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                yield return new ValidationResult("Name is required", new[] { nameof(this.Name) });
            }

            if (this.Rating > ushort.MaxValue)
            {
                yield return new ValidationResult($"Rating must be smaller than {ushort.MaxValue}", new[] { nameof(this.Rating) });
            }
        }

        #endregion
    }
}
