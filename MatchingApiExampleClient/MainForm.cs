// ================================================================================================
// <summary>
//      gRPC勉強用マッチングAPIサンプル主画面クラスソース</summary>
//
// <copyright file="MainForm.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Client
{
    using System.Windows.Forms;

    /// <summary>
    /// gRPC勉強用マッチングAPIサンプル主画面のクラスです。
    /// </summary>
    public partial class MainForm : Form
    {
        #region コンストラクタ

        /// <summary>
        /// 画面を生成する。
        /// </summary>
        public MainForm()
        {
            // 初期化メソッド呼び出しのみ。
            this.InitializeComponent();
        }

        #endregion
    }
}
