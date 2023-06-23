namespace OmniConverter
{
    partial class MIDIConverter
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
            LogGB = new System.Windows.Forms.GroupBox();
            ThreadsPanel = new System.Windows.Forms.Panel();
            CancelBtn = new System.Windows.Forms.Button();
            PB = new wyDay.Controls.Windows7ProgressBar();
            StatusLab = new System.Windows.Forms.Label();
            Check = new System.Windows.Forms.Timer(components);
            TPB = new wyDay.Controls.Windows7ProgressBar();
            LogGB.SuspendLayout();
            SuspendLayout();
            // 
            // LogGB
            // 
            LogGB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LogGB.Controls.Add(ThreadsPanel);
            LogGB.Location = new System.Drawing.Point(14, 112);
            LogGB.Name = "LogGB";
            LogGB.Size = new System.Drawing.Size(454, 291);
            LogGB.TabIndex = 7;
            LogGB.TabStop = false;
            LogGB.Text = "Active threads";
            // 
            // ThreadsPanel
            // 
            ThreadsPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ThreadsPanel.AutoScroll = true;
            ThreadsPanel.Location = new System.Drawing.Point(2, 20);
            ThreadsPanel.Name = "ThreadsPanel";
            ThreadsPanel.Size = new System.Drawing.Size(449, 268);
            ThreadsPanel.TabIndex = 0;
            // 
            // CancelBtn
            // 
            CancelBtn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            CancelBtn.Location = new System.Drawing.Point(380, 78);
            CancelBtn.Name = "CancelBtn";
            CancelBtn.Size = new System.Drawing.Size(87, 27);
            CancelBtn.TabIndex = 6;
            CancelBtn.Text = "Cancel";
            CancelBtn.UseVisualStyleBackColor = true;
            CancelBtn.Click += CancelBtn_Click;
            // 
            // PB
            // 
            PB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PB.ContainerControl = this;
            PB.Location = new System.Drawing.Point(14, 79);
            PB.Name = "PB";
            PB.ShowInTaskbar = true;
            PB.Size = new System.Drawing.Size(359, 13);
            PB.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            PB.TabIndex = 5;
            // 
            // StatusLab
            // 
            StatusLab.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            StatusLab.Location = new System.Drawing.Point(14, 10);
            StatusLab.Name = "StatusLab";
            StatusLab.Size = new System.Drawing.Size(454, 65);
            StatusLab.TabIndex = 4;
            StatusLab.Text = "Initializing, please wait...";
            StatusLab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Check
            // 
            Check.Enabled = true;
            Check.Tick += Check_Tick;
            // 
            // TPB
            // 
            TPB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TPB.ContainerControl = this;
            TPB.Location = new System.Drawing.Point(14, 91);
            TPB.Name = "TPB";
            TPB.ShowInTaskbar = true;
            TPB.Size = new System.Drawing.Size(359, 13);
            TPB.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            TPB.TabIndex = 8;
            // 
            // MIDIConverter
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(482, 417);
            ControlBox = false;
            Controls.Add(TPB);
            Controls.Add(LogGB);
            Controls.Add(CancelBtn);
            Controls.Add(PB);
            Controls.Add(StatusLab);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MIDIConverter";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Converting MIDIs...";
            FormClosing += MIDIConverter_FormClosing;
            Load += MIDIConverter_Load;
            LogGB.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox LogGB;
        private System.Windows.Forms.Button CancelBtn;
        private wyDay.Controls.Windows7ProgressBar PB;
        private System.Windows.Forms.Label StatusLab;
        private System.Windows.Forms.Timer Check;
        private wyDay.Controls.Windows7ProgressBar TPB;
        private System.Windows.Forms.Panel ThreadsPanel;
    }
}