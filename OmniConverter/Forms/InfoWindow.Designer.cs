﻿namespace OmniConverter
{
    partial class InfoWindow
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
            this.components = new System.ComponentModel.Container();
            this.OCBigLogo = new System.Windows.Forms.PictureBox();
            this.VerLabel = new System.Windows.Forms.Label();
            this.OKBtn = new System.Windows.Forms.Button();
            this.CopyrightLabel = new System.Windows.Forms.Label();
            this.GitHubPage = new System.Windows.Forms.PictureBox();
            this.DIGroup = new System.Windows.Forms.GroupBox();
            this.CurBranch = new System.Windows.Forms.Label();
            this.BASSMIDIVer = new System.Windows.Forms.Label();
            this.BASSVer = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RAMAmount = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.WinVer = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.WinName = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.BranchToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ChangeBranch = new System.Windows.Forms.Button();
            this.CheckForUpdates = new System.Windows.Forms.Button();
            this.OMLicense = new System.Windows.Forms.PictureBox();
            this.DonateKep = new System.Windows.Forms.LinkLabel();
            this.DonateArd = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.OCBigLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GitHubPage)).BeginInit();
            this.DIGroup.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OMLicense)).BeginInit();
            this.SuspendLayout();
            // 
            // OCBigLogo
            // 
            this.OCBigLogo.Location = new System.Drawing.Point(11, 12);
            this.OCBigLogo.Name = "OCBigLogo";
            this.OCBigLogo.Size = new System.Drawing.Size(256, 256);
            this.OCBigLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.OCBigLogo.TabIndex = 0;
            this.OCBigLogo.TabStop = false;
            // 
            // VerLabel
            // 
            this.VerLabel.AutoSize = true;
            this.VerLabel.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VerLabel.Location = new System.Drawing.Point(273, 12);
            this.VerLabel.Name = "VerLabel";
            this.VerLabel.Size = new System.Drawing.Size(101, 24);
            this.VerLabel.TabIndex = 1;
            this.VerLabel.Text = "Template";
            this.VerLabel.Click += new System.EventHandler(this.VerLabel_Click);
            // 
            // OKBtn
            // 
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OKBtn.Location = new System.Drawing.Point(651, 244);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 24);
            this.OKBtn.TabIndex = 4;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // CopyrightLabel
            // 
            this.CopyrightLabel.Location = new System.Drawing.Point(274, 47);
            this.CopyrightLabel.Name = "CopyrightLabel";
            this.CopyrightLabel.Size = new System.Drawing.Size(272, 26);
            this.CopyrightLabel.TabIndex = 3;
            this.CopyrightLabel.Text = "Copyright Ⓒ 2019-{0} Keppy\'s Software and Arduano\r\nFree MIDI converter for Window" +
    "s 7 and newer";
            // 
            // GitHubPage
            // 
            this.GitHubPage.Location = new System.Drawing.Point(702, 12);
            this.GitHubPage.Name = "GitHubPage";
            this.GitHubPage.Size = new System.Drawing.Size(24, 24);
            this.GitHubPage.TabIndex = 5;
            this.GitHubPage.TabStop = false;
            this.GitHubPage.Click += new System.EventHandler(this.GitHubPage_Click);
            this.GitHubPage.MouseHover += new System.EventHandler(this.GitHubPage_MouseHover);
            // 
            // DIGroup
            // 
            this.DIGroup.Controls.Add(this.CurBranch);
            this.DIGroup.Controls.Add(this.BASSMIDIVer);
            this.DIGroup.Controls.Add(this.BASSVer);
            this.DIGroup.Controls.Add(this.label9);
            this.DIGroup.Controls.Add(this.label5);
            this.DIGroup.Controls.Add(this.label4);
            this.DIGroup.Location = new System.Drawing.Point(277, 138);
            this.DIGroup.Name = "DIGroup";
            this.DIGroup.Size = new System.Drawing.Size(220, 100);
            this.DIGroup.TabIndex = 6;
            this.DIGroup.TabStop = false;
            this.DIGroup.Text = "Driver information";
            // 
            // CurBranch
            // 
            this.CurBranch.AutoSize = true;
            this.CurBranch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurBranch.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CurBranch.Location = new System.Drawing.Point(105, 76);
            this.CurBranch.Name = "CurBranch";
            this.CurBranch.Size = new System.Drawing.Size(74, 13);
            this.CurBranch.TabIndex = 12;
            this.CurBranch.Text = "Unavailable";
            // 
            // BASSMIDIVer
            // 
            this.BASSMIDIVer.AutoSize = true;
            this.BASSMIDIVer.Location = new System.Drawing.Point(105, 38);
            this.BASSMIDIVer.Name = "BASSMIDIVer";
            this.BASSMIDIVer.Size = new System.Drawing.Size(54, 13);
            this.BASSMIDIVer.TabIndex = 11;
            this.BASSMIDIVer.Text = "LIB VER2";
            // 
            // BASSVer
            // 
            this.BASSVer.AutoSize = true;
            this.BASSVer.Location = new System.Drawing.Point(105, 19);
            this.BASSVer.Name = "BASSVer";
            this.BASSVer.Size = new System.Drawing.Size(54, 13);
            this.BASSVer.TabIndex = 10;
            this.BASSVer.Text = "LIB VER1";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Update branch:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "BASSMIDI version:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "BASS version:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(274, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(452, 42);
            this.label1.TabIndex = 7;
            this.label1.Text = "This software is open-source.\r\nRedistribution and use of this code or any derivat" +
    "ive works are permitted provided that specific conditions are met. Click the blu" +
    "e note button to see the license.\r\n";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RAMAmount);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.WinVer);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.WinName);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(506, 138);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(220, 100);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Windows install information";
            // 
            // RAMAmount
            // 
            this.RAMAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RAMAmount.AutoSize = true;
            this.RAMAmount.Location = new System.Drawing.Point(51, 76);
            this.RAMAmount.Name = "RAMAmount";
            this.RAMAmount.Size = new System.Drawing.Size(81, 13);
            this.RAMAmount.TabIndex = 16;
            this.RAMAmount.Text = "RAM AMOUNT";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 76);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "RAM:";
            // 
            // WinVer
            // 
            this.WinVer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.WinVer.Location = new System.Drawing.Point(51, 38);
            this.WinVer.Name = "WinVer";
            this.WinVer.Size = new System.Drawing.Size(163, 32);
            this.WinVer.TabIndex = 13;
            this.WinVer.Text = "WIN VERS";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 38);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Version:";
            // 
            // WinName
            // 
            this.WinName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.WinName.Location = new System.Drawing.Point(51, 19);
            this.WinName.Name = "WinName";
            this.WinName.Size = new System.Drawing.Size(163, 13);
            this.WinName.TabIndex = 12;
            this.WinName.Text = "WIN NAME";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Name:";
            // 
            // BranchToolTip
            // 
            this.BranchToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.BranchToolTip.ToolTipTitle = "Branch info";
            // 
            // ChangeBranch
            // 
            this.ChangeBranch.Location = new System.Drawing.Point(277, 244);
            this.ChangeBranch.Name = "ChangeBranch";
            this.ChangeBranch.Size = new System.Drawing.Size(101, 24);
            this.ChangeBranch.TabIndex = 2;
            this.ChangeBranch.Text = "Change branch";
            this.ChangeBranch.UseVisualStyleBackColor = true;
            this.ChangeBranch.Click += new System.EventHandler(this.ChangeBranch_Click);
            // 
            // CheckForUpdates
            // 
            this.CheckForUpdates.Location = new System.Drawing.Point(538, 244);
            this.CheckForUpdates.Name = "CheckForUpdates";
            this.CheckForUpdates.Size = new System.Drawing.Size(107, 24);
            this.CheckForUpdates.TabIndex = 3;
            this.CheckForUpdates.Text = "Check for updates";
            this.CheckForUpdates.UseVisualStyleBackColor = true;
            this.CheckForUpdates.Click += new System.EventHandler(this.CheckForUpdates_Click);
            // 
            // OMLicense
            // 
            this.OMLicense.Cursor = System.Windows.Forms.Cursors.Hand;
            this.OMLicense.Location = new System.Drawing.Point(672, 12);
            this.OMLicense.Name = "OMLicense";
            this.OMLicense.Size = new System.Drawing.Size(24, 24);
            this.OMLicense.TabIndex = 10;
            this.OMLicense.TabStop = false;
            this.OMLicense.Click += new System.EventHandler(this.OMLicense_Click);
            this.OMLicense.MouseHover += new System.EventHandler(this.OMLicense_MouseHover);
            // 
            // DonateKep
            // 
            this.DonateKep.AutoSize = true;
            this.DonateKep.LinkColor = System.Drawing.SystemColors.Highlight;
            this.DonateKep.Location = new System.Drawing.Point(587, 47);
            this.DonateKep.Name = "DonateKep";
            this.DonateKep.Size = new System.Drawing.Size(139, 13);
            this.DonateKep.TabIndex = 11;
            this.DonateKep.TabStop = true;
            this.DonateKep.Text = "Donate to Keppy\'s Software";
            this.DonateKep.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DonateKep_LinkClicked);
            // 
            // DonateArd
            // 
            this.DonateArd.AutoSize = true;
            this.DonateArd.LinkColor = System.Drawing.SystemColors.Highlight;
            this.DonateArd.Location = new System.Drawing.Point(629, 60);
            this.DonateArd.Name = "DonateArd";
            this.DonateArd.Size = new System.Drawing.Size(97, 13);
            this.DonateArd.TabIndex = 12;
            this.DonateArd.TabStop = true;
            this.DonateArd.Text = "Donate to Arduano";
            this.DonateArd.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DonateArd_LinkClicked);
            // 
            // InfoWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.OKBtn;
            this.ClientSize = new System.Drawing.Size(738, 280);
            this.Controls.Add(this.DonateArd);
            this.Controls.Add(this.DonateKep);
            this.Controls.Add(this.OMLicense);
            this.Controls.Add(this.CheckForUpdates);
            this.Controls.Add(this.ChangeBranch);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DIGroup);
            this.Controls.Add(this.GitHubPage);
            this.Controls.Add(this.CopyrightLabel);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.VerLabel);
            this.Controls.Add(this.OCBigLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InfoWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Information";
            this.Load += new System.EventHandler(this.InfoWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.OCBigLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GitHubPage)).EndInit();
            this.DIGroup.ResumeLayout(false);
            this.DIGroup.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OMLicense)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox OCBigLogo;
        private System.Windows.Forms.Label VerLabel;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Label CopyrightLabel;
        private System.Windows.Forms.PictureBox GitHubPage;
        private System.Windows.Forms.GroupBox DIGroup;
        private System.Windows.Forms.Label CurBranch;
        private System.Windows.Forms.Label BASSMIDIVer;
        private System.Windows.Forms.Label BASSVer;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolTip BranchToolTip;
        private System.Windows.Forms.Button ChangeBranch;
        private System.Windows.Forms.Label WinVer;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label WinName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label RAMAmount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button CheckForUpdates;
        private System.Windows.Forms.PictureBox OMLicense;
        private System.Windows.Forms.LinkLabel DonateKep;
        private System.Windows.Forms.LinkLabel DonateArd;
    }
}