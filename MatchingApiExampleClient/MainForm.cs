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
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Honememo.MatchingApiExample.Client.Properties;
    using Honememo.MatchingApiExample.Client.Services;
    using Honememo.MatchingApiExample.Protos;

    /// <summary>
    /// gRPC勉強用マッチングAPIサンプル主画面のクラスです。
    /// </summary>
    public partial class MainForm : Form
    {
        #region メンバー変数

        /// <summary>
        /// 主画面のビジネスロジックを扱うサービス。
        /// </summary>
        private readonly MainFormService service = new MainFormService();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 画面を生成する。
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.service.OnRoomsUpdated += this.UpdateRooms;
            this.service.OnRoomUpdated += this.UpdateRoom;
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
            // 普通ならプレイヤー名とレーティング値はサーバーから取るが、
            // マッチングサンプルアプリとしての利便性のために、ランダムに初期値を入れる。
            this.textBoxUrl.Text = Settings.Default.Url;
            this.textBoxPlayerName.Text = this.service.GetRandomName();
            this.textBoxRating.Text = this.service.GenerateRandomRating().ToString();
        }

        /// <summary>
        /// 画面クローズ時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 普通ならここでSettingsも保存するが、サンプルクライアントでは毎回別プレイヤーにするため保存しない。
            // Settings.Default.Save();
            this.service.Dispose();
        }

        #endregion

        #region 環境設定グループの各イベントのメソッド

        /// <summary>
        /// 接続先URLテキストボックスバリデーション。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TextBoxUrl_Validating(object sender, CancelEventArgs e)
        {
            this.RequireTextBox_Validating(sender, e);
            if (e.Cancel)
            {
                return;
            }

            var box = (TextBox)sender;
            try
            {
                new Uri(box.Text);
            }
            catch (UriFormatException)
            {
                this.errorProvider.SetError(box, Resources.WarningMessageInvalidUri);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 接続ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ButtonConnect_Click(object sender, EventArgs e)
        {
            // サーバーに接続して、ログイン状態にする
            try
            {
                await this.SwitchFormAtConnected(async () =>
                {
                    // gRPC接続を確立する
                    this.service.Connect(new Uri(this.textBoxUrl.Text));

                    // プレイヤー登録orログインを実施
                    if (Settings.Default.PlayerId <= 0)
                    {
                        var (id, token) = await this.service.SignUp(this.textBoxPlayerName.Text, uint.Parse(this.textBoxRating.Text));
                        Settings.Default.PlayerId = id;
                        Settings.Default.Token = token;
                    }
                    else
                    {
                        var (name, rating) = await this.service.SignIn(Settings.Default.PlayerId, Settings.Default.Token);
                        this.textBoxPlayerName.Text = name;
                        this.textBoxRating.Text = rating.ToString();
                    }

                    // ルーム更新イベントを監視する
                    this.service.SubscribeRoomsUpdated();
                });
            }
            catch (Exception)
            {
                this.SwitchFormAtStartup(() => this.service.Disconnect());
                throw;
            }
        }

        #endregion

        #region プレイヤー情報グループの各イベントのメソッド

        /// <summary>
        /// レーティング値テキストボックスバリデーション。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TextBoxRating_Validating(object sender, CancelEventArgs e)
        {
            this.RequireTextBox_Validating(sender, e);
            if (e.Cancel)
            {
                return;
            }

            var box = (TextBox)sender;
            if (!int.TryParse(box.Text, out int rating) || rating < 0)
            {
                this.errorProvider.SetError(box, string.Format(Resources.WarningMessageGreaterThan, 0));
                e.Cancel = true;
            }
        }

        /// <summary>
        /// プレイヤー変更ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ButtonChangeMe_Click(object sender, EventArgs e)
        {
            await this.service.ChangeMe(this.textBoxPlayerName.Text, uint.Parse(this.textBoxRating.Text));
        }

        #endregion

        #region マッチング系グループの各イベントのメソッド

        /// <summary>
        /// 部屋の最大人数テキストボックスバリデーション。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TextBoxRoomSize_Validating(object sender, CancelEventArgs e)
        {
            this.RequireTextBox_Validating(sender, e);
            if (e.Cancel)
            {
                return;
            }

            var box = (TextBox)sender;
            if (!int.TryParse(box.Text, out int size) || size < 2)
            {
                this.errorProvider.SetError(box, string.Format(Resources.WarningMessageGreaterThan, 2));
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 部屋作成ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ButtonCreateRoom_Click(object sender, EventArgs e)
        {
            try
            {
                await this.SwitchFormAtJoined(async () =>
                {
                    var no = await this.service.CreateRoom(uint.Parse(this.textBoxRoomSize.Text));
                    this.service.SubscribeRoomUpdated();
                    this.textBoxRoomNo.Text = no.ToString();
                });
            }
            catch (Exception)
            {
                this.SwitchFormAtConnected();
                throw;
            }
        }

        /// <summary>
        /// 部屋探索ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private async void ButtonMatch_Click(object sender, EventArgs e)
        {
            try
            {
                await this.SwitchFormAtJoined(async () =>
                {
                    var no = await this.service.MatchRoom();
                    this.service.SubscribeRoomUpdated();
                    this.textBoxRoomNo.Text = no.ToString();
                });
            }
            catch (Exception)
            {
                this.SwitchFormAtConnected();
                throw;
            }
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
            try
            {
                await this.SwitchFormAtConnected(async () =>
                {
                    await this.service.LeaveRoom();
                });
            }
            catch (Exception)
            {
                this.SwitchFormAtJoined();
                throw;
            }
        }

        /// <summary>
        /// しりとりゲーム開始ボタンクリック時のイベント処理。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void ButtonShiritori_Click(object sender, EventArgs e)
        {
            using (var form = new ShiritoriForm(this.service.Channel))
            {
                form.ShowDialog();
            }
        }

        #endregion

        #region フォーム状態更新メソッド

        /// <summary>
        /// フォームの表示をアプリ起動時の状態に切り替える。
        /// </summary>
        /// <param name="action">切り替え中に処理を行う場合その処理。</param>
        private void SwitchFormAtStartup(Action action = null)
        {
            this.groupBoxGame.Enabled = false;
            this.groupBoxList.Enabled = false;
            this.groupBoxMatch.Enabled = false;
            this.groupBoxCreateRoom.Enabled = false;
            this.buttonChangeMe.Enabled = false;

            action?.Invoke();

            this.groupBoxConfig.Enabled = true;
            this.groupBoxPlayer.Enabled = true;
        }

        /// <summary>
        /// フォームの表示をサーバー接続時の状態に切り替える。
        /// </summary>
        /// <param name="func">切り替え中に行う処理。</param>
        private async Task SwitchFormAtConnected(Func<Task> func)
        {
            this.groupBoxGame.Enabled = false;
            this.groupBoxConfig.Enabled = false;

            if (func != null)
            {
                await func();
            }

            this.groupBoxPlayer.Enabled = true;
            this.buttonChangeMe.Enabled = true;
            this.groupBoxCreateRoom.Enabled = true;
            this.groupBoxMatch.Enabled = true;
            this.groupBoxList.Enabled = true;
        }

        /// <summary>
        /// フォームの表示をサーバー接続時の状態に切り替える。
        /// </summary>
        /// <param name="action">切り替え中に処理を行う場合その処理。</param>
        private async void SwitchFormAtConnected(Action action = null)
        {
            await this.SwitchFormAtConnected(async () => action?.Invoke());
        }

        /// <summary>
        /// フォームの表示をルーム入室時の状態に切り替える。
        /// </summary>
        /// <param name="func">切り替え中に行う処理。</param>
        private async Task SwitchFormAtJoined(Func<Task> func)
        {
            this.groupBoxConfig.Enabled = false;
            this.groupBoxPlayer.Enabled = false;
            this.groupBoxCreateRoom.Enabled = false;
            this.groupBoxMatch.Enabled = false;
            this.groupBoxList.Enabled = false;

            if (func != null)
            {
                await func();
            }

            this.groupBoxGame.Enabled = true;
        }

        /// <summary>
        /// フォームの表示をルーム入室時の状態に切り替える。
        /// </summary>
        /// <param name="action">切り替え中に処理を行う場合その処理。</param>
        private async void SwitchFormAtJoined(Action action = null)
        {
            await this.SwitchFormAtJoined(async () => action?.Invoke());
        }

        #endregion

        #region ゲームイベント用メソッド

        /// <summary>
        /// ルーム一覧を再描画する。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void UpdateRooms(object sender, FindRoomsReply e)
        {
            this.listViewRoomList.Items.Clear();
            foreach (var room in e.Rooms)
            {
                // TODO: クリックイベントを設定する
                this.listViewRoomList.Items.Add(new ListViewItem(new string[] { room.No.ToString(), $"{room.Players}/{room.MaxPlayers}", room.Rating.ToString() }));
            }
        }

        /// <summary>
        /// 入室中のルームの状態を再描画する。
        /// </summary>
        /// <param name="sender">イベント発生元インスタンス。</param>
        /// <param name="e">イベントパラメータ。</param>
        private void UpdateRoom(object sender, GetRoomReply e)
        {
            this.listViewMemberList.Items.Clear();
            foreach (var player in e.Players)
            {
                this.listViewMemberList.Items.Add(new ListViewItem(player.Name));
            }

            this.buttonShiritori.Enabled = e.Players.Count >= 2;
        }

        #endregion

        #region 汎用のバリデートメソッド

        /// <summary>
        /// 汎用のテキストボックス必須バリデーション。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void RequireTextBox_Validating(object sender, CancelEventArgs e)
        {
            var box = (TextBox)sender;
            box.Text = box.Text?.Trim();
            if (string.IsNullOrEmpty(box.Text))
            {
                this.errorProvider.SetError(box, Resources.WarningMessageRequire);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 汎用のエラープロバイダ初期化処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ResetErrorProvider_Validated(object sender, EventArgs e)
        {
            this.errorProvider.SetError((Control)sender, null);
        }

        #endregion
    }
}
