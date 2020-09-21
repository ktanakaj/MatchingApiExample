// ================================================================================================
// <summary>
//      gRPC勉強用マッチングAPIサンプル主画面デザインソース</summary>
//
// <copyright file="MainForm.Designer.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Client
{
    /// <summary>
    /// gRPC勉強用マッチングAPIサンプル主画面のクラスです。
    /// </summary>
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxConfig = new System.Windows.Forms.GroupBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.textBoxUrl = new System.Windows.Forms.TextBox();
            this.labelUrl = new System.Windows.Forms.Label();
            this.groupBoxPlayer = new System.Windows.Forms.GroupBox();
            this.buttonChangeMe = new System.Windows.Forms.Button();
            this.textBoxRating = new System.Windows.Forms.TextBox();
            this.labelPlayerRating = new System.Windows.Forms.Label();
            this.textBoxPlayerName = new System.Windows.Forms.TextBox();
            this.labelPlayerName = new System.Windows.Forms.Label();
            this.groupBoxCreateRoom = new System.Windows.Forms.GroupBox();
            this.buttonCreateRoom = new System.Windows.Forms.Button();
            this.textBoxRoomSize = new System.Windows.Forms.TextBox();
            this.labelRoomSize = new System.Windows.Forms.Label();
            this.groupBoxMatch = new System.Windows.Forms.GroupBox();
            this.buttonMatch = new System.Windows.Forms.Button();
            this.groupBoxList = new System.Windows.Forms.GroupBox();
            this.listViewRoomList = new System.Windows.Forms.ListView();
            this.columnHeaderRoomListNo = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderRoomListPlayers = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderRoomListRating = new System.Windows.Forms.ColumnHeader();
            this.groupBoxGame = new System.Windows.Forms.GroupBox();
            this.buttonShiritori = new System.Windows.Forms.Button();
            this.listViewMemberList = new System.Windows.Forms.ListView();
            this.labelMemberList = new System.Windows.Forms.Label();
            this.buttonLeaveRoom = new System.Windows.Forms.Button();
            this.textBoxRoomNo = new System.Windows.Forms.TextBox();
            this.labelRoomNo = new System.Windows.Forms.Label();
            this.groupBoxConfig.SuspendLayout();
            this.groupBoxPlayer.SuspendLayout();
            this.groupBoxCreateRoom.SuspendLayout();
            this.groupBoxMatch.SuspendLayout();
            this.groupBoxList.SuspendLayout();
            this.groupBoxGame.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxConfig
            // 
            this.groupBoxConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxConfig.Controls.Add(this.buttonConnect);
            this.groupBoxConfig.Controls.Add(this.textBoxUrl);
            this.groupBoxConfig.Controls.Add(this.labelUrl);
            this.groupBoxConfig.Location = new System.Drawing.Point(13, 12);
            this.groupBoxConfig.Name = "groupBoxConfig";
            this.groupBoxConfig.Size = new System.Drawing.Size(559, 61);
            this.groupBoxConfig.TabIndex = 0;
            this.groupBoxConfig.TabStop = false;
            this.groupBoxConfig.Text = "環境設定";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConnect.AutoSize = true;
            this.buttonConnect.Location = new System.Drawing.Point(508, 20);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(41, 25);
            this.buttonConnect.TabIndex = 2;
            this.buttonConnect.Text = "接続";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
            // 
            // textBoxUrl
            // 
            this.textBoxUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUrl.Location = new System.Drawing.Point(79, 22);
            this.textBoxUrl.MaxLength = 4096;
            this.textBoxUrl.Name = "textBoxUrl";
            this.textBoxUrl.Size = new System.Drawing.Size(423, 23);
            this.textBoxUrl.TabIndex = 1;
            // 
            // labelUrl
            // 
            this.labelUrl.AutoSize = true;
            this.labelUrl.Location = new System.Drawing.Point(6, 25);
            this.labelUrl.Name = "labelUrl";
            this.labelUrl.Size = new System.Drawing.Size(67, 15);
            this.labelUrl.TabIndex = 0;
            this.labelUrl.Text = "接続先URL:";
            // 
            // groupBoxPlayer
            // 
            this.groupBoxPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPlayer.Controls.Add(this.buttonChangeMe);
            this.groupBoxPlayer.Controls.Add(this.textBoxRating);
            this.groupBoxPlayer.Controls.Add(this.labelPlayerRating);
            this.groupBoxPlayer.Controls.Add(this.textBoxPlayerName);
            this.groupBoxPlayer.Controls.Add(this.labelPlayerName);
            this.groupBoxPlayer.Location = new System.Drawing.Point(13, 82);
            this.groupBoxPlayer.Name = "groupBoxPlayer";
            this.groupBoxPlayer.Size = new System.Drawing.Size(559, 61);
            this.groupBoxPlayer.TabIndex = 1;
            this.groupBoxPlayer.TabStop = false;
            this.groupBoxPlayer.Text = "プレイヤー情報";
            // 
            // buttonChangeMe
            // 
            this.buttonChangeMe.AutoSize = true;
            this.buttonChangeMe.Enabled = false;
            this.buttonChangeMe.Location = new System.Drawing.Point(461, 20);
            this.buttonChangeMe.Name = "buttonChangeMe";
            this.buttonChangeMe.Size = new System.Drawing.Size(41, 25);
            this.buttonChangeMe.TabIndex = 4;
            this.buttonChangeMe.Text = "変更";
            this.buttonChangeMe.UseVisualStyleBackColor = true;
            this.buttonChangeMe.Click += new System.EventHandler(this.ButtonChangeMe_Click);
            // 
            // textBoxRating
            // 
            this.textBoxRating.Location = new System.Drawing.Point(376, 22);
            this.textBoxRating.MaxLength = 5;
            this.textBoxRating.Name = "textBoxRating";
            this.textBoxRating.Size = new System.Drawing.Size(49, 23);
            this.textBoxRating.TabIndex = 3;
            // 
            // labelPlayerRating
            // 
            this.labelPlayerRating.AutoSize = true;
            this.labelPlayerRating.Location = new System.Drawing.Point(309, 25);
            this.labelPlayerRating.Name = "labelPlayerRating";
            this.labelPlayerRating.Size = new System.Drawing.Size(61, 15);
            this.labelPlayerRating.TabIndex = 2;
            this.labelPlayerRating.Text = "レーティング:";
            // 
            // textBoxPlayerName
            // 
            this.textBoxPlayerName.Location = new System.Drawing.Point(79, 22);
            this.textBoxPlayerName.MaxLength = 32;
            this.textBoxPlayerName.Name = "textBoxPlayerName";
            this.textBoxPlayerName.Size = new System.Drawing.Size(205, 23);
            this.textBoxPlayerName.TabIndex = 1;
            // 
            // labelPlayerName
            // 
            this.labelPlayerName.AutoSize = true;
            this.labelPlayerName.Location = new System.Drawing.Point(6, 25);
            this.labelPlayerName.Name = "labelPlayerName";
            this.labelPlayerName.Size = new System.Drawing.Size(67, 15);
            this.labelPlayerName.TabIndex = 0;
            this.labelPlayerName.Text = "プレイヤー名:";
            // 
            // groupBoxCreateRoom
            // 
            this.groupBoxCreateRoom.Controls.Add(this.buttonCreateRoom);
            this.groupBoxCreateRoom.Controls.Add(this.textBoxRoomSize);
            this.groupBoxCreateRoom.Controls.Add(this.labelRoomSize);
            this.groupBoxCreateRoom.Enabled = false;
            this.groupBoxCreateRoom.Location = new System.Drawing.Point(13, 155);
            this.groupBoxCreateRoom.Name = "groupBoxCreateRoom";
            this.groupBoxCreateRoom.Size = new System.Drawing.Size(180, 61);
            this.groupBoxCreateRoom.TabIndex = 2;
            this.groupBoxCreateRoom.TabStop = false;
            this.groupBoxCreateRoom.Text = "部屋を作る";
            // 
            // buttonCreateRoom
            // 
            this.buttonCreateRoom.AutoSize = true;
            this.buttonCreateRoom.Location = new System.Drawing.Point(104, 20);
            this.buttonCreateRoom.Name = "buttonCreateRoom";
            this.buttonCreateRoom.Size = new System.Drawing.Size(41, 25);
            this.buttonCreateRoom.TabIndex = 2;
            this.buttonCreateRoom.Text = "作成";
            this.buttonCreateRoom.UseVisualStyleBackColor = true;
            this.buttonCreateRoom.Click += new System.EventHandler(this.ButtonCreateRoom_Click);
            // 
            // textBoxRoomSize
            // 
            this.textBoxRoomSize.Location = new System.Drawing.Point(46, 22);
            this.textBoxRoomSize.MaxLength = 2;
            this.textBoxRoomSize.Name = "textBoxRoomSize";
            this.textBoxRoomSize.Size = new System.Drawing.Size(52, 23);
            this.textBoxRoomSize.TabIndex = 1;
            this.textBoxRoomSize.Text = "2";
            // 
            // labelRoomSize
            // 
            this.labelRoomSize.AutoSize = true;
            this.labelRoomSize.Location = new System.Drawing.Point(6, 25);
            this.labelRoomSize.Name = "labelRoomSize";
            this.labelRoomSize.Size = new System.Drawing.Size(34, 15);
            this.labelRoomSize.TabIndex = 0;
            this.labelRoomSize.Text = "人数:";
            // 
            // groupBoxMatch
            // 
            this.groupBoxMatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMatch.Controls.Add(this.buttonMatch);
            this.groupBoxMatch.Enabled = false;
            this.groupBoxMatch.Location = new System.Drawing.Point(217, 155);
            this.groupBoxMatch.Name = "groupBoxMatch";
            this.groupBoxMatch.Size = new System.Drawing.Size(149, 61);
            this.groupBoxMatch.TabIndex = 3;
            this.groupBoxMatch.TabStop = false;
            this.groupBoxMatch.Text = "部屋を探す";
            // 
            // buttonMatch
            // 
            this.buttonMatch.AutoSize = true;
            this.buttonMatch.Location = new System.Drawing.Point(16, 20);
            this.buttonMatch.Name = "buttonMatch";
            this.buttonMatch.Size = new System.Drawing.Size(41, 25);
            this.buttonMatch.TabIndex = 0;
            this.buttonMatch.Text = "探索";
            this.buttonMatch.UseVisualStyleBackColor = true;
            this.buttonMatch.Click += new System.EventHandler(this.ButtonMatch_Click);
            // 
            // groupBoxList
            // 
            this.groupBoxList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxList.Controls.Add(this.listViewRoomList);
            this.groupBoxList.Enabled = false;
            this.groupBoxList.Location = new System.Drawing.Point(389, 155);
            this.groupBoxList.Name = "groupBoxList";
            this.groupBoxList.Size = new System.Drawing.Size(183, 274);
            this.groupBoxList.TabIndex = 4;
            this.groupBoxList.TabStop = false;
            this.groupBoxList.Text = "部屋一覧";
            // 
            // listViewRoomList
            // 
            this.listViewRoomList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewRoomList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderRoomListNo,
            this.columnHeaderRoomListPlayers,
            this.columnHeaderRoomListRating});
            this.listViewRoomList.HideSelection = false;
            this.listViewRoomList.Location = new System.Drawing.Point(11, 22);
            this.listViewRoomList.Name = "listViewRoomList";
            this.listViewRoomList.Size = new System.Drawing.Size(157, 235);
            this.listViewRoomList.TabIndex = 0;
            this.listViewRoomList.UseCompatibleStateImageBehavior = false;
            this.listViewRoomList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderRoomListNo
            // 
            this.columnHeaderRoomListNo.Text = "番号";
            this.columnHeaderRoomListNo.Width = 40;
            // 
            // columnHeaderRoomListPlayers
            // 
            this.columnHeaderRoomListPlayers.Text = "人数";
            this.columnHeaderRoomListPlayers.Width = 40;
            // 
            // columnHeaderRoomListRating
            // 
            this.columnHeaderRoomListRating.Text = "レーティング";
            this.columnHeaderRoomListRating.Width = 70;
            // 
            // groupBoxGame
            // 
            this.groupBoxGame.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGame.Controls.Add(this.buttonShiritori);
            this.groupBoxGame.Controls.Add(this.listViewMemberList);
            this.groupBoxGame.Controls.Add(this.labelMemberList);
            this.groupBoxGame.Controls.Add(this.buttonLeaveRoom);
            this.groupBoxGame.Controls.Add(this.textBoxRoomNo);
            this.groupBoxGame.Controls.Add(this.labelRoomNo);
            this.groupBoxGame.Enabled = false;
            this.groupBoxGame.Location = new System.Drawing.Point(13, 233);
            this.groupBoxGame.Name = "groupBoxGame";
            this.groupBoxGame.Size = new System.Drawing.Size(353, 196);
            this.groupBoxGame.TabIndex = 5;
            this.groupBoxGame.TabStop = false;
            this.groupBoxGame.Text = "ゲームプレイ";
            // 
            // buttonShiritori
            // 
            this.buttonShiritori.AutoSize = true;
            this.buttonShiritori.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.buttonShiritori.Location = new System.Drawing.Point(173, 90);
            this.buttonShiritori.Name = "buttonShiritori";
            this.buttonShiritori.Size = new System.Drawing.Size(130, 47);
            this.buttonShiritori.TabIndex = 0;
            this.buttonShiritori.Text = "しりとりスタート";
            this.buttonShiritori.UseVisualStyleBackColor = true;
            this.buttonShiritori.Click += new System.EventHandler(this.ButtonShiritori_Click);
            // 
            // listViewMemberList
            // 
            this.listViewMemberList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewMemberList.HideSelection = false;
            this.listViewMemberList.Location = new System.Drawing.Point(14, 78);
            this.listViewMemberList.Name = "listViewMemberList";
            this.listViewMemberList.Size = new System.Drawing.Size(84, 101);
            this.listViewMemberList.TabIndex = 5;
            this.listViewMemberList.UseCompatibleStateImageBehavior = false;
            this.listViewMemberList.View = System.Windows.Forms.View.List;
            // 
            // labelMemberList
            // 
            this.labelMemberList.AutoSize = true;
            this.labelMemberList.Location = new System.Drawing.Point(6, 55);
            this.labelMemberList.Name = "labelMemberList";
            this.labelMemberList.Size = new System.Drawing.Size(46, 15);
            this.labelMemberList.TabIndex = 4;
            this.labelMemberList.Text = "参加者:";
            // 
            // buttonLeaveRoom
            // 
            this.buttonLeaveRoom.AutoSize = true;
            this.buttonLeaveRoom.Location = new System.Drawing.Point(132, 21);
            this.buttonLeaveRoom.Name = "buttonLeaveRoom";
            this.buttonLeaveRoom.Size = new System.Drawing.Size(41, 25);
            this.buttonLeaveRoom.TabIndex = 3;
            this.buttonLeaveRoom.Text = "退室";
            this.buttonLeaveRoom.UseVisualStyleBackColor = true;
            this.buttonLeaveRoom.Click += new System.EventHandler(this.ButtonLeaveRoom_Click);
            // 
            // textBoxRoomNo
            // 
            this.textBoxRoomNo.Location = new System.Drawing.Point(70, 22);
            this.textBoxRoomNo.MaxLength = 10;
            this.textBoxRoomNo.Name = "textBoxRoomNo";
            this.textBoxRoomNo.ReadOnly = true;
            this.textBoxRoomNo.Size = new System.Drawing.Size(56, 23);
            this.textBoxRoomNo.TabIndex = 2;
            // 
            // labelRoomNo
            // 
            this.labelRoomNo.AutoSize = true;
            this.labelRoomNo.Location = new System.Drawing.Point(6, 25);
            this.labelRoomNo.Name = "labelRoomNo";
            this.labelRoomNo.Size = new System.Drawing.Size(58, 15);
            this.labelRoomNo.TabIndex = 1;
            this.labelRoomNo.Text = "部屋番号:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 441);
            this.Controls.Add(this.groupBoxGame);
            this.Controls.Add(this.groupBoxList);
            this.Controls.Add(this.groupBoxMatch);
            this.Controls.Add(this.groupBoxCreateRoom);
            this.Controls.Add(this.groupBoxPlayer);
            this.Controls.Add(this.groupBoxConfig);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(600, 600);
            this.MinimumSize = new System.Drawing.Size(600, 480);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "しりとり対戦アプリ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxConfig.ResumeLayout(false);
            this.groupBoxConfig.PerformLayout();
            this.groupBoxPlayer.ResumeLayout(false);
            this.groupBoxPlayer.PerformLayout();
            this.groupBoxCreateRoom.ResumeLayout(false);
            this.groupBoxCreateRoom.PerformLayout();
            this.groupBoxMatch.ResumeLayout(false);
            this.groupBoxMatch.PerformLayout();
            this.groupBoxList.ResumeLayout(false);
            this.groupBoxGame.ResumeLayout(false);
            this.groupBoxGame.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxConfig;
        private System.Windows.Forms.TextBox textBoxUrl;
        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.GroupBox groupBoxPlayer;
        private System.Windows.Forms.TextBox textBoxPlayerName;
        private System.Windows.Forms.Label labelPlayerName;
        private System.Windows.Forms.TextBox textBoxRating;
        private System.Windows.Forms.Label labelPlayerRating;
        private System.Windows.Forms.GroupBox groupBoxCreateRoom;
        private System.Windows.Forms.Button buttonCreateRoom;
        private System.Windows.Forms.TextBox textBoxRoomSize;
        private System.Windows.Forms.Label labelRoomSize;
        private System.Windows.Forms.GroupBox groupBoxMatch;
        private System.Windows.Forms.Button buttonMatch;
        private System.Windows.Forms.GroupBox groupBoxList;
        private System.Windows.Forms.ListView listViewRoomList;
        private System.Windows.Forms.GroupBox groupBoxGame;
        private System.Windows.Forms.Button buttonLeaveRoom;
        private System.Windows.Forms.TextBox textBoxRoomNo;
        private System.Windows.Forms.Label labelRoomNo;
        private System.Windows.Forms.ListView listViewMemberList;
        private System.Windows.Forms.Label labelMemberList;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonChangeMe;
        private System.Windows.Forms.ColumnHeader columnHeaderRoomListNo;
        private System.Windows.Forms.ColumnHeader columnHeaderRoomListRating;
        private System.Windows.Forms.ColumnHeader columnHeaderRoomListPlayers;
        private System.Windows.Forms.Button buttonShiritori;
    }
}

