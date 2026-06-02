// ================================================================================================
// <summary>
//      汎用の業務エラー例外クラスソース</summary>
//
// <copyright file="AppException.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Exceptions;

/// <summary>
/// 汎用の業務エラー例外クラス。
/// </summary>
public class AppException : Exception
{
    /// <summary>
    /// 渡されたエラーメッセージとエラーコード、追加情報で業務例外を生成する。
    /// </summary>
    /// <param name="message">エラーメッセージ。</param>
    /// <param name="code">エラーコード。</param>
    /// <param name="data">エラーの追加情報。</param>
    public AppException(string message, string code, System.Collections.IDictionary? data)
        : this(message, code, data, null)
    {
    }

    /// <summary>
    /// 渡されたエラーメッセージとエラーコード、発生元の例外で業務例外を生成する。
    /// </summary>
    /// <param name="message">エラーメッセージ。</param>
    /// <param name="code">エラーコード。</param>
    /// <param name="innerException">発生元の例外。</param>
    public AppException(string message, string code, Exception? innerException)
        : this(message, code, null, innerException)
    {
    }

    /// <summary>
    /// 渡されたエラーメッセージとエラーコード、追加情報、発生元の例外で業務例外を生成する。
    /// </summary>
    /// <param name="message">エラーメッセージ。</param>
    /// <param name="code">エラーコード。</param>
    /// <param name="data">エラーの追加情報。</param>
    /// <param name="innerException">発生元の例外。</param>
    public AppException(string message, string code, System.Collections.IDictionary? data, Exception? innerException)
        : base(message, innerException)
    {
        this.Code = code;
        this.Data = data ?? new System.Collections.Hashtable();
    }

    /// <summary>
    /// エラーコード。
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// 追加情報。
    /// </summary>
    public override System.Collections.IDictionary Data { get; }
}
