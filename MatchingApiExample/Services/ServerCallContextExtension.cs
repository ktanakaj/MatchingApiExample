// ================================================================================================
// <summary>
//      ServerCallContext拡張クラスソース</summary>
//
// <copyright file="ServerCallContextExtension.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Security.Claims;
using Grpc.Core;

namespace Honememo.MatchingApiExample.Services;

/// <summary>
/// <see cref="ServerCallContext"/>の拡張クラス。
/// </summary>
public static class ServerCallContextExtension
{
    /// <summary>
    /// 認証中プレイヤーのIDを取得する。
    /// </summary>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>プレイヤーのID。</returns>
    /// <remarks>このアプリの仕様で認証されたプレイヤーのIDを取得する。</remarks>
    public static int GetPlayerId(this ServerCallContext context)
    {
        return int.Parse(context.GetHttpContext().User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
