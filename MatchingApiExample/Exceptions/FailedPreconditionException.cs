// ================================================================================================
// <summary>
//      実行条件を満たさない場合の例外クラスソース</summary>
//
// <copyright file="FailedPreconditionException.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Exceptions
{
    using System;

    /// <summary>
    /// 実行条件を満たさない場合の例外クラス。
    /// </summary>
    public class FailedPreconditionException : AppException
    {
        /// <summary>
        /// 渡されたエラーメッセージと発生元の例外で実行条件を満たさない場合の例外を生成する。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        /// <param name="innerException">発生元の例外。</param>
        public FailedPreconditionException(string message, Exception innerException = null)
            : base(message, "FAILED_PRECONDITION", innerException)
        {
        }
    }
}
