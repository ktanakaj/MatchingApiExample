﻿// ================================================================================================
// <summary>
//      不正なリクエストの例外クラスソース</summary>
//
// <copyright file="BadRequestException.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Exceptions
{
    using System;

    /// <summary>
    /// 不正なリクエストの例外クラス。
    /// </summary>
    public class BadRequestException : AppException
    {
        /// <summary>
        /// 渡されたエラーメッセージと発生元の例外で不正なリクエストの例外を生成する。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        /// <param name="innerException">発生元の例外。</param>
        public BadRequestException(string message,  Exception innerException = null) : base(message, "BAD_REQUEST", innerException)
        {
        }
    }
}