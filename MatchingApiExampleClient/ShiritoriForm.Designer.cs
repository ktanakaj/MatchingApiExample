namespace Honememo.MatchingApiExample.Client
{
    partial class ShiritoriForm
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
            this.labelMemberList = new System.Windows.Forms.Label();
            this.listViewMemberList = new System.Windows.Forms.ListView();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.labelLog = new System.Windows.Forms.Label();
            this.labelInput = new System.Windows.Forms.Label();
            this.textBoxWord = new System.Windows.Forms.TextBox();
            this.buttonSubmit = new System.Windows.Forms.Button();
            this.buttonClaim = new System.Windows.Forms.Button();
            this.labelResult = new System.Windows.Forms.Label();
            this.labelCountdown = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelMemberList
            // 
            this.labelMemberList.AutoSize = true;
            this.labelMemberList.Location = new System.Drawing.Point(12, 9);
            this.labelMemberList.Name = "labelMemberList";
            this.labelMemberList.Size = new System.Drawing.Size(46, 15);
            this.labelMemberList.TabIndex = 3;
            this.labelMemberList.Text = "参加者:";
            // 
            // listViewMemberList
            // 
            this.listViewMemberList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewMemberList.HideSelection = false;
            this.listViewMemberList.Location = new System.Drawing.Point(12, 27);
            this.listViewMemberList.Name = "listViewMemberList";
            this.listViewMemberList.Size = new System.Drawing.Size(84, 156);
            this.listViewMemberList.TabIndex = 4;
            this.listViewMemberList.UseCompatibleStateImageBehavior = false;
            this.listViewMemberList.View = System.Windows.Forms.View.List;
            // 
            // textBoxLog
            // 
            this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLog.Location = new System.Drawing.Point(111, 27);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(341, 229);
            this.textBoxLog.TabIndex = 6;
            // 
            // labelLog
            // 
            this.labelLog.AutoSize = true;
            this.labelLog.Location = new System.Drawing.Point(111, 9);
            this.labelLog.Name = "labelLog";
            this.labelLog.Size = new System.Drawing.Size(34, 15);
            this.labelLog.TabIndex = 5;
            this.labelLog.Text = "履歴:";
            // 
            // labelInput
            // 
            this.labelInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelInput.AutoSize = true;
            this.labelInput.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelInput.Location = new System.Drawing.Point(86, 311);
            this.labelInput.Name = "labelInput";
            this.labelInput.Size = new System.Drawing.Size(319, 21);
            this.labelInput.TabIndex = 0;
            this.labelInput.Text = "ひらがなで「{0}」で始まる単語を入力してください。";
            // 
            // textBoxWord
            // 
            this.textBoxWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWord.Enabled = false;
            this.textBoxWord.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxWord.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.textBoxWord.Location = new System.Drawing.Point(121, 335);
            this.textBoxWord.MaxLength = 16;
            this.textBoxWord.Name = "textBoxWord";
            this.textBoxWord.Size = new System.Drawing.Size(219, 29);
            this.textBoxWord.TabIndex = 1;
            // 
            // buttonSubmit
            // 
            this.buttonSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSubmit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonSubmit.Enabled = false;
            this.buttonSubmit.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.buttonSubmit.Location = new System.Drawing.Point(197, 370);
            this.buttonSubmit.Name = "buttonSubmit";
            this.buttonSubmit.Size = new System.Drawing.Size(52, 31);
            this.buttonSubmit.TabIndex = 2;
            this.buttonSubmit.Text = "決定";
            this.buttonSubmit.UseVisualStyleBackColor = true;
            this.buttonSubmit.Click += new System.EventHandler(this.ButtonSubmit_Click);
            // 
            // buttonClaim
            // 
            this.buttonClaim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClaim.AutoSize = true;
            this.buttonClaim.Enabled = false;
            this.buttonClaim.Location = new System.Drawing.Point(393, 262);
            this.buttonClaim.Name = "buttonClaim";
            this.buttonClaim.Size = new System.Drawing.Size(59, 25);
            this.buttonClaim.TabIndex = 7;
            this.buttonClaim.Text = "異議あり";
            this.buttonClaim.UseVisualStyleBackColor = true;
            this.buttonClaim.Click += new System.EventHandler(this.ButtonClaim_Click);
            // 
            // labelResult
            // 
            this.labelResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Yu Gothic UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelResult.ForeColor = System.Drawing.Color.Red;
            this.labelResult.Location = new System.Drawing.Point(157, 371);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(34, 30);
            this.labelResult.TabIndex = 8;
            this.labelResult.Text = "〇";
            // 
            // labelCountdown
            // 
            this.labelCountdown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCountdown.AutoSize = true;
            this.labelCountdown.Font = new System.Drawing.Font("Yu Gothic UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelCountdown.Location = new System.Drawing.Point(265, 370);
            this.labelCountdown.Name = "labelCountdown";
            this.labelCountdown.Size = new System.Drawing.Size(33, 30);
            this.labelCountdown.TabIndex = 9;
            this.labelCountdown.Text = "10";
            // 
            // ShiritoriForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 421);
            this.Controls.Add(this.labelCountdown);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.buttonClaim);
            this.Controls.Add(this.buttonSubmit);
            this.Controls.Add(this.textBoxWord);
            this.Controls.Add(this.labelInput);
            this.Controls.Add(this.labelLog);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.listViewMemberList);
            this.Controls.Add(this.labelMemberList);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(960, 920);
            this.MinimumSize = new System.Drawing.Size(480, 460);
            this.Name = "ShiritoriForm";
            this.ShowIcon = false;
            this.Text = "しりとり（部屋番号{0}）";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShiritoriForm_FormClosed);
            this.Load += new System.EventHandler(this.ShiritoriForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMemberList;
        private System.Windows.Forms.ListView listViewMemberList;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Label labelLog;
        private System.Windows.Forms.Label labelInput;
        private System.Windows.Forms.TextBox textBoxWord;
        private System.Windows.Forms.Button buttonSubmit;
        private System.Windows.Forms.Button buttonClaim;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Label labelCountdown;
    }
}