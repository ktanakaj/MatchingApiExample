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
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Honememo.MatchingApiExample.Client.Properties;
    using Honememo.MatchingApiExample.Protos;

    /// <summary>
    /// gRPC勉強用マッチングAPIサンプル主画面のクラスです。
    /// </summary>
    /// <remarks>TODO: 全体的に動けばいいやの仮実装。</remarks>
    public partial class MainForm : Form
    {
        #region メンバー変数

        /// <summary>
        /// 乱数ジェネレータ。
        /// </summary>
        private readonly Random rand = new Random();

        /// <summary>
        /// gRPCチャネル。
        /// </summary>
        private GrpcChannel channel;

        /// <summary>
        /// プレイヤー関連サービスのクライアント。
        /// </summary>
        private Player.PlayerClient playerService;

        /// <summary>
        /// マッチングサービスのクライアント。
        /// </summary>
        private Matching.MatchingClient matchingService;

        /// <summary>
        /// ゲームサービスのクライアント。
        /// </summary>
        private Game.GameClient gameService;

        /// <summary>
        /// RoomsUpdated用のキャンセルトークンソース。
        /// </summary>
        private CancellationTokenSource roomsUpdatedSource;

        /// <summary>
        /// RoomUpdated用のキャンセルトークンソース。
        /// </summary>
        private CancellationTokenSource roomUpdatedSource;

        #endregion

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

        #region フォームの各イベントのメソッド

        /// <summary>
        /// 画面ロード時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.textBoxUrl.Text = Settings.Default.Url;

            // 端末トークンを発行していない場合は最初に発行
            if (string.IsNullOrWhiteSpace(Settings.Default.Token))
            {
                Settings.Default.Token = Guid.NewGuid().ToString();
            }

            // 普通ならプレイヤー名とレーティング値はサーバーから取るが、
            // マッチングサンプルアプリとしての利便性のために、ランダムに初期値を入れる。
            this.textBoxPlayerName.Text = this.GetRandomName();
            this.textBoxRating.Text = this.GenerateRandomRating().ToString();
        }

        /// <summary>
        /// 画面クローズ時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 普通ならここでSettingsも保存するが、サンプルクライアントでは毎回別プレイヤーにするため保存しない。
            // 一応gRPC接続を解放する。
            try
            {
                this.DisconnectServer();
            }
            catch (Exception ex)
            {
                this.ErrorDialog(ex.ToString());
            }
        }

        #endregion

        #region 環境設定グループの各イベントのメソッド

        /// <summary>
        /// 接続ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ButtonConnect_Click(object sender, EventArgs e)
        {
            await this.ConnectServer();
        }

        #endregion

        #region プレイヤー情報グループの各イベントのメソッド

        /// <summary>
        /// プレイヤー変更ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ButtonChangeMe_Click(object sender, EventArgs e)
        {
            await this.playerService.ChangeMeAsync(new ChangeMeRequest { Name = this.textBoxPlayerName.Text, Rating = uint.Parse(this.textBoxRating.Text) });
        }

        #endregion

        #region マッチング系グループの各イベントのメソッド

        /// <summary>
        /// 部屋作成ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ButtonCreateRoom_Click(object sender, EventArgs e)
        {
            // TODO: もしエラーになったらボタン等を戻すようにする
            this.groupBoxPlayer.Enabled = false;
            this.groupBoxCreateRoom.Enabled = false;
            this.groupBoxMatch.Enabled = false;
            this.groupBoxList.Enabled = false;
            var res = await this.matchingService.CreateRoomAsync(new CreateRoomRequest { MaxPlayers = uint.Parse(this.textBoxRoomSize.Text) });
            this.MonitorRoomUpdated();
            this.textBoxRoomNo.Text = res.No.ToString();
            this.groupBoxGame.Enabled = true;
        }

        /// <summary>
        /// 部屋探索ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ButtonMatch_Click(object sender, EventArgs e)
        {
            // TODO: もしエラーになったらボタン等を戻すようにする
            this.groupBoxPlayer.Enabled = false;
            this.groupBoxCreateRoom.Enabled = false;
            this.groupBoxMatch.Enabled = false;
            this.groupBoxList.Enabled = false;
            var res = await this.matchingService.MatchRoomAsync(new Empty());
            this.MonitorRoomUpdated();
            this.textBoxRoomNo.Text = res.No.ToString();
            this.groupBoxGame.Enabled = true;
        }

        #endregion

        #region ゲームグループの各イベントのメソッド

        /// <summary>
        /// 部屋退室ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ButtonLeaveRoom_Click(object sender, EventArgs e)
        {
            // TODO: もしエラーになったらボタン等を戻すようにする
            this.textBoxRoomNo.Text = string.Empty;
            this.groupBoxGame.Enabled = false;
            if (this.roomUpdatedSource != null)
            {
                this.roomUpdatedSource.Cancel();
                this.roomUpdatedSource = null;
            }

            await this.matchingService.LeaveRoomAsync(new Empty());
            this.groupBoxCreateRoom.Enabled = true;
            this.groupBoxMatch.Enabled = true;
            this.groupBoxList.Enabled = true;
            this.groupBoxPlayer.Enabled = true;
        }

        /// <summary>
        /// しりとりゲーム開始ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void ButtonShiritori_Click(object sender, EventArgs e)
        {
            using (var form = new ShiritoriForm())
            {
                form.ShowDialog();
            }
        }

        #endregion

        #region フォーム部品メソッド

        /// <summary>
        /// 単純デザインのエラーダイアログ。
        /// </summary>
        /// <param name="msg">メッセージ。</param>
        private void ErrorDialog(string msg)
        {
            MessageBox.Show(msg, Resources.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region その他のメソッド

        /// <summary>
        /// gRPC接続を確立する。
        /// </summary>
        /// <returns>処理状態。</returns>
        private async Task ConnectServer()
        {
            try
            {
                // gRPC接続時に使う設定欄を変更不可する
                this.groupBoxConfig.Enabled = false;

                // gRPC接続を確立する
                var uri = new Uri(this.textBoxUrl.Text);
                var options = new GrpcChannelOptions();
                if (uri.Scheme == "http")
                {
                    options.Credentials = ChannelCredentials.Insecure;
                }

                this.channel = GrpcChannel.ForAddress(uri, options);
                this.playerService = new Player.PlayerClient(this.channel);
                this.matchingService = new Matching.MatchingClient(this.channel);
                this.gameService = new Game.GameClient(this.channel);

                // プレイヤー登録orログインを実施
                if (Settings.Default.PlayerId <= 0)
                {
                    await this.playerService.SignUpAsync(new SignUpRequest { Token = Settings.Default.Token });
                    await this.playerService.ChangeMeAsync(new ChangeMeRequest { Name = this.textBoxPlayerName.Text, Rating = uint.Parse(this.textBoxRating.Text) });
                }
                else
                {
                    await this.playerService.SignInAsync(new SignInRequest { Id = Settings.Default.PlayerId, Token = Settings.Default.Token });
                    var res = await this.playerService.FindMeAsync(new Empty());
                    this.textBoxPlayerName.Text = res.Name;
                    this.textBoxRating.Text = res.Rating.ToString();
                }

                // ルーム更新イベントを監視する
                this.MonitorRoomsUpdated();

                this.buttonChangeMe.Enabled = true;
                this.groupBoxCreateRoom.Enabled = true;
                this.groupBoxMatch.Enabled = true;
                this.groupBoxList.Enabled = true;
            }
            catch (Exception e)
            {
                this.ErrorDialog(e.ToString());
                this.DisconnectServer();
            }
        }

        /// <summary>
        /// gRPC接続を切断する。
        /// </summary>
        private void DisconnectServer()
        {
            this.groupBoxGame.Enabled = false;
            this.groupBoxList.Enabled = false;
            this.groupBoxMatch.Enabled = false;
            this.groupBoxCreateRoom.Enabled = false;
            this.buttonChangeMe.Enabled = false;

            if (this.roomUpdatedSource != null)
            {
                this.roomUpdatedSource.Cancel();
                this.roomUpdatedSource = null;
            }

            if (this.roomsUpdatedSource != null)
            {
                this.roomsUpdatedSource.Cancel();
                this.roomsUpdatedSource = null;
            }

            if (this.matchingService != null)
            {
                this.matchingService.LeaveRoom(new Empty());
            }

            this.gameService = null;
            this.matchingService = null;
            this.playerService = null;

            if (this.channel != null)
            {
                this.channel.Dispose();
                this.channel = null;
            }

            this.groupBoxConfig.Enabled = true;
        }

        /// <summary>
        /// ルーム一覧更新イベントを監視する。
        /// </summary>
        private async void MonitorRoomsUpdated()
        {
            this.roomsUpdatedSource = new CancellationTokenSource();
            using var call = this.matchingService.FireRoomsUpdated(new Empty());
            try
            {
                await foreach (var reply in call.ResponseStream.ReadAllAsync(this.roomsUpdatedSource.Token))
                {
                    this.listViewRoomList.Items.Clear();
                    foreach (var room in reply.Rooms)
                    {
                        // TODO: クリックイベントを設定する
                        this.listViewRoomList.Items.Add(new ListViewItem(new string[] { room.No.ToString(), $"{room.Players}/{room.MaxPlayers}", room.Rating.ToString() }));
                    }
                }
            }
            catch (RpcException e)
            {
                this.listViewRoomList.Items.Clear();
                if (e.StatusCode != StatusCode.Cancelled)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// ルーム状態更新イベントを監視する。
        /// </summary>
        private async void MonitorRoomUpdated()
        {
            this.roomUpdatedSource = new CancellationTokenSource();
            using var call = this.gameService.FireRoomUpdated(new Empty());
            try
            {
                await foreach (var reply in call.ResponseStream.ReadAllAsync(this.roomUpdatedSource.Token))
                {
                    this.listViewMemberList.Items.Clear();
                    foreach (var player in reply.Players)
                    {
                        this.listViewMemberList.Items.Add(new ListViewItem(player.Name));
                    }
                }
            }
            catch (RpcException e)
            {
                this.listViewMemberList.Items.Clear();
                if (e.StatusCode != StatusCode.Cancelled)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// ランダムな名前を取得する。
        /// </summary>
        /// <returns>取得した名前。</returns>
        /// <remarks>マッチング動作検証用。</remarks>
        private string GetRandomName()
        {
            var names = Settings.Default.CandidateNames;
            return names[this.rand.Next(0, names.Count)];
        }

        /// <summary>
        /// ランダムなレーティング値を生成する。
        /// </summary>
        /// <returns>生成したレーティング値。</returns>
        /// <remarks>マッチング動作検証用。</remarks>
        private int GenerateRandomRating()
        {
            return this.rand.Next(1200, 1800);
        }

        #endregion
    }
}
