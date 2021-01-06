// ================================================================================================
// <summary>
//      アプリケーション起動用クラスソース</summary>
//
// <copyright file="Program.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Client
{
    using System;
    using System.Threading;
    using System.Windows.Forms;
    using Honememo.MatchingApiExample.Client.Utilities;

    /// <summary>
    /// アプリケーション起動時に最初に呼ばれるクラスです。
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// アプリケーションのメインエントリポイントです。
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        /// <summary>
        /// アプリ全体の例外イベントハンドラ。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        /// <remarks>何処にもキャッチされなかった例外を処理する。</remarks>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            // 何処でもキャッチされなかった例外は、アプリが落ちないようにエラーダイアログを出す
            // ※ 実アプリでやる場合はログ出力とかも入れる
            FormUtils.ErrorDialog(e.Exception.Message);
        }
    }
}
