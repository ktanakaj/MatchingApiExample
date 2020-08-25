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
            this.textBoxUrl = new System.Windows.Forms.TextBox();
            this.labelUrl = new System.Windows.Forms.Label();
            this.groupBoxPlayer = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
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
            this.groupBoxGame = new System.Windows.Forms.GroupBox();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.listViewMemberList = new System.Windows.Forms.ListView();
            this.labelMemberList = new System.Windows.Forms.Label();
            this.checkBoxGameRandom = new System.Windows.Forms.CheckBox();
            this.buttonGamePaper = new System.Windows.Forms.Button();
            this.buttonGameScissors = new System.Windows.Forms.Button();
            this.buttonGameRock = new System.Windows.Forms.Button();
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
            this.groupBoxConfig.Controls.Add(this.textBoxUrl);
            this.groupBoxConfig.Controls.Add(this.labelUrl);
            this.groupBoxConfig.Location = new System.Drawing.Point(13, 12);
            this.groupBoxConfig.Name = "groupBoxConfig";
            this.groupBoxConfig.Size = new System.Drawing.Size(559, 61);
            this.groupBoxConfig.TabIndex = 0;
            this.groupBoxConfig.TabStop = false;
            this.groupBoxConfig.Text = "環境設定";
            // 
            // textBoxUrl
            // 
            this.textBoxUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUrl.Location = new System.Drawing.Point(79, 22);
            this.textBoxUrl.MaxLength = 4096;
            this.textBoxUrl.Name = "textBoxUrl";
            this.textBoxUrl.Size = new System.Drawing.Size(466, 23);
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
            this.groupBoxPlayer.Controls.Add(this.textBox1);
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
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(376, 22);
            this.textBox1.MaxLength = 5;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(49, 23);
            this.textBox1.TabIndex = 3;
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
            this.groupBoxCreateRoom.Location = new System.Drawing.Point(13, 155);
            this.groupBoxCreateRoom.Name = "groupBoxCreateRoom";
            this.groupBoxCreateRoom.Size = new System.Drawing.Size(206, 61);
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
            // 
            // textBoxRoomSize
            // 
            this.textBoxRoomSize.Location = new System.Drawing.Point(46, 22);
            this.textBoxRoomSize.MaxLength = 2;
            this.textBoxRoomSize.Name = "textBoxRoomSize";
            this.textBoxRoomSize.Size = new System.Drawing.Size(52, 23);
            this.textBoxRoomSize.TabIndex = 1;
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
            this.groupBoxMatch.Location = new System.Drawing.Point(233, 155);
            this.groupBoxMatch.Name = "groupBoxMatch";
            this.groupBoxMatch.Size = new System.Drawing.Size(206, 61);
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
            // 
            // groupBoxList
            // 
            this.groupBoxList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxList.Controls.Add(this.listViewRoomList);
            this.groupBoxList.Location = new System.Drawing.Point(455, 155);
            this.groupBoxList.Name = "groupBoxList";
            this.groupBoxList.Size = new System.Drawing.Size(117, 394);
            this.groupBoxList.TabIndex = 4;
            this.groupBoxList.TabStop = false;
            this.groupBoxList.Text = "部屋一覧";
            // 
            // listViewRoomList
            // 
            this.listViewRoomList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewRoomList.HideSelection = false;
            this.listViewRoomList.Location = new System.Drawing.Point(11, 22);
            this.listViewRoomList.Name = "listViewRoomList";
            this.listViewRoomList.Size = new System.Drawing.Size(91, 355);
            this.listViewRoomList.TabIndex = 0;
            this.listViewRoomList.UseCompatibleStateImageBehavior = false;
            // 
            // groupBoxGame
            // 
            this.groupBoxGame.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGame.Controls.Add(this.textBoxLog);
            this.groupBoxGame.Controls.Add(this.listViewMemberList);
            this.groupBoxGame.Controls.Add(this.labelMemberList);
            this.groupBoxGame.Controls.Add(this.checkBoxGameRandom);
            this.groupBoxGame.Controls.Add(this.buttonGamePaper);
            this.groupBoxGame.Controls.Add(this.buttonGameScissors);
            this.groupBoxGame.Controls.Add(this.buttonGameRock);
            this.groupBoxGame.Controls.Add(this.buttonLeaveRoom);
            this.groupBoxGame.Controls.Add(this.textBoxRoomNo);
            this.groupBoxGame.Controls.Add(this.labelRoomNo);
            this.groupBoxGame.Location = new System.Drawing.Point(13, 233);
            this.groupBoxGame.Name = "groupBoxGame";
            this.groupBoxGame.Size = new System.Drawing.Size(426, 316);
            this.groupBoxGame.TabIndex = 5;
            this.groupBoxGame.TabStop = false;
            this.groupBoxGame.Text = "ゲームプレイ";
            // 
            // textBoxLog
            // 
            this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLog.Location = new System.Drawing.Point(113, 61);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(302, 238);
            this.textBoxLog.TabIndex = 8;
            // 
            // listViewMemberList
            // 
            this.listViewMemberList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewMemberList.HideSelection = false;
            this.listViewMemberList.Location = new System.Drawing.Point(14, 102);
            this.listViewMemberList.Name = "listViewMemberList";
            this.listViewMemberList.Size = new System.Drawing.Size(84, 156);
            this.listViewMemberList.TabIndex = 7;
            this.listViewMemberList.UseCompatibleStateImageBehavior = false;
            // 
            // labelMemberList
            // 
            this.labelMemberList.AutoSize = true;
            this.labelMemberList.Location = new System.Drawing.Point(6, 79);
            this.labelMemberList.Name = "labelMemberList";
            this.labelMemberList.Size = new System.Drawing.Size(46, 15);
            this.labelMemberList.TabIndex = 6;
            this.labelMemberList.Text = "参加者:";
            // 
            // checkBoxGameRandom
            // 
            this.checkBoxGameRandom.AutoSize = true;
            this.checkBoxGameRandom.Location = new System.Drawing.Point(331, 31);
            this.checkBoxGameRandom.Name = "checkBoxGameRandom";
            this.checkBoxGameRandom.Size = new System.Drawing.Size(61, 19);
            this.checkBoxGameRandom.TabIndex = 5;
            this.checkBoxGameRandom.Text = "ランダム";
            this.checkBoxGameRandom.UseVisualStyleBackColor = true;
            // 
            // buttonGamePaper
            // 
            this.buttonGamePaper.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.buttonGamePaper.Location = new System.Drawing.Point(267, 22);
            this.buttonGamePaper.Name = "buttonGamePaper";
            this.buttonGamePaper.Size = new System.Drawing.Size(58, 31);
            this.buttonGamePaper.TabIndex = 4;
            this.buttonGamePaper.Text = "パー";
            this.buttonGamePaper.UseVisualStyleBackColor = true;
            // 
            // buttonGameScissors
            // 
            this.buttonGameScissors.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.buttonGameScissors.Location = new System.Drawing.Point(203, 22);
            this.buttonGameScissors.Name = "buttonGameScissors";
            this.buttonGameScissors.Size = new System.Drawing.Size(58, 31);
            this.buttonGameScissors.TabIndex = 3;
            this.buttonGameScissors.Text = "チョキ";
            this.buttonGameScissors.UseVisualStyleBackColor = true;
            // 
            // buttonGameRock
            // 
            this.buttonGameRock.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.buttonGameRock.Location = new System.Drawing.Point(139, 22);
            this.buttonGameRock.Name = "buttonGameRock";
            this.buttonGameRock.Size = new System.Drawing.Size(58, 31);
            this.buttonGameRock.TabIndex = 2;
            this.buttonGameRock.Text = "グー";
            this.buttonGameRock.UseVisualStyleBackColor = true;
            // 
            // buttonLeaveRoom
            // 
            this.buttonLeaveRoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonLeaveRoom.AutoSize = true;
            this.buttonLeaveRoom.Location = new System.Drawing.Point(11, 274);
            this.buttonLeaveRoom.Name = "buttonLeaveRoom";
            this.buttonLeaveRoom.Size = new System.Drawing.Size(41, 25);
            this.buttonLeaveRoom.TabIndex = 9;
            this.buttonLeaveRoom.Text = "退室";
            this.buttonLeaveRoom.UseVisualStyleBackColor = true;
            // 
            // textBoxRoomNo
            // 
            this.textBoxRoomNo.Location = new System.Drawing.Point(17, 43);
            this.textBoxRoomNo.MaxLength = 10;
            this.textBoxRoomNo.Name = "textBoxRoomNo";
            this.textBoxRoomNo.ReadOnly = true;
            this.textBoxRoomNo.Size = new System.Drawing.Size(56, 23);
            this.textBoxRoomNo.TabIndex = 1;
            // 
            // labelRoomNo
            // 
            this.labelRoomNo.AutoSize = true;
            this.labelRoomNo.Location = new System.Drawing.Point(6, 25);
            this.labelRoomNo.Name = "labelRoomNo";
            this.labelRoomNo.Size = new System.Drawing.Size(58, 15);
            this.labelRoomNo.TabIndex = 0;
            this.labelRoomNo.Text = "部屋番号:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.groupBoxGame);
            this.Controls.Add(this.groupBoxList);
            this.Controls.Add(this.groupBoxMatch);
            this.Controls.Add(this.groupBoxCreateRoom);
            this.Controls.Add(this.groupBoxPlayer);
            this.Controls.Add(this.groupBoxConfig);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 600);
            this.Name = "MainForm";
            this.Text = "じゃんけん対戦アプリ";
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
        private System.Windows.Forms.TextBox textBox1;
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
        private System.Windows.Forms.Button buttonGamePaper;
        private System.Windows.Forms.Button buttonGameScissors;
        private System.Windows.Forms.Button buttonGameRock;
        private System.Windows.Forms.Button buttonLeaveRoom;
        private System.Windows.Forms.TextBox textBoxRoomNo;
        private System.Windows.Forms.Label labelRoomNo;
        private System.Windows.Forms.CheckBox checkBoxGameRandom;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.ListView listViewMemberList;
        private System.Windows.Forms.Label labelMemberList;
    }
}

