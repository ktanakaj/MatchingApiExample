// ================================================================================================
// <summary>
//      プレイヤー登録の引数クラスソース</summary>
//
// <copyright file="SignUpRequest.cs">
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
    /// プレイヤー登録の引数。
    /// </summary>
    /// <remarks>gRPCモデルクラスのバリデーション定義用。</remarks>
    public sealed partial class SignUpRequest : IValidatableObject
    {
        #region メソッド

        /// <summary>
        /// モデルのバリデーションを実行する。
        /// </summary>
        /// <param name="validationContext">バリデーションコンテキスト。</param>
        /// <returns>バリデーション結果。</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(this.Token))
            {
                yield return new ValidationResult("Token is required", new[] { nameof(this.Token) });
            }
        }

        #endregion
    }
}
