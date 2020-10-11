// ================================================================================================
// <summary>
//      主画面サービスクラスソース</summary>
//
// <copyright file="MainFormService.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Client.Services
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Honememo.MatchingApiExample.Client.Properties;
    using Honememo.MatchingApiExample.Protos;

    /// <summary>
    /// 主画面のサービスを受け持つクラスです。
    /// </summary>
    /// <remarks>MainForm肥大化に伴いGUIの古典的MVCに置けるMを切り出したクラス。</remarks>
    public class MainFormService : IDisposable
    {
        #region メンバー変数

        /// <summary>
        /// 乱数ジェネレータ。
        /// </summary>
        private readonly Random rand = new Random();

        /// <summary>
        /// プレイヤー関連サービスのクライアント。
        /// </summary>
        private Player.PlayerClient playerService;

        /// <summary>
        /// マッチングサービスのクライアント。
        /// </summary>
        private Matching.MatchingClient matchingService;

        /// <summary>
        /// ルーム一覧更新通知用のキャンセルトークンソース。
        /// </summary>
        private CancellationTokenSource roomsUpdatedSource;

        /// <summary>
        /// ルーム状態更新通知用のキャンセルトークンソース。
        /// </summary>
        private CancellationTokenSource roomUpdatedSource;

        #endregion

        #region イベント

        /// <summary>
        /// ルーム一覧更新イベント。
        /// </summary>
        public event EventHandler<FindRoomsReply> OnRoomsUpdated;

        /// <summary>
        /// ルーム状態更新イベント。
        /// </summary>
        public event EventHandler<GetRoomReply> OnRoomUpdated;

        #endregion

        #region プロパティ

        /// <summary>
        /// gRPCチャネル。
        /// </summary>
        public GrpcChannel Channel { get; private set; }

        #endregion

        #region システム系メソッド

        /// <summary>
        /// gRPC接続を確立する。
        /// </summary>
        /// <param name="uri">接続先URL。</param>
        /// <remarks>このクラスの各メソッドは、基本的に事前にこのメソッドを実行して接続中にしておく必要がある。</remarks>
        public void Connect(Uri uri)
        {
            // gRPC接続を確立、gRPCサービスを初期化する
            this.Disconnect();
            var options = new GrpcChannelOptions();
            if (uri.Scheme == "http")
            {
                options.Credentials = ChannelCredentials.Insecure;
            }

            this.Channel = GrpcChannel.ForAddress(uri, options);
            this.playerService = new Player.PlayerClient(this.Channel);
            this.matchingService = new Matching.MatchingClient(this.Channel);
        }

        /// <summary>
        /// gRPC接続を切断する。
        /// </summary>
        public void Disconnect()
        {
            // 通知受信中の場合は解除する
            this.UnsubscribeRoomUpdatedSource();
            this.UnsubscribeRoomsUpdatedSource();

            // 部屋に入っている場合は抜ける
            if (this.matchingService != null)
            {
                try
                {
                    this.matchingService.LeaveRoom(new Empty());
                }
                catch (Exception e)
                {
                    // ここでエラーになってもどうしようもないのでデバッグログだけ。
                    Debug.WriteLine(e);
                }
            }

            // 接続済みのサービス等を解放する
            this.matchingService = null;
            this.playerService = null;

            if (this.Channel != null)
            {
                this.Channel.Dispose();
                this.Channel = null;
            }
        }

        /// <summary>
        /// サービスを解放する。
        /// </summary>
        public void Dispose()
        {
            // 現状はDisconnectのエイリアス
            this.Disconnect();
        }

        #endregion

        #region プレイヤー系メソッド

        /// <summary>
        /// プレイヤーを登録する。
        /// </summary>
        /// <param name="name">プレイヤー名。</param>
        /// <param name="rating">初期レーティング値。※デバッグ用</param>
        /// <returns>プレイヤーID, 端末トークン。</returns>
        public async Task<(int, string)> SignUp(string name, uint rating = 0)
        {
            var token = this.GenerateToken();
            var res = await this.playerService.SignUpAsync(new SignUpRequest { Token = token });
            await this.ChangeMe(name, rating);
            return (res.Id, token);
        }

        /// <summary>
        /// プレイヤーを認証する。
        /// </summary>
        /// <param name="id">プレイヤーID。</param>
        /// <param name="token">端末トークン。</param>
        /// <returns>プレイヤー名, レーティング値。</returns>
        public async Task<(string, uint)> SignIn(int id, string token)
        {
            await this.playerService.SignInAsync(new SignInRequest { Id = id, Token = token });
            var res = await this.playerService.FindMeAsync(new Empty());
            return (res.Name, res.Rating);
        }

        /// <summary>
        /// プレイヤー情報を変更する。
        /// </summary>
        /// <param name="name">プレイヤー名。</param>
        /// <param name="rating">初期レーティング値。※デバッグ用</param>
        /// <returns>処理状態。</returns>
        public async Task ChangeMe(string name, uint rating = 0)
        {
            await this.playerService.ChangeMeAsync(new ChangeMeRequest { Name = name, Rating = rating });
        }

        #endregion

        #region マッチング系メソッド

        /// <summary>
        /// 部屋を作成する。
        /// </summary>
        /// <param name="maxPlayers">部屋の最大人数。</param>
        /// <returns>作成した部屋の番号。</returns>
        public async Task<uint> CreateRoom(uint maxPlayers)
        {
            var res = await this.matchingService.CreateRoomAsync(new CreateRoomRequest { MaxPlayers = maxPlayers });
            return res.No;
        }

        /// <summary>
        /// 部屋をマッチングする。
        /// </summary>
        /// <returns>入室した部屋の番号。</returns>
        public async Task<uint> MatchRoom()
        {
            var res = await this.matchingService.MatchRoomAsync(new Empty());
            return res.No;
        }

        /// <summary>
        /// 部屋を出る。
        /// </summary>
        /// <returns>処理状態。</returns>
        public async Task LeaveRoom()
        {
            this.UnsubscribeRoomUpdatedSource();
            await this.matchingService.LeaveRoomAsync(new Empty());
        }

        #endregion

        #region 状態通知系メソッド

        /// <summary>
        /// ルーム一覧更新の通知を開始する。
        /// </summary>
        public async void SubscribeRoomsUpdated()
        {
            this.UnsubscribeRoomsUpdatedSource();
            this.roomsUpdatedSource = new CancellationTokenSource();
            using var call = this.matchingService.WatchRooms(new Empty());
            try
            {
                await foreach (var reply in call.ResponseStream.ReadAllAsync(this.roomsUpdatedSource.Token))
                {
                    this.OnRoomsUpdated?.Invoke(this, reply);
                }
            }
            catch (RpcException e)
            {
                // 空のイベントを投げておく
                this.OnRoomsUpdated?.Invoke(this, new FindRoomsReply());
                if (e.StatusCode != StatusCode.Cancelled)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 入室中のルームの状態更新の通知を開始する。
        /// </summary>
        public async void SubscribeRoomUpdated()
        {
            this.UnsubscribeRoomUpdatedSource();
            this.roomUpdatedSource = new CancellationTokenSource();
            using var call = this.matchingService.WatchRoom(new Empty());
            try
            {
                await foreach (var reply in call.ResponseStream.ReadAllAsync(this.roomUpdatedSource.Token))
                {
                    this.OnRoomUpdated?.Invoke(this, reply);
                }
            }
            catch (RpcException e)
            {
                // 空のイベントを投げておく
                this.OnRoomUpdated?.Invoke(this, new GetRoomReply());
                if (e.StatusCode != StatusCode.Cancelled)
                {
                    throw;
                }
            }
        }

        #endregion

        #region その他メソッド

        /// <summary>
        /// ランダムな名前を取得する。
        /// </summary>
        /// <returns>取得した名前。</returns>
        /// <remarks>マッチング動作検証用。</remarks>
        public string GetRandomName()
        {
            var names = Settings.Default.CandidateNames;
            return names[this.rand.Next(0, names.Count)];
        }

        /// <summary>
        /// ランダムなレーティング値を生成する。
        /// </summary>
        /// <returns>生成したレーティング値。</returns>
        /// <remarks>マッチング動作検証用。</remarks>
        public int GenerateRandomRating()
        {
            return this.rand.Next(1200, 1800);
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// 一意なトークンを生成する。
        /// </summary>
        /// <returns>生成したトークン文字列。</returns>
        private string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// ルーム一覧更新の通知を解除する。
        /// </summary>
        private void UnsubscribeRoomsUpdatedSource()
        {
            if (this.roomsUpdatedSource != null)
            {
                this.roomsUpdatedSource.Cancel();
                this.roomsUpdatedSource = null;
            }
        }

        /// <summary>
        /// 入室中のルームの状態更新の解除を開始する。
        /// </summary>
        private void UnsubscribeRoomUpdatedSource()
        {
            if (this.roomUpdatedSource != null)
            {
                this.roomUpdatedSource.Cancel();
                this.roomUpdatedSource = null;
            }
        }

        #endregion
    }
}
