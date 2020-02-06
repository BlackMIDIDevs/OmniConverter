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
            this.components = new System.ComponentModel.Container();
            this.LogGB = new System.Windows.Forms.GroupBox();
            this.ThreadsPanel = new System.Windows.Forms.Panel();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.PB = new wyDay.Controls.Windows7ProgressBar();
            this.StatusLab = new System.Windows.Forms.Label();
            this.Check = new System.Windows.Forms.Timer(this.components);
            this.TPB = new wyDay.Controls.Windows7ProgressBar();
            this.LogGB.SuspendLayout();
            this.SuspendLayout();
            // 
            // LogGB
            // 
            this.LogGB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogGB.Controls.Add(this.ThreadsPanel);
            this.LogGB.Location = new System.Drawing.Point(14, 112);
            this.LogGB.Name = "LogGB";
            this.LogGB.Size = new System.Drawing.Size(454, 195);
            this.LogGB.TabIndex = 7;
            this.LogGB.TabStop = false;
            this.LogGB.Text = "Active threads";
            // 
            // ThreadsPanel
            // 
            this.ThreadsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ThreadsPanel.AutoScroll = true;
            this.ThreadsPanel.Location = new System.Drawing.Point(2, 20);
            this.ThreadsPanel.Name = "ThreadsPanel";
            this.ThreadsPanel.Size = new System.Drawing.Size(449, 172);
            this.ThreadsPanel.TabIndex = 0;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.Location = new System.Drawing.Point(380, 78);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(87, 27);
            this.CancelBtn.TabIndex = 6;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // PB
            // 
            this.PB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PB.ContainerControl = this;
            this.PB.Location = new System.Drawing.Point(14, 79);
            this.PB.Name = "PB";
            this.PB.ShowInTaskbar = true;
            this.PB.Size = new System.Drawing.Size(359, 13);
            this.PB.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.PB.TabIndex = 5;
            // 
            // StatusLab
            // 
            this.StatusLab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StatusLab.Location = new System.Drawing.Point(14, 10);
            this.StatusLab.Name = "StatusLab";
            this.StatusLab.Size = new System.Drawing.Size(454, 65);
            this.StatusLab.TabIndex = 4;
            this.StatusLab.Text = "Initializing, please wait...";
            this.StatusLab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Check
            // 
            this.Check.Enabled = true;
            this.Check.Tick += new System.EventHandler(this.Check_Tick);
            // 
            // TPB
            // 
            this.TPB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TPB.ContainerControl = this;
            this.TPB.Location = new System.Drawing.Point(14, 91);
            this.TPB.Name = "TPB";
            this.TPB.ShowInTaskbar = true;
            this.TPB.Size = new System.Drawing.Size(359, 13);
            this.TPB.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.TPB.TabIndex = 8;
            // 
            // MIDIConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 321);
            this.ControlBox = false;
            this.Controls.Add(this.TPB);
            this.Controls.Add(this.LogGB);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.PB);
            this.Controls.Add(this.StatusLab);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MIDIConverter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Converting MIDIs...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MIDIConverter_FormClosing);
            this.Load += new System.EventHandler(this.MIDIConverter_Load);
            this.LogGB.ResumeLayout(false);
            this.ResumeLayout(false);

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