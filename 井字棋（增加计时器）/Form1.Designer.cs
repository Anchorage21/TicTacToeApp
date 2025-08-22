namespace TicTacToeWinForms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btnPos0 = new System.Windows.Forms.Button();
            this.btnPos1 = new System.Windows.Forms.Button();
            this.btnPos2 = new System.Windows.Forms.Button();
            this.btnPos3 = new System.Windows.Forms.Button();
            this.btnPos4 = new System.Windows.Forms.Button();
            this.btnPos5 = new System.Windows.Forms.Button();
            this.btnPos6 = new System.Windows.Forms.Button();
            this.btnPos7 = new System.Windows.Forms.Button();
            this.btnPos8 = new System.Windows.Forms.Button();
            this.btnNewGamePlayerFirst = new System.Windows.Forms.Button();
            this.btnNewGameComputerFirst = new System.Windows.Forms.Button();
            this.btnShowHistory = new System.Windows.Forms.Button();
            this.btnShowStats = new System.Windows.Forms.Button();
            this.btnResetStats = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblTimer = new System.Windows.Forms.Label();
            this.btnSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPos0
            // 
            this.btnPos0.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPos0.Location = new System.Drawing.Point(30, 50);
            this.btnPos0.Name = "btnPos0";
            this.btnPos0.Size = new System.Drawing.Size(60, 60);
            this.btnPos0.TabIndex = 0;
            this.btnPos0.UseVisualStyleBackColor = true;
            // 
            // btnPos1
            // 
            this.btnPos1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPos1.Location = new System.Drawing.Point(96, 50);
            this.btnPos1.Name = "btnPos1";
            this.btnPos1.Size = new System.Drawing.Size(60, 60);
            this.btnPos1.TabIndex = 1;
            this.btnPos1.UseVisualStyleBackColor = true;
            // 
            // btnPos2
            // 
            this.btnPos2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPos2.Location = new System.Drawing.Point(162, 50);
            this.btnPos2.Name = "btnPos2";
            this.btnPos2.Size = new System.Drawing.Size(60, 60);
            this.btnPos2.TabIndex = 2;
            this.btnPos2.UseVisualStyleBackColor = true;
            // 
            // btnPos3
            // 
            this.btnPos3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPos3.Location = new System.Drawing.Point(30, 116);
            this.btnPos3.Name = "btnPos3";
            this.btnPos3.Size = new System.Drawing.Size(60, 60);
            this.btnPos3.TabIndex = 3;
            this.btnPos3.UseVisualStyleBackColor = true;
            // 
            // btnPos4
            // 
            this.btnPos4.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPos4.Location = new System.Drawing.Point(96, 116);
            this.btnPos4.Name = "btnPos4";
            this.btnPos4.Size = new System.Drawing.Size(60, 60);
            this.btnPos4.TabIndex = 4;
            this.btnPos4.UseVisualStyleBackColor = true;
            // 
            // btnPos5
            // 
            this.btnPos5.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPos5.Location = new System.Drawing.Point(162, 116);
            this.btnPos5.Name = "btnPos5";
            this.btnPos5.Size = new System.Drawing.Size(60, 60);
            this.btnPos5.TabIndex = 5;
            this.btnPos5.UseVisualStyleBackColor = true;
            // 
            // btnPos6
            // 
            this.btnPos6.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPos6.Location = new System.Drawing.Point(30, 182);
            this.btnPos6.Name = "btnPos6";
            this.btnPos6.Size = new System.Drawing.Size(60, 60);
            this.btnPos6.TabIndex = 6;
            this.btnPos6.UseVisualStyleBackColor = true;
            // 
            // btnPos7
            // 
            this.btnPos7.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPos7.Location = new System.Drawing.Point(96, 182);
            this.btnPos7.Name = "btnPos7";
            this.btnPos7.Size = new System.Drawing.Size(60, 60);
            this.btnPos7.TabIndex = 7;
            this.btnPos7.UseVisualStyleBackColor = true;
            // 
            // btnPos8
            // 
            this.btnPos8.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPos8.Location = new System.Drawing.Point(162, 182);
            this.btnPos8.Name = "btnPos8";
            this.btnPos8.Size = new System.Drawing.Size(60, 60);
            this.btnPos8.TabIndex = 8;
            this.btnPos8.UseVisualStyleBackColor = true;
            // 
            // btnNewGamePlayerFirst
            // 
            this.btnNewGamePlayerFirst.Location = new System.Drawing.Point(250, 50);
            this.btnNewGamePlayerFirst.Name = "btnNewGamePlayerFirst";
            this.btnNewGamePlayerFirst.Size = new System.Drawing.Size(150, 30);
            this.btnNewGamePlayerFirst.TabIndex = 9;
            this.btnNewGamePlayerFirst.Text = "新游戏 (玩家先手)";
            this.btnNewGamePlayerFirst.UseVisualStyleBackColor = true;
            this.btnNewGamePlayerFirst.Click += new System.EventHandler(this.btnNewGamePlayerFirst_Click);
            // 
            // btnNewGameComputerFirst
            // 
            this.btnNewGameComputerFirst.Location = new System.Drawing.Point(250, 86);
            this.btnNewGameComputerFirst.Name = "btnNewGameComputerFirst";
            this.btnNewGameComputerFirst.Size = new System.Drawing.Size(150, 30);
            this.btnNewGameComputerFirst.TabIndex = 10;
            this.btnNewGameComputerFirst.Text = "新游戏 (电脑先手)";
            this.btnNewGameComputerFirst.UseVisualStyleBackColor = true;
            this.btnNewGameComputerFirst.Click += new System.EventHandler(this.btnNewGameComputerFirst_Click);
            // 
            // btnShowHistory
            // 
            this.btnShowHistory.Location = new System.Drawing.Point(250, 122);
            this.btnShowHistory.Name = "btnShowHistory";
            this.btnShowHistory.Size = new System.Drawing.Size(150, 30);
            this.btnShowHistory.TabIndex = 11;
            this.btnShowHistory.Text = "查看对决记录";
            this.btnShowHistory.UseVisualStyleBackColor = true;
            this.btnShowHistory.Click += new System.EventHandler(this.btnShowHistory_Click);
            // 
            // btnShowStats
            // 
            this.btnShowStats.Location = new System.Drawing.Point(250, 158);
            this.btnShowStats.Name = "btnShowStats";
            this.btnShowStats.Size = new System.Drawing.Size(150, 30);
            this.btnShowStats.TabIndex = 12;
            this.btnShowStats.Text = "显示统计";
            this.btnShowStats.UseVisualStyleBackColor = true;
            this.btnShowStats.Click += new System.EventHandler(this.btnShowStats_Click);
            // 
            // btnResetStats
            // 
            this.btnResetStats.Location = new System.Drawing.Point(250, 194);
            this.btnResetStats.Name = "btnResetStats";
            this.btnResetStats.Size = new System.Drawing.Size(150, 30);
            this.btnResetStats.TabIndex = 13;
            this.btnResetStats.Text = "重置统计";
            this.btnResetStats.UseVisualStyleBackColor = true;
            this.btnResetStats.Click += new System.EventHandler(this.btnResetStats_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(250, 230);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(150, 30);
            this.btnExit.TabIndex = 14;
            this.btnExit.Text = "退出";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(30, 260);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(55, 13);
            this.lblStatus.TabIndex = 15;
            this.lblStatus.Text = "游戏状态";
            // 
            // lblTimer
            // 
            this.lblTimer.AutoSize = true;
            this.lblTimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimer.Location = new System.Drawing.Point(30, 20);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(106, 20);
            this.lblTimer.TabIndex = 16;
            this.lblTimer.Text = "剩余时间: 30秒";
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(250, 266);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(150, 30);
            this.btnSettings.TabIndex = 17;
            this.btnSettings.Text = "设置";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 310);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnResetStats);
            this.Controls.Add(this.btnShowStats);
            this.Controls.Add(this.btnShowHistory);
            this.Controls.Add(this.btnNewGameComputerFirst);
            this.Controls.Add(this.btnNewGamePlayerFirst);
            this.Controls.Add(this.btnPos8);
            this.Controls.Add(this.btnPos7);
            this.Controls.Add(this.btnPos6);
            this.Controls.Add(this.btnPos5);
            this.Controls.Add(this.btnPos4);
            this.Controls.Add(this.btnPos3);
            this.Controls.Add(this.btnPos2);
            this.Controls.Add(this.btnPos1);
            this.Controls.Add(this.btnPos0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "井字棋游戏";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPos0;
        private System.Windows.Forms.Button btnPos1;
        private System.Windows.Forms.Button btnPos2;
        private System.Windows.Forms.Button btnPos3;
        private System.Windows.Forms.Button btnPos4;
        private System.Windows.Forms.Button btnPos5;
        private System.Windows.Forms.Button btnPos6;
        private System.Windows.Forms.Button btnPos7;
        private System.Windows.Forms.Button btnPos8;
        private System.Windows.Forms.Button btnNewGamePlayerFirst;
        private System.Windows.Forms.Button btnNewGameComputerFirst;
        private System.Windows.Forms.Button btnShowHistory;
        private System.Windows.Forms.Button btnShowStats;
        private System.Windows.Forms.Button btnResetStats;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Button btnSettings;
    }
}