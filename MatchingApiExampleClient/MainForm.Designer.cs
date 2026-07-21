// ================================================================================================
// <summary>
//      gRPC勉強用マッチングAPIサンプル主画面デザインソース</summary>
//
// <copyright file="MainForm.Designer.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Client;

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
        components = new System.ComponentModel.Container();
        groupBoxConfig = new GroupBox();
        buttonConnect = new Button();
        textBoxUrl = new TextBox();
        labelUrl = new Label();
        groupBoxPlayer = new GroupBox();
        buttonChangeMe = new Button();
        textBoxRating = new TextBox();
        labelPlayerRating = new Label();
        textBoxPlayerName = new TextBox();
        labelPlayerName = new Label();
        groupBoxCreateRoom = new GroupBox();
        buttonCreateRoom = new Button();
        textBoxRoomSize = new TextBox();
        labelRoomSize = new Label();
        groupBoxMatch = new GroupBox();
        buttonMatch = new Button();
        groupBoxList = new GroupBox();
        listViewRoomList = new ListView();
        columnHeaderRoomListNo = new ColumnHeader();
        columnHeaderRoomListPlayers = new ColumnHeader();
        columnHeaderRoomListRating = new ColumnHeader();
        groupBoxGame = new GroupBox();
        buttonReactionGame = new Button();
        listViewMemberList = new ListView();
        labelMemberList = new Label();
        buttonLeaveRoom = new Button();
        textBoxRoomNo = new TextBox();
        labelRoomNo = new Label();
        errorProvider = new ErrorProvider(components);
        groupBoxConfig.SuspendLayout();
        groupBoxPlayer.SuspendLayout();
        groupBoxCreateRoom.SuspendLayout();
        groupBoxMatch.SuspendLayout();
        groupBoxList.SuspendLayout();
        groupBoxGame.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
        SuspendLayout();
        // 
        // groupBoxConfig
        // 
        groupBoxConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        groupBoxConfig.Controls.Add(buttonConnect);
        groupBoxConfig.Controls.Add(textBoxUrl);
        groupBoxConfig.Controls.Add(labelUrl);
        groupBoxConfig.Location = new Point(13, 12);
        groupBoxConfig.Name = "groupBoxConfig";
        groupBoxConfig.Size = new Size(559, 61);
        groupBoxConfig.TabIndex = 0;
        groupBoxConfig.TabStop = false;
        groupBoxConfig.Text = "環境設定";
        // 
        // buttonConnect
        // 
        buttonConnect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonConnect.AutoSize = true;
        buttonConnect.Location = new Point(508, 20);
        buttonConnect.Name = "buttonConnect";
        buttonConnect.Size = new Size(41, 25);
        buttonConnect.TabIndex = 2;
        buttonConnect.Text = "接続";
        buttonConnect.UseVisualStyleBackColor = true;
        buttonConnect.Click += ButtonConnect_Click;
        // 
        // textBoxUrl
        // 
        textBoxUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxUrl.Location = new Point(79, 22);
        textBoxUrl.MaxLength = 4096;
        textBoxUrl.Name = "textBoxUrl";
        textBoxUrl.Size = new Size(411, 23);
        textBoxUrl.TabIndex = 1;
        textBoxUrl.Validating += TextBoxUrl_Validating;
        textBoxUrl.Validated += ResetErrorProvider_Validated;
        // 
        // labelUrl
        // 
        labelUrl.AutoSize = true;
        labelUrl.Location = new Point(6, 25);
        labelUrl.Name = "labelUrl";
        labelUrl.Size = new Size(67, 15);
        labelUrl.TabIndex = 0;
        labelUrl.Text = "接続先URL:";
        // 
        // groupBoxPlayer
        // 
        groupBoxPlayer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        groupBoxPlayer.Controls.Add(buttonChangeMe);
        groupBoxPlayer.Controls.Add(textBoxRating);
        groupBoxPlayer.Controls.Add(labelPlayerRating);
        groupBoxPlayer.Controls.Add(textBoxPlayerName);
        groupBoxPlayer.Controls.Add(labelPlayerName);
        groupBoxPlayer.Location = new Point(13, 82);
        groupBoxPlayer.Name = "groupBoxPlayer";
        groupBoxPlayer.Size = new Size(559, 61);
        groupBoxPlayer.TabIndex = 1;
        groupBoxPlayer.TabStop = false;
        groupBoxPlayer.Text = "プレイヤー情報";
        // 
        // buttonChangeMe
        // 
        buttonChangeMe.AutoSize = true;
        buttonChangeMe.Enabled = false;
        buttonChangeMe.Location = new Point(461, 20);
        buttonChangeMe.Name = "buttonChangeMe";
        buttonChangeMe.Size = new Size(41, 25);
        buttonChangeMe.TabIndex = 4;
        buttonChangeMe.Text = "変更";
        buttonChangeMe.UseVisualStyleBackColor = true;
        buttonChangeMe.Click += ButtonChangeMe_Click;
        // 
        // textBoxRating
        // 
        textBoxRating.Location = new Point(376, 22);
        textBoxRating.MaxLength = 5;
        textBoxRating.Name = "textBoxRating";
        textBoxRating.Size = new Size(49, 23);
        textBoxRating.TabIndex = 3;
        textBoxRating.Validating += TextBoxRating_Validating;
        textBoxRating.Validated += ResetErrorProvider_Validated;
        // 
        // labelPlayerRating
        // 
        labelPlayerRating.AutoSize = true;
        labelPlayerRating.Location = new Point(309, 25);
        labelPlayerRating.Name = "labelPlayerRating";
        labelPlayerRating.Size = new Size(61, 15);
        labelPlayerRating.TabIndex = 2;
        labelPlayerRating.Text = "レーティング:";
        // 
        // textBoxPlayerName
        // 
        textBoxPlayerName.Location = new Point(79, 22);
        textBoxPlayerName.MaxLength = 32;
        textBoxPlayerName.Name = "textBoxPlayerName";
        textBoxPlayerName.Size = new Size(205, 23);
        textBoxPlayerName.TabIndex = 1;
        textBoxPlayerName.Validating += RequireTextBox_Validating;
        textBoxPlayerName.Validated += ResetErrorProvider_Validated;
        // 
        // labelPlayerName
        // 
        labelPlayerName.AutoSize = true;
        labelPlayerName.Location = new Point(6, 25);
        labelPlayerName.Name = "labelPlayerName";
        labelPlayerName.Size = new Size(67, 15);
        labelPlayerName.TabIndex = 0;
        labelPlayerName.Text = "プレイヤー名:";
        // 
        // groupBoxCreateRoom
        // 
        groupBoxCreateRoom.Controls.Add(buttonCreateRoom);
        groupBoxCreateRoom.Controls.Add(textBoxRoomSize);
        groupBoxCreateRoom.Controls.Add(labelRoomSize);
        groupBoxCreateRoom.Enabled = false;
        groupBoxCreateRoom.Location = new Point(13, 155);
        groupBoxCreateRoom.Name = "groupBoxCreateRoom";
        groupBoxCreateRoom.Size = new Size(180, 61);
        groupBoxCreateRoom.TabIndex = 2;
        groupBoxCreateRoom.TabStop = false;
        groupBoxCreateRoom.Text = "部屋を作る";
        // 
        // buttonCreateRoom
        // 
        buttonCreateRoom.AutoSize = true;
        buttonCreateRoom.Location = new Point(104, 20);
        buttonCreateRoom.Name = "buttonCreateRoom";
        buttonCreateRoom.Size = new Size(41, 25);
        buttonCreateRoom.TabIndex = 2;
        buttonCreateRoom.Text = "作成";
        buttonCreateRoom.UseVisualStyleBackColor = true;
        buttonCreateRoom.Click += ButtonCreateRoom_Click;
        // 
        // textBoxRoomSize
        // 
        textBoxRoomSize.Location = new Point(46, 22);
        textBoxRoomSize.MaxLength = 2;
        textBoxRoomSize.Name = "textBoxRoomSize";
        textBoxRoomSize.Size = new Size(40, 23);
        textBoxRoomSize.TabIndex = 1;
        textBoxRoomSize.Text = "2";
        textBoxRoomSize.Validating += TextBoxRoomSize_Validating;
        textBoxRoomSize.Validated += ResetErrorProvider_Validated;
        // 
        // labelRoomSize
        // 
        labelRoomSize.AutoSize = true;
        labelRoomSize.Location = new Point(6, 25);
        labelRoomSize.Name = "labelRoomSize";
        labelRoomSize.Size = new Size(34, 15);
        labelRoomSize.TabIndex = 0;
        labelRoomSize.Text = "人数:";
        // 
        // groupBoxMatch
        // 
        groupBoxMatch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        groupBoxMatch.Controls.Add(buttonMatch);
        groupBoxMatch.Enabled = false;
        groupBoxMatch.Location = new Point(217, 155);
        groupBoxMatch.Name = "groupBoxMatch";
        groupBoxMatch.Size = new Size(149, 61);
        groupBoxMatch.TabIndex = 3;
        groupBoxMatch.TabStop = false;
        groupBoxMatch.Text = "部屋を探す";
        // 
        // buttonMatch
        // 
        buttonMatch.AutoSize = true;
        buttonMatch.Location = new Point(16, 20);
        buttonMatch.Name = "buttonMatch";
        buttonMatch.Size = new Size(41, 25);
        buttonMatch.TabIndex = 0;
        buttonMatch.Text = "探索";
        buttonMatch.UseVisualStyleBackColor = true;
        buttonMatch.Click += ButtonMatch_Click;
        // 
        // groupBoxList
        // 
        groupBoxList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        groupBoxList.Controls.Add(listViewRoomList);
        groupBoxList.Enabled = false;
        groupBoxList.Location = new Point(389, 155);
        groupBoxList.Name = "groupBoxList";
        groupBoxList.Size = new Size(183, 274);
        groupBoxList.TabIndex = 4;
        groupBoxList.TabStop = false;
        groupBoxList.Text = "部屋一覧";
        // 
        // listViewRoomList
        // 
        listViewRoomList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        listViewRoomList.Columns.AddRange(new ColumnHeader[] { columnHeaderRoomListNo, columnHeaderRoomListPlayers, columnHeaderRoomListRating });
        listViewRoomList.Location = new Point(11, 22);
        listViewRoomList.Name = "listViewRoomList";
        listViewRoomList.Size = new Size(157, 235);
        listViewRoomList.TabIndex = 0;
        listViewRoomList.UseCompatibleStateImageBehavior = false;
        listViewRoomList.View = View.Details;
        // 
        // columnHeaderRoomListNo
        // 
        columnHeaderRoomListNo.Text = "番号";
        columnHeaderRoomListNo.Width = 40;
        // 
        // columnHeaderRoomListPlayers
        // 
        columnHeaderRoomListPlayers.Text = "人数";
        columnHeaderRoomListPlayers.Width = 40;
        // 
        // columnHeaderRoomListRating
        // 
        columnHeaderRoomListRating.Text = "レーティング";
        columnHeaderRoomListRating.Width = 70;
        // 
        // groupBoxGame
        // 
        groupBoxGame.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        groupBoxGame.Controls.Add(buttonReactionGame);
        groupBoxGame.Controls.Add(listViewMemberList);
        groupBoxGame.Controls.Add(labelMemberList);
        groupBoxGame.Controls.Add(buttonLeaveRoom);
        groupBoxGame.Controls.Add(textBoxRoomNo);
        groupBoxGame.Controls.Add(labelRoomNo);
        groupBoxGame.Enabled = false;
        groupBoxGame.Location = new Point(13, 233);
        groupBoxGame.Name = "groupBoxGame";
        groupBoxGame.Size = new Size(353, 196);
        groupBoxGame.TabIndex = 5;
        groupBoxGame.TabStop = false;
        groupBoxGame.Text = "ゲームプレイ";
        // 
        // buttonReactionGame
        // 
        buttonReactionGame.AutoSize = true;
        buttonReactionGame.Font = new Font("Yu Gothic UI", 12F);
        buttonReactionGame.Location = new Point(173, 90);
        buttonReactionGame.Name = "buttonReactionGame";
        buttonReactionGame.Size = new Size(143, 47);
        buttonReactionGame.TabIndex = 0;
        buttonReactionGame.Text = "早押しゲーム起動";
        buttonReactionGame.UseVisualStyleBackColor = true;
        buttonReactionGame.Click += ButtonReactionGame_Click;
        // 
        // listViewMemberList
        // 
        listViewMemberList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        listViewMemberList.Location = new Point(14, 78);
        listViewMemberList.Name = "listViewMemberList";
        listViewMemberList.Size = new Size(84, 101);
        listViewMemberList.TabIndex = 5;
        listViewMemberList.UseCompatibleStateImageBehavior = false;
        listViewMemberList.View = View.List;
        // 
        // labelMemberList
        // 
        labelMemberList.AutoSize = true;
        labelMemberList.Location = new Point(6, 55);
        labelMemberList.Name = "labelMemberList";
        labelMemberList.Size = new Size(46, 15);
        labelMemberList.TabIndex = 4;
        labelMemberList.Text = "参加者:";
        // 
        // buttonLeaveRoom
        // 
        buttonLeaveRoom.AutoSize = true;
        buttonLeaveRoom.Location = new Point(132, 21);
        buttonLeaveRoom.Name = "buttonLeaveRoom";
        buttonLeaveRoom.Size = new Size(41, 25);
        buttonLeaveRoom.TabIndex = 3;
        buttonLeaveRoom.Text = "退室";
        buttonLeaveRoom.UseVisualStyleBackColor = true;
        buttonLeaveRoom.Click += ButtonLeaveRoom_Click;
        // 
        // textBoxRoomNo
        // 
        textBoxRoomNo.Location = new Point(70, 22);
        textBoxRoomNo.MaxLength = 10;
        textBoxRoomNo.Name = "textBoxRoomNo";
        textBoxRoomNo.ReadOnly = true;
        textBoxRoomNo.Size = new Size(56, 23);
        textBoxRoomNo.TabIndex = 2;
        // 
        // labelRoomNo
        // 
        labelRoomNo.AutoSize = true;
        labelRoomNo.Location = new Point(6, 25);
        labelRoomNo.Name = "labelRoomNo";
        labelRoomNo.Size = new Size(58, 15);
        labelRoomNo.TabIndex = 1;
        labelRoomNo.Text = "部屋番号:";
        // 
        // errorProvider
        // 
        errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        errorProvider.ContainerControl = this;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(584, 441);
        Controls.Add(groupBoxGame);
        Controls.Add(groupBoxList);
        Controls.Add(groupBoxMatch);
        Controls.Add(groupBoxCreateRoom);
        Controls.Add(groupBoxPlayer);
        Controls.Add(groupBoxConfig);
        MaximizeBox = false;
        MaximumSize = new Size(600, 600);
        MinimumSize = new Size(600, 480);
        Name = "MainForm";
        ShowIcon = false;
        Text = "早押し対戦アプリ";
        FormClosed += MainForm_FormClosed;
        Load += MainForm_Load;
        groupBoxConfig.ResumeLayout(false);
        groupBoxConfig.PerformLayout();
        groupBoxPlayer.ResumeLayout(false);
        groupBoxPlayer.PerformLayout();
        groupBoxCreateRoom.ResumeLayout(false);
        groupBoxCreateRoom.PerformLayout();
        groupBoxMatch.ResumeLayout(false);
        groupBoxMatch.PerformLayout();
        groupBoxList.ResumeLayout(false);
        groupBoxGame.ResumeLayout(false);
        groupBoxGame.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
        ResumeLayout(false);

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
    private System.Windows.Forms.Button buttonReactionGame;
    private System.Windows.Forms.ErrorProvider errorProvider;
}

