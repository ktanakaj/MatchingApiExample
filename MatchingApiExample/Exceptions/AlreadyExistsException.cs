// ================================================================================================
// <summary>
//      データ重複の例外クラスソース</summary>
//
// <copyright file="AlreadyExistsException.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Exceptions
{
    using System;

    /// <summary>
    /// データ重複の例外クラス。
    /// </summary>
    public class AlreadyExistsException : AppException
    {
        /// <summary>
        /// 渡されたエラーメッセージと追加情報でデータ重複の例外を生成する。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        /// <param name="data">エラーの追加情報。</param>
        public AlreadyExistsException(string message, System.Collections.IDictionary data = null) : base(message, "ALREADY_EXISTS", data)
        {
        }
    }
}
