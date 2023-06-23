namespace OmniConverter
{
    partial class MIDIImporter
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
            StatusLab = new System.Windows.Forms.Label();
            CancelBtn = new System.Windows.Forms.Button();
            LogGB = new System.Windows.Forms.GroupBox();
            LogPanel = new System.Windows.Forms.Panel();
            Check = new System.Windows.Forms.Timer(components);
            PB = new wyDay.Controls.Windows7ProgressBar();
            LogGB.SuspendLayout();
            SuspendLayout();
            // 
            // StatusLab
            // 
            StatusLab.Location = new System.Drawing.Point(14, 10);
            StatusLab.Name = "StatusLab";
            StatusLab.Size = new System.Drawing.Size(454, 65);
            StatusLab.TabIndex = 0;
            StatusLab.Text = "Initializing, please wait...";
            StatusLab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CancelBtn
            // 
            CancelBtn.Location = new System.Drawing.Point(380, 78);
            CancelBtn.Name = "CancelBtn";
            CancelBtn.Size = new System.Drawing.Size(87, 27);
            CancelBtn.TabIndex = 2;
            CancelBtn.Text = "Cancel";
            CancelBtn.UseVisualStyleBackColor = true;
            CancelBtn.Click += CancelBtn_Click;
            // 
            // LogGB
            // 
            LogGB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LogGB.Controls.Add(LogPanel);
            LogGB.Location = new System.Drawing.Point(14, 112);
            LogGB.Name = "LogGB";
            LogGB.Size = new System.Drawing.Size(454, 194);
            LogGB.TabIndex = 3;
            LogGB.TabStop = false;
            LogGB.Text = "Log";
            // 
            // LogPanel
            // 
            LogPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LogPanel.AutoScroll = true;
            LogPanel.Location = new System.Drawing.Point(3, 19);
            LogPanel.Name = "LogPanel";
            LogPanel.Size = new System.Drawing.Size(449, 172);
            LogPanel.TabIndex = 1;
            // 
            // Check
            // 
            Check.Enabled = true;
            Check.Tick += Check_Tick;
            // 
            // PB
            // 
            PB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PB.ContainerControl = this;
            PB.Location = new System.Drawing.Point(14, 79);
            PB.Name = "PB";
            PB.ShowInTaskbar = true;
            PB.Size = new System.Drawing.Size(359, 25);
            PB.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            PB.TabIndex = 9;
            // 
            // MIDIImporter
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(482, 320);
            ControlBox = false;
            Controls.Add(PB);
            Controls.Add(LogGB);
            Controls.Add(CancelBtn);
            Controls.Add(StatusLab);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MIDIImporter";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Importing MIDIs...";
            FormClosing += MIDIImporter_FormClosing;
            Load += MIDIImporter_Load;
            LogGB.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label StatusLab;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.GroupBox LogGB;
        private System.Windows.Forms.Timer Check;
        private System.Windows.Forms.Panel LogPanel;
        private wyDay.Controls.Windows7ProgressBar PB;
    }
}