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
            groupBox1 = new System.Windows.Forms.GroupBox();
            ChorusV = new System.Windows.Forms.NumericUpDown();
            ChorusL = new System.Windows.Forms.Label();
            ReverbV = new System.Windows.Forms.NumericUpDown();
            ReverbL = new System.Windows.Forms.Label();
            EnableRCOverride = new System.Windows.Forms.CheckBox();
            EnableLoudMax = new System.Windows.Forms.CheckBox();
            NoteOff1 = new System.Windows.Forms.CheckBox();
            FXDisable = new System.Windows.Forms.CheckBox();
            SincInter = new System.Windows.Forms.CheckBox();
            MaxVoicesLabel = new System.Windows.Forms.Label();
            MaxVoices = new System.Windows.Forms.NumericUpDown();
            AudioFreqLabel = new System.Windows.Forms.Label();
            FrequencyBox = new System.Windows.Forms.ComboBox();
            Label6 = new System.Windows.Forms.Label();
            MTLimitVal = new System.Windows.Forms.NumericUpDown();
            MTLimitLab = new System.Windows.Forms.Label();
            MTLimit = new System.Windows.Forms.CheckBox();
            PerTrackMode = new System.Windows.Forms.CheckBox();
            AOFPath = new System.Windows.Forms.TextBox();
            AutoOutputFolder = new System.Windows.Forms.CheckBox();
            PerTrackStorage = new System.Windows.Forms.CheckBox();
            PerTrackExportEach = new System.Windows.Forms.CheckBox();
            MTMode = new System.Windows.Forms.CheckBox();
            OkBtn = new System.Windows.Forms.Button();
            DoActionAfterRenderVal = new System.Windows.Forms.ComboBox();
            DoActionAfterRender = new System.Windows.Forms.CheckBox();
            tabControl1 = new System.Windows.Forms.TabControl();
            ASet = new System.Windows.Forms.TabPage();
            groupBox2 = new System.Windows.Forms.GroupBox();
            RealTimeSimulation = new System.Windows.Forms.CheckBox();
            frameFluctValue = new System.Windows.Forms.NumericUpDown();
            fpsLab = new System.Windows.Forms.Label();
            frameFluctLab = new System.Windows.Forms.Label();
            fpsValue = new System.Windows.Forms.NumericUpDown();
            UnlimitedVoices = new System.Windows.Forms.CheckBox();
            ESet = new System.Windows.Forms.TabPage();
            AOFBrowse = new System.Windows.Forms.Button();
            PCSet = new System.Windows.Forms.TabPage();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ChorusV).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ReverbV).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MaxVoices).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MTLimitVal).BeginInit();
            tabControl1.SuspendLayout();
            ASet.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)frameFluctValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)fpsValue).BeginInit();
            ESet.SuspendLayout();
            PCSet.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(ChorusV);
            groupBox1.Controls.Add(ChorusL);
            groupBox1.Controls.Add(ReverbV);
            groupBox1.Controls.Add(ReverbL);
            groupBox1.Controls.Add(EnableRCOverride);
            groupBox1.Location = new System.Drawing.Point(6, 151);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(442, 81);
            groupBox1.TabIndex = 46;
            groupBox1.TabStop = false;
            groupBox1.Text = "Control Event Override";
            // 
            // ChorusV
            // 
            ChorusV.Enabled = false;
            ChorusV.Location = new System.Drawing.Point(218, 46);
            ChorusV.Maximum = new decimal(new int[] { 127, 0, 0, 0 });
            ChorusV.Name = "ChorusV";
            ChorusV.Size = new System.Drawing.Size(39, 23);
            ChorusV.TabIndex = 49;
            // 
            // ChorusL
            // 
            ChorusL.AutoSize = true;
            ChorusL.Enabled = false;
            ChorusL.Location = new System.Drawing.Point(131, 49);
            ChorusL.Name = "ChorusL";
            ChorusL.Size = new System.Drawing.Size(85, 15);
            ChorusL.TabIndex = 48;
            ChorusL.Text = "Chorus (0-127)";
            // 
            // ReverbV
            // 
            ReverbV.Enabled = false;
            ReverbV.Location = new System.Drawing.Point(91, 46);
            ReverbV.Maximum = new decimal(new int[] { 127, 0, 0, 0 });
            ReverbV.Name = "ReverbV";
            ReverbV.Size = new System.Drawing.Size(39, 23);
            ReverbV.TabIndex = 47;
            // 
            // ReverbL
            // 
            ReverbL.AutoSize = true;
            ReverbL.BackColor = System.Drawing.Color.Transparent;
            ReverbL.Enabled = false;
            ReverbL.Location = new System.Drawing.Point(4, 49);
            ReverbL.Name = "ReverbL";
            ReverbL.Size = new System.Drawing.Size(83, 15);
            ReverbL.TabIndex = 46;
            ReverbL.Text = "Reverb (0-127)";
            // 
            // EnableRCOverride
            // 
            EnableRCOverride.AutoSize = true;
            EnableRCOverride.FlatStyle = System.Windows.Forms.FlatStyle.System;
            EnableRCOverride.Location = new System.Drawing.Point(6, 22);
            EnableRCOverride.Name = "EnableRCOverride";
            EnableRCOverride.Size = new System.Drawing.Size(248, 20);
            EnableRCOverride.TabIndex = 45;
            EnableRCOverride.Text = "Override reverb and chorus in the render:";
            EnableRCOverride.UseVisualStyleBackColor = true;
            EnableRCOverride.CheckedChanged += EnableRCOverride_CheckedChanged;
            // 
            // EnableLoudMax
            // 
            EnableLoudMax.AutoSize = true;
            EnableLoudMax.FlatStyle = System.Windows.Forms.FlatStyle.System;
            EnableLoudMax.Location = new System.Drawing.Point(6, 126);
            EnableLoudMax.Name = "EnableLoudMax";
            EnableLoudMax.Size = new System.Drawing.Size(235, 20);
            EnableLoudMax.TabIndex = 44;
            EnableLoudMax.Text = "Limit audio to 0dB, to prevent clipping";
            EnableLoudMax.UseVisualStyleBackColor = true;
            // 
            // NoteOff1
            // 
            NoteOff1.AutoSize = true;
            NoteOff1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            NoteOff1.Location = new System.Drawing.Point(6, 104);
            NoteOff1.Name = "NoteOff1";
            NoteOff1.Size = new System.Drawing.Size(358, 20);
            NoteOff1.TabIndex = 43;
            NoteOff1.Text = "Only release the oldest instance of a note upon note-off event";
            NoteOff1.UseVisualStyleBackColor = true;
            // 
            // FXDisable
            // 
            FXDisable.AutoSize = true;
            FXDisable.FlatStyle = System.Windows.Forms.FlatStyle.System;
            FXDisable.Location = new System.Drawing.Point(6, 82);
            FXDisable.Name = "FXDisable";
            FXDisable.Size = new System.Drawing.Size(241, 20);
            FXDisable.TabIndex = 42;
            FXDisable.Text = "Disable Sound FXs (Reverb, Chorus etc.)";
            FXDisable.UseVisualStyleBackColor = true;
            // 
            // SincInter
            // 
            SincInter.AutoSize = true;
            SincInter.FlatStyle = System.Windows.Forms.FlatStyle.System;
            SincInter.Location = new System.Drawing.Point(6, 60);
            SincInter.Name = "SincInter";
            SincInter.Size = new System.Drawing.Size(341, 20);
            SincInter.TabIndex = 41;
            SincInter.Text = "Enable Whittaker–Shannon audio interpolation (Sinc inter.)";
            SincInter.UseVisualStyleBackColor = true;
            // 
            // MaxVoicesLabel
            // 
            MaxVoicesLabel.AutoSize = true;
            MaxVoicesLabel.Location = new System.Drawing.Point(6, 8);
            MaxVoicesLabel.Name = "MaxVoicesLabel";
            MaxVoicesLabel.Size = new System.Drawing.Size(62, 15);
            MaxVoicesLabel.TabIndex = 40;
            MaxVoicesLabel.Text = "Voice limit";
            // 
            // MaxVoices
            // 
            MaxVoices.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            MaxVoices.Location = new System.Drawing.Point(361, 6);
            MaxVoices.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            MaxVoices.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            MaxVoices.Name = "MaxVoices";
            MaxVoices.Size = new System.Drawing.Size(87, 23);
            MaxVoices.TabIndex = 39;
            MaxVoices.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            MaxVoices.Value = new decimal(new int[] { 100000, 0, 0, 0 });
            // 
            // AudioFreqLabel
            // 
            AudioFreqLabel.AutoSize = true;
            AudioFreqLabel.Location = new System.Drawing.Point(6, 35);
            AudioFreqLabel.Name = "AudioFreqLabel";
            AudioFreqLabel.Size = new System.Drawing.Size(133, 15);
            AudioFreqLabel.TabIndex = 37;
            AudioFreqLabel.Text = "Render audio frequency";
            // 
            // FrequencyBox
            // 
            FrequencyBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            FrequencyBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            FrequencyBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            FrequencyBox.FormattingEnabled = true;
            FrequencyBox.Items.AddRange(new object[] { "192000", "176400", "142180", "96000", "88200", "74750", "66150", "50400", "50000", "48000", "47250", "44100", "44056", "37800", "32000", "22050", "16000", "11025", "8000", "4000" });
            FrequencyBox.Location = new System.Drawing.Point(361, 32);
            FrequencyBox.Name = "FrequencyBox";
            FrequencyBox.Size = new System.Drawing.Size(68, 23);
            FrequencyBox.TabIndex = 36;
            // 
            // Label6
            // 
            Label6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            Label6.AutoSize = true;
            Label6.Location = new System.Drawing.Point(429, 35);
            Label6.Name = "Label6";
            Label6.Size = new System.Drawing.Size(21, 15);
            Label6.TabIndex = 38;
            Label6.Text = "Hz";
            // 
            // MTLimitVal
            // 
            MTLimitVal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            MTLimitVal.Location = new System.Drawing.Point(400, 102);
            MTLimitVal.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            MTLimitVal.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            MTLimitVal.Name = "MTLimitVal";
            MTLimitVal.Size = new System.Drawing.Size(48, 23);
            MTLimitVal.TabIndex = 45;
            MTLimitVal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            MTLimitVal.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // MTLimitLab
            // 
            MTLimitLab.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            MTLimitLab.AutoSize = true;
            MTLimitLab.Location = new System.Drawing.Point(350, 104);
            MTLimitLab.Name = "MTLimitLab";
            MTLimitLab.Size = new System.Drawing.Size(51, 15);
            MTLimitLab.TabIndex = 52;
            MTLimitLab.Text = "Threads:";
            // 
            // MTLimit
            // 
            MTLimit.AutoSize = true;
            MTLimit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            MTLimit.Location = new System.Drawing.Point(6, 102);
            MTLimit.Name = "MTLimit";
            MTLimit.Size = new System.Drawing.Size(326, 20);
            MTLimit.TabIndex = 51;
            MTLimit.Text = "Limit the amount of system threads available to the app";
            MTLimit.UseVisualStyleBackColor = true;
            MTLimit.CheckedChanged += MTLimit_CheckedChanged;
            // 
            // PerTrackMode
            // 
            PerTrackMode.AutoSize = true;
            PerTrackMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            PerTrackMode.Location = new System.Drawing.Point(6, 28);
            PerTrackMode.Name = "PerTrackMode";
            PerTrackMode.Size = new System.Drawing.Size(443, 20);
            PerTrackMode.TabIndex = 50;
            PerTrackMode.Text = "Render instead each track on a separate thread, and merge all in one audio file";
            PerTrackMode.UseVisualStyleBackColor = true;
            PerTrackMode.CheckedChanged += PerTrackMode_CheckedChanged;
            // 
            // AOFPath
            // 
            AOFPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            AOFPath.Location = new System.Drawing.Point(6, 155);
            AOFPath.Name = "AOFPath";
            AOFPath.Size = new System.Drawing.Size(361, 23);
            AOFPath.TabIndex = 48;
            // 
            // AutoOutputFolder
            // 
            AutoOutputFolder.AutoSize = true;
            AutoOutputFolder.FlatStyle = System.Windows.Forms.FlatStyle.System;
            AutoOutputFolder.Location = new System.Drawing.Point(6, 132);
            AutoOutputFolder.Name = "AutoOutputFolder";
            AutoOutputFolder.Size = new System.Drawing.Size(429, 20);
            AutoOutputFolder.TabIndex = 47;
            AutoOutputFolder.Text = "Export final audios directly to a specified folder instead of asking every time";
            AutoOutputFolder.UseVisualStyleBackColor = true;
            AutoOutputFolder.CheckedChanged += AutoOutputFolder_CheckedChanged;
            // 
            // PerTrackStorage
            // 
            PerTrackStorage.AutoSize = true;
            PerTrackStorage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            PerTrackStorage.Location = new System.Drawing.Point(6, 72);
            PerTrackStorage.Name = "PerTrackStorage";
            PerTrackStorage.Size = new System.Drawing.Size(405, 20);
            PerTrackStorage.TabIndex = 46;
            PerTrackStorage.Text = "Store each exported track into a folder with the same name as the MIDI";
            PerTrackStorage.UseVisualStyleBackColor = true;
            // 
            // PerTrackExportEach
            // 
            PerTrackExportEach.AutoSize = true;
            PerTrackExportEach.FlatStyle = System.Windows.Forms.FlatStyle.System;
            PerTrackExportEach.Location = new System.Drawing.Point(6, 50);
            PerTrackExportEach.Name = "PerTrackExportEach";
            PerTrackExportEach.Size = new System.Drawing.Size(408, 20);
            PerTrackExportEach.TabIndex = 45;
            PerTrackExportEach.Text = "Disable automatic merging of the tracks, and export them all separately";
            PerTrackExportEach.UseVisualStyleBackColor = true;
            PerTrackExportEach.CheckedChanged += PerTrackExportEach_CheckedChanged;
            // 
            // MTMode
            // 
            MTMode.AutoSize = true;
            MTMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            MTMode.Location = new System.Drawing.Point(6, 6);
            MTMode.Name = "MTMode";
            MTMode.Size = new System.Drawing.Size(409, 20);
            MTMode.TabIndex = 44;
            MTMode.Text = "Enable multi-threaded rendering mode, and render one MIDI per thread";
            MTMode.UseVisualStyleBackColor = true;
            MTMode.CheckedChanged += MTMode_CheckedChanged;
            // 
            // OkBtn
            // 
            OkBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            OkBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            OkBtn.Location = new System.Drawing.Point(395, 356);
            OkBtn.Name = "OkBtn";
            OkBtn.Size = new System.Drawing.Size(75, 23);
            OkBtn.TabIndex = 3;
            OkBtn.Text = "OK";
            OkBtn.UseVisualStyleBackColor = true;
            OkBtn.Click += OkBtn_Click;
            // 
            // DoActionAfterRenderVal
            // 
            DoActionAfterRenderVal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            DoActionAfterRenderVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            DoActionAfterRenderVal.FormattingEnabled = true;
            DoActionAfterRenderVal.Items.AddRange(new object[] { "Put the computer into sleep mode", "Put the computer into hibernation mode", "Turn the computer off", "Restart the computer" });
            DoActionAfterRenderVal.Location = new System.Drawing.Point(6, 29);
            DoActionAfterRenderVal.Name = "DoActionAfterRenderVal";
            DoActionAfterRenderVal.Size = new System.Drawing.Size(255, 23);
            DoActionAfterRenderVal.TabIndex = 1;
            // 
            // DoActionAfterRender
            // 
            DoActionAfterRender.AutoSize = true;
            DoActionAfterRender.FlatStyle = System.Windows.Forms.FlatStyle.System;
            DoActionAfterRender.Location = new System.Drawing.Point(6, 6);
            DoActionAfterRender.Name = "DoActionAfterRender";
            DoActionAfterRender.Size = new System.Drawing.Size(397, 20);
            DoActionAfterRender.TabIndex = 0;
            DoActionAfterRender.Text = "Do one of the following actions once the computer is done rendering";
            DoActionAfterRender.UseVisualStyleBackColor = true;
            DoActionAfterRender.CheckedChanged += DoActionAfterRender_CheckedChanged;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControl1.Controls.Add(ASet);
            tabControl1.Controls.Add(ESet);
            tabControl1.Controls.Add(PCSet);
            tabControl1.Location = new System.Drawing.Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(462, 336);
            tabControl1.TabIndex = 5;
            // 
            // ASet
            // 
            ASet.Controls.Add(groupBox2);
            ASet.Controls.Add(UnlimitedVoices);
            ASet.Controls.Add(groupBox1);
            ASet.Controls.Add(MaxVoicesLabel);
            ASet.Controls.Add(EnableLoudMax);
            ASet.Controls.Add(Label6);
            ASet.Controls.Add(NoteOff1);
            ASet.Controls.Add(FrequencyBox);
            ASet.Controls.Add(FXDisable);
            ASet.Controls.Add(AudioFreqLabel);
            ASet.Controls.Add(SincInter);
            ASet.Controls.Add(MaxVoices);
            ASet.Location = new System.Drawing.Point(4, 24);
            ASet.Name = "ASet";
            ASet.Padding = new System.Windows.Forms.Padding(3);
            ASet.Size = new System.Drawing.Size(454, 308);
            ASet.TabIndex = 0;
            ASet.Text = "Audio Settings";
            ASet.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox2.Controls.Add(RealTimeSimulation);
            groupBox2.Controls.Add(frameFluctValue);
            groupBox2.Controls.Add(fpsLab);
            groupBox2.Controls.Add(frameFluctLab);
            groupBox2.Controls.Add(fpsValue);
            groupBox2.Location = new System.Drawing.Point(6, 238);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(442, 64);
            groupBox2.TabIndex = 63;
            groupBox2.TabStop = false;
            groupBox2.Text = "RTS Mode (NEW!)";
            // 
            // RealTimeSimulation
            // 
            RealTimeSimulation.AutoSize = true;
            RealTimeSimulation.FlatStyle = System.Windows.Forms.FlatStyle.System;
            RealTimeSimulation.Location = new System.Drawing.Point(6, 27);
            RealTimeSimulation.Name = "RealTimeSimulation";
            RealTimeSimulation.Size = new System.Drawing.Size(227, 20);
            RealTimeSimulation.TabIndex = 58;
            RealTimeSimulation.Text = "Enable real-time playback simulation";
            RealTimeSimulation.UseVisualStyleBackColor = true;
            RealTimeSimulation.CheckedChanged += RealTimeSimulation_CheckedChanged;
            // 
            // frameFluctValue
            // 
            frameFluctValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            frameFluctValue.Enabled = false;
            frameFluctValue.Location = new System.Drawing.Point(389, 36);
            frameFluctValue.Name = "frameFluctValue";
            frameFluctValue.Size = new System.Drawing.Size(48, 23);
            frameFluctValue.TabIndex = 61;
            frameFluctValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            frameFluctValue.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // fpsLab
            // 
            fpsLab.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            fpsLab.AutoSize = true;
            fpsLab.Enabled = false;
            fpsLab.Location = new System.Drawing.Point(360, 15);
            fpsLab.Name = "fpsLab";
            fpsLab.Size = new System.Drawing.Size(29, 15);
            fpsLab.TabIndex = 60;
            fpsLab.Text = "FPS:";
            // 
            // frameFluctLab
            // 
            frameFluctLab.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            frameFluctLab.AutoSize = true;
            frameFluctLab.Enabled = false;
            frameFluctLab.Location = new System.Drawing.Point(264, 39);
            frameFluctLab.Name = "frameFluctLab";
            frameFluctLab.Size = new System.Drawing.Size(125, 15);
            frameFluctLab.TabIndex = 62;
            frameFluctLab.Text = "Frame fluctuation (%):";
            // 
            // fpsValue
            // 
            fpsValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            fpsValue.Enabled = false;
            fpsValue.Location = new System.Drawing.Point(389, 12);
            fpsValue.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            fpsValue.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            fpsValue.Name = "fpsValue";
            fpsValue.Size = new System.Drawing.Size(48, 23);
            fpsValue.TabIndex = 59;
            fpsValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            fpsValue.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // UnlimitedVoices
            // 
            UnlimitedVoices.AutoSize = true;
            UnlimitedVoices.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            UnlimitedVoices.FlatStyle = System.Windows.Forms.FlatStyle.System;
            UnlimitedVoices.Location = new System.Drawing.Point(274, 6);
            UnlimitedVoices.Name = "UnlimitedVoices";
            UnlimitedVoices.Size = new System.Drawing.Size(84, 20);
            UnlimitedVoices.TabIndex = 50;
            UnlimitedVoices.Text = "Unlimited";
            UnlimitedVoices.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            UnlimitedVoices.UseVisualStyleBackColor = true;
            UnlimitedVoices.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // ESet
            // 
            ESet.Controls.Add(MTLimitVal);
            ESet.Controls.Add(MTMode);
            ESet.Controls.Add(MTLimitLab);
            ESet.Controls.Add(PerTrackExportEach);
            ESet.Controls.Add(MTLimit);
            ESet.Controls.Add(PerTrackStorage);
            ESet.Controls.Add(PerTrackMode);
            ESet.Controls.Add(AutoOutputFolder);
            ESet.Controls.Add(AOFBrowse);
            ESet.Controls.Add(AOFPath);
            ESet.Location = new System.Drawing.Point(4, 24);
            ESet.Name = "ESet";
            ESet.Padding = new System.Windows.Forms.Padding(3);
            ESet.Size = new System.Drawing.Size(454, 308);
            ESet.TabIndex = 1;
            ESet.Text = "Export Settings";
            ESet.UseVisualStyleBackColor = true;
            // 
            // AOFBrowse
            // 
            AOFBrowse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            AOFBrowse.Location = new System.Drawing.Point(373, 154);
            AOFBrowse.Name = "AOFBrowse";
            AOFBrowse.Size = new System.Drawing.Size(75, 25);
            AOFBrowse.TabIndex = 49;
            AOFBrowse.Text = "Browse...";
            AOFBrowse.UseVisualStyleBackColor = true;
            AOFBrowse.Click += AOFBrowse_Click;
            // 
            // PCSet
            // 
            PCSet.Controls.Add(DoActionAfterRenderVal);
            PCSet.Controls.Add(DoActionAfterRender);
            PCSet.Location = new System.Drawing.Point(4, 24);
            PCSet.Name = "PCSet";
            PCSet.Size = new System.Drawing.Size(454, 308);
            PCSet.TabIndex = 2;
            PCSet.Text = "Post-Conversion Settings";
            PCSet.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(487, 393);
            ControlBox = false;
            Controls.Add(tabControl1);
            Controls.Add(OkBtn);
            Font = new System.Drawing.Font("Segoe UI", 9F);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Settings";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Settings";
            Load += Settings_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ChorusV).EndInit();
            ((System.ComponentModel.ISupportInitialize)ReverbV).EndInit();
            ((System.ComponentModel.ISupportInitialize)MaxVoices).EndInit();
            ((System.ComponentModel.ISupportInitialize)MTLimitVal).EndInit();
            tabControl1.ResumeLayout(false);
            ASet.ResumeLayout(false);
            ASet.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)frameFluctValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)fpsValue).EndInit();
            ESet.ResumeLayout(false);
            ESet.PerformLayout();
            PCSet.ResumeLayout(false);
            PCSet.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        internal System.Windows.Forms.CheckBox SincInter;
        internal System.Windows.Forms.Label MaxVoicesLabel;
        public System.Windows.Forms.NumericUpDown MaxVoices;
        internal System.Windows.Forms.Label AudioFreqLabel;
        internal System.Windows.Forms.ComboBox FrequencyBox;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.CheckBox EnableLoudMax;
        internal System.Windows.Forms.CheckBox NoteOff1;
        internal System.Windows.Forms.CheckBox FXDisable;
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
        private System.Windows.Forms.ComboBox DoActionAfterRenderVal;
        private System.Windows.Forms.CheckBox DoActionAfterRender;
        private System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.CheckBox EnableRCOverride;
        private System.Windows.Forms.Label ReverbL;
        private System.Windows.Forms.NumericUpDown ChorusV;
        private System.Windows.Forms.Label ChorusL;
        private System.Windows.Forms.NumericUpDown ReverbV;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ASet;
        private System.Windows.Forms.TabPage ESet;
        private System.Windows.Forms.Button AOFBrowse;
        private System.Windows.Forms.TabPage PCSet;
        internal System.Windows.Forms.CheckBox UnlimitedVoices;
        private System.Windows.Forms.GroupBox groupBox2;
        internal System.Windows.Forms.CheckBox RealTimeSimulation;
        public System.Windows.Forms.NumericUpDown frameFluctValue;
        private System.Windows.Forms.Label fpsLab;
        private System.Windows.Forms.Label frameFluctLab;
        public System.Windows.Forms.NumericUpDown fpsValue;
    }
}