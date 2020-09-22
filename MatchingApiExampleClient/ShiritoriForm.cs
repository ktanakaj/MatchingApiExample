// ================================================================================================
// <summary>
//      gRPC勉強用マッチングAPIサンプルしりとりゲーム画面クラスソース</summary>
//
// <copyright file="ShiritoriForm.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Net.Client;
    using Honememo.MatchingApiExample.Protos;

    /// <summary>
    /// gRPC勉強用マッチングAPIサンプルしりとりゲーム画面のクラスです。
    /// </summary>
    /// <remarks>TODO: 全体的に動けばいいやの仮実装。</remarks>
    public partial class ShiritoriForm : Form
    {
        #region メンバー変数

        /// <summary>
        /// gRPCチャネル。
        /// </summary>
        private GrpcChannel channel;

        /// <summary>
        /// しりとりゲームサービスのクライアント。
        /// </summary>
        private Shiritori.ShiritoriClient shiritoriService;

        /// <summary>
        /// Ready用のキャンセルトークンソース。
        /// </summary>
        private CancellationTokenSource readySource;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 画面を生成する。
        /// </summary>
        public ShiritoriForm()
        {
            // 初期化メソッド呼び出しのみ。
            this.InitializeComponent();
        }

        #endregion

        #region フォームの各イベントのメソッド

        /// <summary>
        /// 画面ロード時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void ShiritoriForm_Load(object sender, EventArgs e)
        {
            // TODO: 画面の初期表示を行う
        }

        #endregion

        #region ゲーム操作の各イベントのメソッド

        /// <summary>
        /// しりとり入力決定ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void ButtonSubmit_Click(object sender, EventArgs e)
        {
            // TODO: API呼び出しを実装する
        }

        /// <summary>
        /// しりとり他人のコメントへの異議ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void ButtonClaim_Click(object sender, EventArgs e)
        {
            // TODO: API呼び出しを実装する
        }

        #endregion
    }
}
