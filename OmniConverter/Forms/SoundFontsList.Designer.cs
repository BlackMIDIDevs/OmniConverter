using System.Windows.Forms;

namespace OmniConverter
{
    partial class SoundFontsList
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
            this.SFList = new System.Windows.Forms.ListBox();
            this.SFSettings = new System.Windows.Forms.GroupBox();
            this.ApplySettings = new System.Windows.Forms.Button();
            this.XGM = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.DBLSB = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.DB = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.DP = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SB = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.SP = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.AddSF = new System.Windows.Forms.Button();
            this.RmvSF = new System.Windows.Forms.Button();
            this.MvD = new System.Windows.Forms.Button();
            this.MvU = new System.Windows.Forms.Button();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.RLCSFL = new System.Windows.Forms.Button();
            this.Enabled = new System.Windows.Forms.CheckBox();
            this.SFSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DBLSB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SP)).BeginInit();
            this.SuspendLayout();
            // 
            // SFList
            // 
            this.SFList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SFList.FormattingEnabled = true;
            this.SFList.HorizontalScrollbar = true;
            this.SFList.IntegralHeight = false;
            this.SFList.ItemHeight = 15;
            this.SFList.Location = new System.Drawing.Point(12, 12);
            this.SFList.Name = "SFList";
            this.SFList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.SFList.Size = new System.Drawing.Size(541, 364);
            this.SFList.TabIndex = 0;
            this.SFList.DrawMode = DrawMode.OwnerDrawFixed;
            this.SFList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.SFList_DrawItem);
            this.SFList.SelectedIndexChanged += new System.EventHandler(this.SFList_SelectedIndexChanged);
            this.SFList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SFList_KeyDown);
            // 
            // SFSettings
            // 
            this.SFSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SFSettings.Controls.Add(this.Enabled);
            this.SFSettings.Controls.Add(this.ApplySettings);
            this.SFSettings.Controls.Add(this.XGM);
            this.SFSettings.Controls.Add(this.label8);
            this.SFSettings.Controls.Add(this.DBLSB);
            this.SFSettings.Controls.Add(this.label7);
            this.SFSettings.Controls.Add(this.label4);
            this.SFSettings.Controls.Add(this.DB);
            this.SFSettings.Controls.Add(this.label5);
            this.SFSettings.Controls.Add(this.DP);
            this.SFSettings.Controls.Add(this.label6);
            this.SFSettings.Controls.Add(this.label3);
            this.SFSettings.Controls.Add(this.SB);
            this.SFSettings.Controls.Add(this.label2);
            this.SFSettings.Controls.Add(this.SP);
            this.SFSettings.Controls.Add(this.label1);
            this.SFSettings.Location = new System.Drawing.Point(559, 12);
            this.SFSettings.Name = "SFSettings";
            this.SFSettings.Size = new System.Drawing.Size(144, 365);
            this.SFSettings.TabIndex = 0;
            this.SFSettings.TabStop = false;
            this.SFSettings.Text = "Settings";
            // 
            // ApplySettings
            // 
            this.ApplySettings.Location = new System.Drawing.Point(5, 335);
            this.ApplySettings.Name = "ApplySettings";
            this.ApplySettings.Size = new System.Drawing.Size(134, 24);
            this.ApplySettings.TabIndex = 12;
            this.ApplySettings.Text = "Apply settings";
            this.ApplySettings.UseVisualStyleBackColor = true;
            this.ApplySettings.Click += new System.EventHandler(this.ApplySettings_Click);
            // 
            // XGM
            // 
            this.XGM.AutoSize = true;
            this.XGM.Location = new System.Drawing.Point(13, 252);
            this.XGM.Name = "XGM";
            this.XGM.Size = new System.Drawing.Size(122, 19);
            this.XGM.TabIndex = 11;
            this.XGM.Text = "Load XG bank first";
            this.XGM.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(6, 228);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(132, 15);
            this.label8.TabIndex = 12;
            this.label8.Text = "ADDITIONAL SET.";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DBLSB
            // 
            this.DBLSB.Location = new System.Drawing.Point(70, 190);
            this.DBLSB.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.DBLSB.Name = "DBLSB";
            this.DBLSB.Size = new System.Drawing.Size(49, 23);
            this.DBLSB.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(35, 192);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 15);
            this.label7.TabIndex = 10;
            this.label7.Text = "LSB:";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "DESTINATION";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DB
            // 
            this.DB.Location = new System.Drawing.Point(70, 161);
            this.DB.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.DB.Name = "DB";
            this.DB.Size = new System.Drawing.Size(49, 23);
            this.DB.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 163);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "Bank:";
            // 
            // DP
            // 
            this.DP.Location = new System.Drawing.Point(70, 132);
            this.DP.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.DP.Name = "DP";
            this.DP.Size = new System.Drawing.Size(49, 23);
            this.DP.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 134);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 15);
            this.label6.TabIndex = 5;
            this.label6.Text = "Preset:";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "SOURCE";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SB
            // 
            this.SB.Location = new System.Drawing.Point(70, 72);
            this.SB.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.SB.Name = "SB";
            this.SB.Size = new System.Drawing.Size(49, 23);
            this.SB.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Bank:";
            // 
            // SP
            // 
            this.SP.Location = new System.Drawing.Point(70, 43);
            this.SP.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.SP.Name = "SP";
            this.SP.Size = new System.Drawing.Size(49, 23);
            this.SP.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Preset:";
            // 
            // AddSF
            // 
            this.AddSF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddSF.Location = new System.Drawing.Point(12, 387);
            this.AddSF.Name = "AddSF";
            this.AddSF.Size = new System.Drawing.Size(130, 23);
            this.AddSF.TabIndex = 1;
            this.AddSF.Text = "Add SoundFont";
            this.AddSF.UseVisualStyleBackColor = true;
            this.AddSF.Click += new System.EventHandler(this.AddSF_Click);
            // 
            // RmvSF
            // 
            this.RmvSF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RmvSF.Location = new System.Drawing.Point(12, 411);
            this.RmvSF.Name = "RmvSF";
            this.RmvSF.Size = new System.Drawing.Size(130, 23);
            this.RmvSF.TabIndex = 2;
            this.RmvSF.Text = "Remove SoundFont";
            this.RmvSF.UseVisualStyleBackColor = true;
            this.RmvSF.Click += new System.EventHandler(this.RmvSF_Click);
            // 
            // MvD
            // 
            this.MvD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MvD.Location = new System.Drawing.Point(148, 411);
            this.MvD.Name = "MvD";
            this.MvD.Size = new System.Drawing.Size(130, 23);
            this.MvD.TabIndex = 4;
            this.MvD.Text = "Move down";
            this.MvD.UseVisualStyleBackColor = true;
            this.MvD.Click += new System.EventHandler(this.MvD_Click);
            // 
            // MvU
            // 
            this.MvU.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MvU.Location = new System.Drawing.Point(148, 387);
            this.MvU.Name = "MvU";
            this.MvU.Size = new System.Drawing.Size(130, 23);
            this.MvU.TabIndex = 3;
            this.MvU.Text = "Move up";
            this.MvU.UseVisualStyleBackColor = true;
            this.MvU.Click += new System.EventHandler(this.MvU_Click);
            // 
            // CloseBtn
            // 
            this.CloseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseBtn.Location = new System.Drawing.Point(628, 411);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(75, 23);
            this.CloseBtn.TabIndex = 13;
            this.CloseBtn.Text = "Close";
            this.CloseBtn.UseVisualStyleBackColor = true;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // RLCSFL
            // 
            this.RLCSFL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RLCSFL.Location = new System.Drawing.Point(284, 387);
            this.RLCSFL.Name = "RLCSFL";
            this.RLCSFL.Size = new System.Drawing.Size(130, 47);
            this.RLCSFL.TabIndex = 5;
            this.RLCSFL.Text = "(Re)Load Common SoundFonts list";
            this.RLCSFL.UseVisualStyleBackColor = true;
            this.RLCSFL.Click += new System.EventHandler(this.RLCSFL_Click);
            // 
            // Enabled
            // 
            this.Enabled.AutoSize = true;
            this.Enabled.Location = new System.Drawing.Point(13, 310);
            this.Enabled.Name = "Enabled";
            this.Enabled.Size = new System.Drawing.Size(68, 19);
            this.Enabled.TabIndex = 13;
            this.Enabled.Text = "Enabled";
            this.Enabled.UseVisualStyleBackColor = true;
            // 
            // SoundFontsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 446);
            this.Controls.Add(this.RLCSFL);
            this.Controls.Add(this.CloseBtn);
            this.Controls.Add(this.MvD);
            this.Controls.Add(this.MvU);
            this.Controls.Add(this.RmvSF);
            this.Controls.Add(this.AddSF);
            this.Controls.Add(this.SFSettings);
            this.Controls.Add(this.SFList);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimizeBox = false;
            this.Name = "SoundFontsList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SoundFonts list";
            this.Load += new System.EventHandler(this.SoundFontsList_Load);
            this.SFSettings.ResumeLayout(false);
            this.SFSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DBLSB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SP)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox SFList;
        private System.Windows.Forms.GroupBox SFSettings;
        private System.Windows.Forms.Button ApplySettings;
        private System.Windows.Forms.CheckBox XGM;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown DBLSB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown DB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown DP;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown SB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown SP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button AddSF;
        private System.Windows.Forms.Button RmvSF;
        private System.Windows.Forms.Button MvD;
        private System.Windows.Forms.Button MvU;
        private System.Windows.Forms.Button CloseBtn;
        private System.Windows.Forms.Button RLCSFL;
        private System.Windows.Forms.CheckBox Enabled;
    }
}