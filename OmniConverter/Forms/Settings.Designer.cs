namespace OmniConverter
{
    partial class Settings
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
            this.AudioSettings = new System.Windows.Forms.GroupBox();
            this.EnableLoudMax = new System.Windows.Forms.CheckBox();
            this.NoteOff1 = new System.Windows.Forms.CheckBox();
            this.FXDisable = new System.Windows.Forms.CheckBox();
            this.SincInter = new System.Windows.Forms.CheckBox();
            this.MaxVoicesLabel = new System.Windows.Forms.Label();
            this.MaxVoices = new System.Windows.Forms.NumericUpDown();
            this.AudioFreqLabel = new System.Windows.Forms.Label();
            this.FrequencyBox = new System.Windows.Forms.ComboBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.EventsSettings = new System.Windows.Forms.GroupBox();
            this.MTLimitVal = new System.Windows.Forms.NumericUpDown();
            this.MTLimitLab = new System.Windows.Forms.Label();
            this.MTLimit = new System.Windows.Forms.CheckBox();
            this.PerTrackMode = new System.Windows.Forms.CheckBox();
            this.AOFBrowse = new System.Windows.Forms.Button();
            this.AOFPath = new System.Windows.Forms.TextBox();
            this.AutoOutputFolder = new System.Windows.Forms.CheckBox();
            this.PerTrackStorage = new System.Windows.Forms.CheckBox();
            this.PerTrackExportEach = new System.Windows.Forms.CheckBox();
            this.MTMode = new System.Windows.Forms.CheckBox();
            this.OkBtn = new System.Windows.Forms.Button();
            this.PostConvSettings = new System.Windows.Forms.GroupBox();
            this.DoActionAfterRenderVal = new System.Windows.Forms.ComboBox();
            this.DoActionAfterRender = new System.Windows.Forms.CheckBox();
            this.AudioSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxVoices)).BeginInit();
            this.EventsSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MTLimitVal)).BeginInit();
            this.PostConvSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // AudioSettings
            // 
            this.AudioSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AudioSettings.Controls.Add(this.EnableLoudMax);
            this.AudioSettings.Controls.Add(this.NoteOff1);
            this.AudioSettings.Controls.Add(this.FXDisable);
            this.AudioSettings.Controls.Add(this.SincInter);
            this.AudioSettings.Controls.Add(this.MaxVoicesLabel);
            this.AudioSettings.Controls.Add(this.MaxVoices);
            this.AudioSettings.Controls.Add(this.AudioFreqLabel);
            this.AudioSettings.Controls.Add(this.FrequencyBox);
            this.AudioSettings.Controls.Add(this.Label6);
            this.AudioSettings.Location = new System.Drawing.Point(14, 14);
            this.AudioSettings.Name = "AudioSettings";
            this.AudioSettings.Size = new System.Drawing.Size(430, 187);
            this.AudioSettings.TabIndex = 0;
            this.AudioSettings.TabStop = false;
            this.AudioSettings.Text = "Audio settings";
            // 
            // EnableLoudMax
            // 
            this.EnableLoudMax.AutoSize = true;
            this.EnableLoudMax.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.EnableLoudMax.Location = new System.Drawing.Point(10, 159);
            this.EnableLoudMax.Name = "EnableLoudMax";
            this.EnableLoudMax.Size = new System.Drawing.Size(276, 20);
            this.EnableLoudMax.TabIndex = 44;
            this.EnableLoudMax.Text = "Enable audio limiter, to prevent audio clipping";
            this.EnableLoudMax.UseVisualStyleBackColor = true;
            // 
            // NoteOff1
            // 
            this.NoteOff1.AutoSize = true;
            this.NoteOff1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.NoteOff1.Location = new System.Drawing.Point(10, 137);
            this.NoteOff1.Name = "NoteOff1";
            this.NoteOff1.Size = new System.Drawing.Size(358, 20);
            this.NoteOff1.TabIndex = 43;
            this.NoteOff1.Text = "Only release the oldest instance of a note upon note-off event";
            this.NoteOff1.UseVisualStyleBackColor = true;
            // 
            // FXDisable
            // 
            this.FXDisable.AutoSize = true;
            this.FXDisable.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.FXDisable.Location = new System.Drawing.Point(10, 115);
            this.FXDisable.Name = "FXDisable";
            this.FXDisable.Size = new System.Drawing.Size(255, 20);
            this.FXDisable.TabIndex = 42;
            this.FXDisable.Text = "Disable sound effects (Reverb, chorus etc.)";
            this.FXDisable.UseVisualStyleBackColor = true;
            // 
            // SincInter
            // 
            this.SincInter.AutoSize = true;
            this.SincInter.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.SincInter.Location = new System.Drawing.Point(10, 93);
            this.SincInter.Name = "SincInter";
            this.SincInter.Size = new System.Drawing.Size(162, 20);
            this.SincInter.TabIndex = 41;
            this.SincInter.Text = "Enable sinc interpolation";
            this.SincInter.UseVisualStyleBackColor = true;
            // 
            // MaxVoicesLabel
            // 
            this.MaxVoicesLabel.AutoSize = true;
            this.MaxVoicesLabel.Location = new System.Drawing.Point(7, 25);
            this.MaxVoicesLabel.Name = "MaxVoicesLabel";
            this.MaxVoicesLabel.Size = new System.Drawing.Size(103, 15);
            this.MaxVoicesLabel.TabIndex = 40;
            this.MaxVoicesLabel.Text = "Active voices limit";
            // 
            // MaxVoices
            // 
            this.MaxVoices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MaxVoices.Location = new System.Drawing.Point(354, 23);
            this.MaxVoices.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.MaxVoices.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MaxVoices.Name = "MaxVoices";
            this.MaxVoices.Size = new System.Drawing.Size(64, 23);
            this.MaxVoices.TabIndex = 39;
            this.MaxVoices.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MaxVoices.Value = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            // 
            // AudioFreqLabel
            // 
            this.AudioFreqLabel.AutoSize = true;
            this.AudioFreqLabel.Location = new System.Drawing.Point(7, 60);
            this.AudioFreqLabel.Name = "AudioFreqLabel";
            this.AudioFreqLabel.Size = new System.Drawing.Size(134, 15);
            this.AudioFreqLabel.TabIndex = 37;
            this.AudioFreqLabel.Text = "Output audio frequency";
            // 
            // FrequencyBox
            // 
            this.FrequencyBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FrequencyBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FrequencyBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.FrequencyBox.FormattingEnabled = true;
            this.FrequencyBox.Items.AddRange(new object[] {
            "192000",
            "176400",
            "142180",
            "96000",
            "88200",
            "74750",
            "66150",
            "50400",
            "50000",
            "48000",
            "47250",
            "44100",
            "44056",
            "37800",
            "32000",
            "22050",
            "16000",
            "11025",
            "8000",
            "4000"});
            this.FrequencyBox.Location = new System.Drawing.Point(333, 55);
            this.FrequencyBox.Name = "FrequencyBox";
            this.FrequencyBox.Size = new System.Drawing.Size(68, 23);
            this.FrequencyBox.TabIndex = 36;
            // 
            // Label6
            // 
            this.Label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(400, 60);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(21, 15);
            this.Label6.TabIndex = 38;
            this.Label6.Text = "Hz";
            // 
            // EventsSettings
            // 
            this.EventsSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EventsSettings.Controls.Add(this.MTLimitVal);
            this.EventsSettings.Controls.Add(this.MTLimitLab);
            this.EventsSettings.Controls.Add(this.MTLimit);
            this.EventsSettings.Controls.Add(this.PerTrackMode);
            this.EventsSettings.Controls.Add(this.AOFBrowse);
            this.EventsSettings.Controls.Add(this.AOFPath);
            this.EventsSettings.Controls.Add(this.AutoOutputFolder);
            this.EventsSettings.Controls.Add(this.PerTrackStorage);
            this.EventsSettings.Controls.Add(this.PerTrackExportEach);
            this.EventsSettings.Controls.Add(this.MTMode);
            this.EventsSettings.Location = new System.Drawing.Point(14, 208);
            this.EventsSettings.Name = "EventsSettings";
            this.EventsSettings.Size = new System.Drawing.Size(430, 205);
            this.EventsSettings.TabIndex = 1;
            this.EventsSettings.TabStop = false;
            this.EventsSettings.Text = "Export settings";
            // 
            // MTLimitVal
            // 
            this.MTLimitVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MTLimitVal.Location = new System.Drawing.Point(370, 118);
            this.MTLimitVal.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MTLimitVal.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MTLimitVal.Name = "MTLimitVal";
            this.MTLimitVal.Size = new System.Drawing.Size(48, 23);
            this.MTLimitVal.TabIndex = 45;
            this.MTLimitVal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MTLimitVal.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // MTLimitLab
            // 
            this.MTLimitLab.AutoSize = true;
            this.MTLimitLab.Location = new System.Drawing.Point(319, 120);
            this.MTLimitLab.Name = "MTLimitLab";
            this.MTLimitLab.Size = new System.Drawing.Size(51, 15);
            this.MTLimitLab.TabIndex = 52;
            this.MTLimitLab.Text = "Threads:";
            // 
            // MTLimit
            // 
            this.MTLimit.AutoSize = true;
            this.MTLimit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MTLimit.Location = new System.Drawing.Point(10, 118);
            this.MTLimit.Name = "MTLimit";
            this.MTLimit.Size = new System.Drawing.Size(247, 20);
            this.MTLimit.TabIndex = 51;
            this.MTLimit.Text = "Use a custom threads limit for the export";
            this.MTLimit.UseVisualStyleBackColor = true;
            this.MTLimit.CheckedChanged += new System.EventHandler(this.MTLimit_CheckedChanged);
            // 
            // PerTrackMode
            // 
            this.PerTrackMode.AutoSize = true;
            this.PerTrackMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.PerTrackMode.Location = new System.Drawing.Point(10, 44);
            this.PerTrackMode.Name = "PerTrackMode";
            this.PerTrackMode.Size = new System.Drawing.Size(423, 20);
            this.PerTrackMode.TabIndex = 50;
            this.PerTrackMode.Text = "Render each track of a MIDI separately and merge them into one audio file";
            this.PerTrackMode.UseVisualStyleBackColor = true;
            this.PerTrackMode.CheckedChanged += new System.EventHandler(this.PerTrackMode_CheckedChanged);
            // 
            // AOFBrowse
            // 
            this.AOFBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AOFBrowse.Location = new System.Drawing.Point(333, 170);
            this.AOFBrowse.Name = "AOFBrowse";
            this.AOFBrowse.Size = new System.Drawing.Size(87, 25);
            this.AOFBrowse.TabIndex = 49;
            this.AOFBrowse.Text = "Browse...";
            this.AOFBrowse.UseVisualStyleBackColor = true;
            this.AOFBrowse.Click += new System.EventHandler(this.AOFBrowse_Click);
            // 
            // AOFPath
            // 
            this.AOFPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AOFPath.Location = new System.Drawing.Point(10, 171);
            this.AOFPath.Name = "AOFPath";
            this.AOFPath.Size = new System.Drawing.Size(317, 23);
            this.AOFPath.TabIndex = 48;
            // 
            // AutoOutputFolder
            // 
            this.AutoOutputFolder.AutoSize = true;
            this.AutoOutputFolder.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.AutoOutputFolder.Location = new System.Drawing.Point(10, 148);
            this.AutoOutputFolder.Name = "AutoOutputFolder";
            this.AutoOutputFolder.Size = new System.Drawing.Size(358, 20);
            this.AutoOutputFolder.TabIndex = 47;
            this.AutoOutputFolder.Text = "Export MIDIs directly to this folder instead of asking everytime";
            this.AutoOutputFolder.UseVisualStyleBackColor = true;
            this.AutoOutputFolder.CheckedChanged += new System.EventHandler(this.AutoOutputFolder_CheckedChanged);
            // 
            // PerTrackStorage
            // 
            this.PerTrackStorage.AutoSize = true;
            this.PerTrackStorage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.PerTrackStorage.Location = new System.Drawing.Point(10, 88);
            this.PerTrackStorage.Name = "PerTrackStorage";
            this.PerTrackStorage.Size = new System.Drawing.Size(405, 20);
            this.PerTrackStorage.TabIndex = 46;
            this.PerTrackStorage.Text = "Store each exported track into a folder with the same name as the MIDI";
            this.PerTrackStorage.UseVisualStyleBackColor = true;
            // 
            // PerTrackExportEach
            // 
            this.PerTrackExportEach.AutoSize = true;
            this.PerTrackExportEach.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.PerTrackExportEach.Location = new System.Drawing.Point(10, 66);
            this.PerTrackExportEach.Name = "PerTrackExportEach";
            this.PerTrackExportEach.Size = new System.Drawing.Size(388, 20);
            this.PerTrackExportEach.TabIndex = 45;
            this.PerTrackExportEach.Text = "Export each track separately instead of merging them automatically";
            this.PerTrackExportEach.UseVisualStyleBackColor = true;
            this.PerTrackExportEach.CheckedChanged += new System.EventHandler(this.PerTrackExportEach_CheckedChanged);
            // 
            // MTMode
            // 
            this.MTMode.AutoSize = true;
            this.MTMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MTMode.Location = new System.Drawing.Point(10, 22);
            this.MTMode.Name = "MTMode";
            this.MTMode.Size = new System.Drawing.Size(215, 20);
            this.MTMode.TabIndex = 44;
            this.MTMode.Text = "Enable multi-threaded MIDI export";
            this.MTMode.UseVisualStyleBackColor = true;
            this.MTMode.CheckedChanged += new System.EventHandler(this.MTMode_CheckedChanged);
            // 
            // OkBtn
            // 
            this.OkBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OkBtn.Location = new System.Drawing.Point(370, 510);
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Size = new System.Drawing.Size(75, 23);
            this.OkBtn.TabIndex = 3;
            this.OkBtn.Text = "OK";
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // PostConvSettings
            // 
            this.PostConvSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PostConvSettings.Controls.Add(this.DoActionAfterRenderVal);
            this.PostConvSettings.Controls.Add(this.DoActionAfterRender);
            this.PostConvSettings.Location = new System.Drawing.Point(14, 419);
            this.PostConvSettings.Name = "PostConvSettings";
            this.PostConvSettings.Size = new System.Drawing.Size(430, 79);
            this.PostConvSettings.TabIndex = 4;
            this.PostConvSettings.TabStop = false;
            this.PostConvSettings.Text = "Post-conversion settings";
            // 
            // DoActionAfterRenderVal
            // 
            this.DoActionAfterRenderVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DoActionAfterRenderVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DoActionAfterRenderVal.FormattingEnabled = true;
            this.DoActionAfterRenderVal.Items.AddRange(new object[] {
            "Put the computer into sleep mode",
            "Put the computer into hibernation mode",
            "Turn the computer off",
            "Restart the computer"});
            this.DoActionAfterRenderVal.Location = new System.Drawing.Point(10, 45);
            this.DoActionAfterRenderVal.Name = "DoActionAfterRenderVal";
            this.DoActionAfterRenderVal.Size = new System.Drawing.Size(410, 23);
            this.DoActionAfterRenderVal.TabIndex = 1;
            // 
            // DoActionAfterRender
            // 
            this.DoActionAfterRender.AutoSize = true;
            this.DoActionAfterRender.Location = new System.Drawing.Point(10, 22);
            this.DoActionAfterRender.Name = "DoActionAfterRender";
            this.DoActionAfterRender.Size = new System.Drawing.Size(391, 19);
            this.DoActionAfterRender.TabIndex = 0;
            this.DoActionAfterRender.Text = "Do one of the following actions once the computer is done rendering";
            this.DoActionAfterRender.UseVisualStyleBackColor = true;
            this.DoActionAfterRender.CheckedChanged += new System.EventHandler(this.DoActionAfterRender_CheckedChanged);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 546);
            this.ControlBox = false;
            this.Controls.Add(this.PostConvSettings);
            this.Controls.Add(this.OkBtn);
            this.Controls.Add(this.EventsSettings);
            this.Controls.Add(this.AudioSettings);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.AudioSettings.ResumeLayout(false);
            this.AudioSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxVoices)).EndInit();
            this.EventsSettings.ResumeLayout(false);
            this.EventsSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MTLimitVal)).EndInit();
            this.PostConvSettings.ResumeLayout(false);
            this.PostConvSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox AudioSettings;
        internal System.Windows.Forms.CheckBox SincInter;
        internal System.Windows.Forms.Label MaxVoicesLabel;
        public System.Windows.Forms.NumericUpDown MaxVoices;
        internal System.Windows.Forms.Label AudioFreqLabel;
        internal System.Windows.Forms.ComboBox FrequencyBox;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.CheckBox EnableLoudMax;
        internal System.Windows.Forms.CheckBox NoteOff1;
        internal System.Windows.Forms.CheckBox FXDisable;
        private System.Windows.Forms.GroupBox EventsSettings;
        private System.Windows.Forms.Button AOFBrowse;
        private System.Windows.Forms.TextBox AOFPath;
        internal System.Windows.Forms.CheckBox AutoOutputFolder;
        internal System.Windows.Forms.CheckBox PerTrackStorage;
        internal System.Windows.Forms.CheckBox PerTrackExportEach;
        internal System.Windows.Forms.CheckBox MTMode;
        private System.Windows.Forms.Button OkBtn;
        internal System.Windows.Forms.CheckBox PerTrackMode;
        public System.Windows.Forms.NumericUpDown MTLimitVal;
        private System.Windows.Forms.Label MTLimitLab;
        internal System.Windows.Forms.CheckBox MTLimit;
        private System.Windows.Forms.GroupBox PostConvSettings;
        private System.Windows.Forms.ComboBox DoActionAfterRenderVal;
        private System.Windows.Forms.CheckBox DoActionAfterRender;
    }
}