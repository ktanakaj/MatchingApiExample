namespace Honememo.MatchingApiExample.Client
{
    partial class ReactionGameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelMemberList = new Label();
            listViewMemberList = new ListView();
            textBoxLog = new TextBox();
            labelLog = new Label();
            labelMessage = new Label();
            buttonSubmit = new Button();
            SuspendLayout();
            // 
            // labelMemberList
            // 
            labelMemberList.AutoSize = true;
            labelMemberList.Location = new Point(12, 9);
            labelMemberList.Name = "labelMemberList";
            labelMemberList.Size = new Size(46, 15);
            labelMemberList.TabIndex = 3;
            labelMemberList.Text = "参加者:";
            // 
            // listViewMemberList
            // 
            listViewMemberList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listViewMemberList.Location = new Point(12, 27);
            listViewMemberList.Name = "listViewMemberList";
            listViewMemberList.Size = new Size(84, 156);
            listViewMemberList.TabIndex = 4;
            listViewMemberList.UseCompatibleStateImageBehavior = false;
            listViewMemberList.View = View.List;
            // 
            // textBoxLog
            // 
            textBoxLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxLog.Location = new Point(111, 27);
            textBoxLog.Multiline = true;
            textBoxLog.Name = "textBoxLog";
            textBoxLog.ReadOnly = true;
            textBoxLog.ScrollBars = ScrollBars.Vertical;
            textBoxLog.Size = new Size(341, 229);
            textBoxLog.TabIndex = 6;
            // 
            // labelLog
            // 
            labelLog.AutoSize = true;
            labelLog.Location = new Point(111, 9);
            labelLog.Name = "labelLog";
            labelLog.Size = new Size(34, 15);
            labelLog.TabIndex = 5;
            labelLog.Text = "履歴:";
            // 
            // labelMessage
            // 
            labelMessage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelMessage.AutoSize = true;
            labelMessage.Font = new Font("Yu Gothic UI", 12F);
            labelMessage.Location = new Point(86, 311);
            labelMessage.Name = "labelMessage";
            labelMessage.Size = new Size(240, 21);
            labelMessage.TabIndex = 0;
            labelMessage.Text = "他のプレイヤーの入室を待っています…";
            // 
            // buttonSubmit
            // 
            buttonSubmit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            buttonSubmit.AutoSize = true;
            buttonSubmit.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonSubmit.Enabled = false;
            buttonSubmit.Font = new Font("Yu Gothic UI", 12F);
            buttonSubmit.Location = new Point(197, 370);
            buttonSubmit.Name = "buttonSubmit";
            buttonSubmit.Size = new Size(65, 31);
            buttonSubmit.TabIndex = 2;
            buttonSubmit.Text = "押す！";
            buttonSubmit.UseVisualStyleBackColor = true;
            buttonSubmit.Click += ButtonSubmit_Click;
            // 
            // ReactionGameForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(464, 421);
            Controls.Add(buttonSubmit);
            Controls.Add(labelMessage);
            Controls.Add(labelLog);
            Controls.Add(textBoxLog);
            Controls.Add(listViewMemberList);
            Controls.Add(labelMemberList);
            MaximizeBox = false;
            MaximumSize = new Size(960, 920);
            MinimumSize = new Size(480, 460);
            Name = "ReactionGameForm";
            ShowIcon = false;
            Text = "早押しゲーム（部屋番号{0}）";
            FormClosed += ReactionGameForm_FormClosed;
            Load += ReactionGameForm_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMemberList;
        private System.Windows.Forms.ListView listViewMemberList;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Label labelLog;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Button buttonSubmit;
    }
}