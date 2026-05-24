// ================================================================================================
// <summary>
//      フォーム処理に関するユーティリティクラスソース。</summary>
//
// <copyright file="FormUtils.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Windows.Forms;

// ※ プロパティを含むので、そのまま他のプロジェクトに流用することはできない
using Honememo.MatchingApiExample.Client.Properties;

namespace Honememo.MatchingApiExample.Client.Utilities;

/// <summary>
/// フォーム処理に関するユーティリティクラスです。
/// </summary>
public static class FormUtils
{
    /// <summary>
    /// 単純デザインのエラーダイアログ（入力された文字列を表示）。
    /// </summary>
    /// <param name="msg">メッセージ。</param>
    public static void ErrorDialog(string msg)
    {
        // 渡された文字列でエラーダイアログを表示
        MessageBox.Show(
            msg,
            Resources.ErrorTitle,
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    /// <summary>
    /// 単純デザインのエラーダイアログ（入力された文字列を書式化して表示）。
    /// </summary>
    /// <param name="format">書式項目を含んだメッセージ。</param>
    /// <param name="args">書式設定対象オブジェクト配列。</param>
    public static void ErrorDialog(string format, params object[] args)
    {
        // オーバーロードメソッドをコール
        FormUtils.ErrorDialog(string.Format(format, args));
    }
}
