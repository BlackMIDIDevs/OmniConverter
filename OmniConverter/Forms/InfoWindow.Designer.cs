namespace OmniConverter
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
            components = new System.ComponentModel.Container();
            OCBigLogo = new System.Windows.Forms.PictureBox();
            VerLabel = new System.Windows.Forms.Label();
            OKBtn = new System.Windows.Forms.Button();
            CopyrightLabel = new System.Windows.Forms.Label();
            GitHubPage = new System.Windows.Forms.PictureBox();
            DIGroup = new System.Windows.Forms.GroupBox();
            CurBranch = new System.Windows.Forms.Label();
            BASSMIDIVer = new System.Windows.Forms.Label();
            BASSVer = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            RAMAmount = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            WinVer = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            WinName = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            BranchToolTip = new System.Windows.Forms.ToolTip(components);
            ChangeBranch = new System.Windows.Forms.Button();
            CheckForUpdates = new System.Windows.Forms.Button();
            OMLicense = new System.Windows.Forms.PictureBox();
            DonateKep = new System.Windows.Forms.LinkLabel();
            DonateArd = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)OCBigLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)GitHubPage).BeginInit();
            DIGroup.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)OMLicense).BeginInit();
            SuspendLayout();
            // 
            // OCBigLogo
            // 
            OCBigLogo.Location = new System.Drawing.Point(13, 14);
            OCBigLogo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            OCBigLogo.Name = "OCBigLogo";
            OCBigLogo.Size = new System.Drawing.Size(299, 295);
            OCBigLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            OCBigLogo.TabIndex = 0;
            OCBigLogo.TabStop = false;
            // 
            // VerLabel
            // 
            VerLabel.AutoSize = true;
            VerLabel.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            VerLabel.Location = new System.Drawing.Point(318, 14);
            VerLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            VerLabel.Name = "VerLabel";
            VerLabel.Size = new System.Drawing.Size(101, 24);
            VerLabel.TabIndex = 1;
            VerLabel.Text = "Template";
            VerLabel.Click += VerLabel_Click;
            // 
            // OKBtn
            // 
            OKBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            OKBtn.Location = new System.Drawing.Point(760, 282);
            OKBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            OKBtn.Name = "OKBtn";
            OKBtn.Size = new System.Drawing.Size(88, 28);
            OKBtn.TabIndex = 4;
            OKBtn.Text = "OK";
            OKBtn.UseVisualStyleBackColor = true;
            OKBtn.Click += OKBtn_Click;
            // 
            // CopyrightLabel
            // 
            CopyrightLabel.Location = new System.Drawing.Point(320, 54);
            CopyrightLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            CopyrightLabel.Name = "CopyrightLabel";
            CopyrightLabel.Size = new System.Drawing.Size(317, 30);
            CopyrightLabel.TabIndex = 3;
            CopyrightLabel.Text = "Copyright Ⓒ 2019-{0} Keppy's Software and Arduano\r\nFree MIDI converter for Windows 7 and newer";
            // 
            // GitHubPage
            // 
            GitHubPage.Location = new System.Drawing.Point(819, 14);
            GitHubPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GitHubPage.Name = "GitHubPage";
            GitHubPage.Size = new System.Drawing.Size(28, 28);
            GitHubPage.TabIndex = 5;
            GitHubPage.TabStop = false;
            GitHubPage.Click += GitHubPage_Click;
            GitHubPage.MouseHover += GitHubPage_MouseHover;
            // 
            // DIGroup
            // 
            DIGroup.Controls.Add(CurBranch);
            DIGroup.Controls.Add(BASSMIDIVer);
            DIGroup.Controls.Add(BASSVer);
            DIGroup.Controls.Add(label9);
            DIGroup.Controls.Add(label5);
            DIGroup.Controls.Add(label4);
            DIGroup.Location = new System.Drawing.Point(323, 159);
            DIGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            DIGroup.Name = "DIGroup";
            DIGroup.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            DIGroup.Size = new System.Drawing.Size(257, 115);
            DIGroup.TabIndex = 6;
            DIGroup.TabStop = false;
            DIGroup.Text = "Driver information";
            // 
            // CurBranch
            // 
            CurBranch.AutoSize = true;
            CurBranch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            CurBranch.ForeColor = System.Drawing.SystemColors.ControlText;
            CurBranch.Location = new System.Drawing.Point(122, 88);
            CurBranch.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            CurBranch.Name = "CurBranch";
            CurBranch.Size = new System.Drawing.Size(74, 13);
            CurBranch.TabIndex = 12;
            CurBranch.Text = "Unavailable";
            // 
            // BASSMIDIVer
            // 
            BASSMIDIVer.AutoSize = true;
            BASSMIDIVer.Location = new System.Drawing.Point(122, 44);
            BASSMIDIVer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            BASSMIDIVer.Name = "BASSMIDIVer";
            BASSMIDIVer.Size = new System.Drawing.Size(52, 15);
            BASSMIDIVer.TabIndex = 11;
            BASSMIDIVer.Text = "LIB VER2";
            // 
            // BASSVer
            // 
            BASSVer.AutoSize = true;
            BASSVer.Location = new System.Drawing.Point(122, 22);
            BASSVer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            BASSVer.Name = "BASSVer";
            BASSVer.Size = new System.Drawing.Size(52, 15);
            BASSVer.TabIndex = 10;
            BASSVer.Text = "LIB VER1";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(7, 88);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(88, 15);
            label9.TabIndex = 9;
            label9.Text = "Update branch:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(7, 44);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(103, 15);
            label5.TabIndex = 5;
            label5.Text = "BASSMIDI version:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(7, 22);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(78, 15);
            label4.TabIndex = 4;
            label4.Text = "BASS version:";
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(320, 100);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(527, 48);
            label1.TabIndex = 7;
            label1.Text = "This software is open-source.\r\nRedistribution and use of this code or any derivative works are permitted provided that specific conditions are met. Click the blue note button to see the license.\r\n";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(RAMAmount);
            groupBox1.Controls.Add(label11);
            groupBox1.Controls.Add(WinVer);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(WinName);
            groupBox1.Controls.Add(label8);
            groupBox1.Location = new System.Drawing.Point(590, 159);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(257, 115);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Windows install information";
            // 
            // RAMAmount
            // 
            RAMAmount.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            RAMAmount.AutoSize = true;
            RAMAmount.Location = new System.Drawing.Point(59, 88);
            RAMAmount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            RAMAmount.Name = "RAMAmount";
            RAMAmount.Size = new System.Drawing.Size(87, 15);
            RAMAmount.TabIndex = 16;
            RAMAmount.Text = "RAM AMOUNT";
            // 
            // label11
            // 
            label11.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(7, 88);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(36, 15);
            label11.TabIndex = 14;
            label11.Text = "RAM:";
            // 
            // WinVer
            // 
            WinVer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            WinVer.Location = new System.Drawing.Point(59, 44);
            WinVer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            WinVer.Name = "WinVer";
            WinVer.Size = new System.Drawing.Size(190, 37);
            WinVer.TabIndex = 13;
            WinVer.Text = "WIN VERS";
            // 
            // label7
            // 
            label7.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(7, 44);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(48, 15);
            label7.TabIndex = 11;
            label7.Text = "Version:";
            // 
            // WinName
            // 
            WinName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            WinName.Location = new System.Drawing.Point(59, 22);
            WinName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            WinName.Name = "WinName";
            WinName.Size = new System.Drawing.Size(190, 15);
            WinName.TabIndex = 12;
            WinName.Text = "WIN NAME";
            // 
            // label8
            // 
            label8.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(7, 22);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(42, 15);
            label8.TabIndex = 10;
            label8.Text = "Name:";
            // 
            // BranchToolTip
            // 
            BranchToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            BranchToolTip.ToolTipTitle = "Branch info";
            // 
            // ChangeBranch
            // 
            ChangeBranch.Location = new System.Drawing.Point(323, 282);
            ChangeBranch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ChangeBranch.Name = "ChangeBranch";
            ChangeBranch.Size = new System.Drawing.Size(118, 28);
            ChangeBranch.TabIndex = 2;
            ChangeBranch.Text = "Change branch";
            ChangeBranch.UseVisualStyleBackColor = true;
            ChangeBranch.Click += ChangeBranch_Click;
            // 
            // CheckForUpdates
            // 
            CheckForUpdates.Location = new System.Drawing.Point(628, 282);
            CheckForUpdates.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CheckForUpdates.Name = "CheckForUpdates";
            CheckForUpdates.Size = new System.Drawing.Size(125, 28);
            CheckForUpdates.TabIndex = 3;
            CheckForUpdates.Text = "Check for updates";
            CheckForUpdates.UseVisualStyleBackColor = true;
            CheckForUpdates.Click += CheckForUpdates_Click;
            // 
            // OMLicense
            // 
            OMLicense.Cursor = System.Windows.Forms.Cursors.Hand;
            OMLicense.Location = new System.Drawing.Point(784, 14);
            OMLicense.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            OMLicense.Name = "OMLicense";
            OMLicense.Size = new System.Drawing.Size(28, 28);
            OMLicense.TabIndex = 10;
            OMLicense.TabStop = false;
            OMLicense.Click += OMLicense_Click;
            OMLicense.MouseHover += OMLicense_MouseHover;
            // 
            // DonateKep
            // 
            DonateKep.AutoSize = true;
            DonateKep.LinkColor = System.Drawing.SystemColors.Highlight;
            DonateKep.Location = new System.Drawing.Point(696, 54);
            DonateKep.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            DonateKep.Name = "DonateKep";
            DonateKep.Size = new System.Drawing.Size(152, 15);
            DonateKep.TabIndex = 11;
            DonateKep.TabStop = true;
            DonateKep.Text = "Donate to Keppy's Software";
            DonateKep.LinkClicked += DonateKep_LinkClicked;
            // 
            // DonateArd
            // 
            DonateArd.AutoSize = true;
            DonateArd.LinkColor = System.Drawing.SystemColors.Highlight;
            DonateArd.Location = new System.Drawing.Point(740, 69);
            DonateArd.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            DonateArd.Name = "DonateArd";
            DonateArd.Size = new System.Drawing.Size(108, 15);
            DonateArd.TabIndex = 12;
            DonateArd.TabStop = true;
            DonateArd.Text = "Donate to Arduano";
            DonateArd.LinkClicked += DonateArd_LinkClicked;
            // 
            // InfoWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = OKBtn;
            ClientSize = new System.Drawing.Size(861, 323);
            Controls.Add(DonateArd);
            Controls.Add(DonateKep);
            Controls.Add(OMLicense);
            Controls.Add(CheckForUpdates);
            Controls.Add(ChangeBranch);
            Controls.Add(groupBox1);
            Controls.Add(label1);
            Controls.Add(DIGroup);
            Controls.Add(GitHubPage);
            Controls.Add(CopyrightLabel);
            Controls.Add(OKBtn);
            Controls.Add(VerLabel);
            Controls.Add(OCBigLogo);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InfoWindow";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Information";
            Load += InfoWindow_Load;
            ((System.ComponentModel.ISupportInitialize)OCBigLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)GitHubPage).EndInit();
            DIGroup.ResumeLayout(false);
            DIGroup.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)OMLicense).EndInit();
            ResumeLayout(false);
            PerformLayout();
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