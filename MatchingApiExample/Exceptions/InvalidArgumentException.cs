// ================================================================================================
// <summary>
//      不正な入力値の例外クラスソース</summary>
//
// <copyright file="InvalidArgumentException.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Exceptions
{
    using System;

    /// <summary>
    /// 不正な入力値の例外クラス。
    /// </summary>
    public class InvalidArgumentException : AppException
    {
        /// <summary>
        /// 渡されたエラーメッセージと発生元の例外で不正な入力値の例外を生成する。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        /// <param name="innerException">発生元の例外。</param>
        public InvalidArgumentException(string message, Exception innerException = null) : base(message, "INVALID_ARGUMENT", innerException)
        {
        }
    }
}
