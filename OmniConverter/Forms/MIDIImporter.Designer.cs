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
            this.components = new System.ComponentModel.Container();
            this.StatusLab = new System.Windows.Forms.Label();
            this.PB = new wyDay.Controls.Windows7ProgressBar();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.LogGB = new System.Windows.Forms.GroupBox();
            this.LogPanel = new System.Windows.Forms.Panel();
            this.Check = new System.Windows.Forms.Timer(this.components);
            this.LogGB.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusLab
            // 
            this.StatusLab.Location = new System.Drawing.Point(14, 10);
            this.StatusLab.Name = "StatusLab";
            this.StatusLab.Size = new System.Drawing.Size(454, 65);
            this.StatusLab.TabIndex = 0;
            this.StatusLab.Text = "Initializing, please wait...";
            this.StatusLab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PB
            // 
            this.PB.ContainerControl = this;
            this.PB.Location = new System.Drawing.Point(14, 78);
            this.PB.Name = "PB";
            this.PB.ShowInTaskbar = true;
            this.PB.Size = new System.Drawing.Size(359, 27);
            this.PB.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.PB.TabIndex = 1;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(380, 78);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(87, 27);
            this.CancelBtn.TabIndex = 2;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // LogGB
            // 
            this.LogGB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogGB.Controls.Add(this.LogPanel);
            this.LogGB.Location = new System.Drawing.Point(14, 112);
            this.LogGB.Name = "LogGB";
            this.LogGB.Size = new System.Drawing.Size(454, 194);
            this.LogGB.TabIndex = 3;
            this.LogGB.TabStop = false;
            this.LogGB.Text = "Log";
            // 
            // LogPanel
            // 
            this.LogPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogPanel.AutoScroll = true;
            this.LogPanel.Location = new System.Drawing.Point(3, 19);
            this.LogPanel.Name = "LogPanel";
            this.LogPanel.Size = new System.Drawing.Size(449, 172);
            this.LogPanel.TabIndex = 1;
            // 
            // Check
            // 
            this.Check.Enabled = true;
            this.Check.Tick += new System.EventHandler(this.Check_Tick);
            // 
            // MIDIImporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 320);
            this.ControlBox = false;
            this.Controls.Add(this.LogGB);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.PB);
            this.Controls.Add(this.StatusLab);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MIDIImporter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Importing MIDIs...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MIDIImporter_FormClosing);
            this.Load += new System.EventHandler(this.MIDIImporter_Load);
            this.LogGB.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label StatusLab;
        private wyDay.Controls.Windows7ProgressBar PB;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.GroupBox LogGB;
        private System.Windows.Forms.Timer Check;
        private System.Windows.Forms.Panel LogPanel;
    }
}