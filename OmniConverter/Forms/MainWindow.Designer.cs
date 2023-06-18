namespace OmniConverter
{
    partial class MainWindow
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            OCMenu = new System.Windows.Forms.MenuStrip();
            menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            AddMIDIsToQueue = new System.Windows.Forms.ToolStripMenuItem();
            RemoveMIDIsFromQueue = new System.Windows.Forms.ToolStripMenuItem();
            ClearQueue = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            ExitFromConverter = new System.Windows.Forms.ToolStripMenuItem();
            menuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            InfoAboutConverter = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            CreateIssueGitHub = new System.Windows.Forms.ToolStripMenuItem();
            CheckForUpdates = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            DownloadConvSrc = new System.Windows.Forms.ToolStripMenuItem();
            groupBox1 = new System.Windows.Forms.GroupBox();
            TMIDIs = new System.Windows.Forms.Label();
            FNVal = new System.Windows.Forms.RichTextBox();
            CMIDIs = new System.Windows.Forms.Button();
            FPVal = new System.Windows.Forms.RichTextBox();
            COS = new System.Windows.Forms.Button();
            TLVal = new System.Windows.Forms.Label();
            ESFL = new System.Windows.Forms.Button();
            TLLab = new System.Windows.Forms.Label();
            SVal = new System.Windows.Forms.Label();
            SLab = new System.Windows.Forms.Label();
            TVal = new System.Windows.Forms.Label();
            TLab = new System.Windows.Forms.Label();
            NCVal = new System.Windows.Forms.Label();
            NCLab = new System.Windows.Forms.Label();
            FPLab = new System.Windows.Forms.Label();
            FNLab = new System.Windows.Forms.Label();
            MIDIQueue = new System.Windows.Forms.ListBox();
            OCContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
            AddMIDIsToQueueRC = new System.Windows.Forms.ToolStripMenuItem();
            RemoveMIDIsFromQueueRC = new System.Windows.Forms.ToolStripMenuItem();
            ClearQueueRC = new System.Windows.Forms.ToolStripMenuItem();
            OCMenu.SuspendLayout();
            groupBox1.SuspendLayout();
            OCContextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // OCMenu
            // 
            OCMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { menuItem1, menuItem7 });
            OCMenu.Location = new System.Drawing.Point(0, 0);
            OCMenu.Name = "OCMenu";
            OCMenu.Size = new System.Drawing.Size(784, 24);
            OCMenu.TabIndex = 2;
            // 
            // menuItem1
            // 
            menuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { AddMIDIsToQueue, RemoveMIDIsFromQueue, ClearQueue, toolStripMenuItem1, ExitFromConverter });
            menuItem1.Name = "menuItem1";
            menuItem1.Size = new System.Drawing.Size(37, 20);
            menuItem1.Text = "File";
            // 
            // AddMIDIsToQueue
            // 
            AddMIDIsToQueue.Name = "AddMIDIsToQueue";
            AddMIDIsToQueue.Size = new System.Drawing.Size(235, 22);
            AddMIDIsToQueue.Text = "Add MIDIs to the queue";
            AddMIDIsToQueue.Click += AddMIDIsToQueue_Click;
            // 
            // RemoveMIDIsFromQueue
            // 
            RemoveMIDIsFromQueue.Name = "RemoveMIDIsFromQueue";
            RemoveMIDIsFromQueue.Size = new System.Drawing.Size(235, 22);
            RemoveMIDIsFromQueue.Text = "Remove MIDIs from the queue";
            RemoveMIDIsFromQueue.Click += RemoveMIDIsFromQueue_Click;
            // 
            // ClearQueue
            // 
            ClearQueue.Name = "ClearQueue";
            ClearQueue.Size = new System.Drawing.Size(235, 22);
            ClearQueue.Text = "Clear queue";
            ClearQueue.Click += ClearQueue_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(232, 6);
            // 
            // ExitFromConverter
            // 
            ExitFromConverter.Name = "ExitFromConverter";
            ExitFromConverter.Size = new System.Drawing.Size(235, 22);
            ExitFromConverter.Text = "Exit";
            ExitFromConverter.Click += ExitFromConverter_Click;
            // 
            // menuItem7
            // 
            menuItem7.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { InfoAboutConverter, toolStripMenuItem3, CreateIssueGitHub, CheckForUpdates, toolStripMenuItem2, DownloadConvSrc });
            menuItem7.Name = "menuItem7";
            menuItem7.Size = new System.Drawing.Size(24, 20);
            menuItem7.Text = "?";
            // 
            // InfoAboutConverter
            // 
            InfoAboutConverter.Name = "InfoAboutConverter";
            InfoAboutConverter.Size = new System.Drawing.Size(276, 22);
            InfoAboutConverter.Text = "Information about the converter";
            InfoAboutConverter.Click += InfoAboutConverter_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new System.Drawing.Size(273, 6);
            // 
            // CreateIssueGitHub
            // 
            CreateIssueGitHub.Name = "CreateIssueGitHub";
            CreateIssueGitHub.Size = new System.Drawing.Size(276, 22);
            CreateIssueGitHub.Text = "Create an issue on GitHub";
            // 
            // CheckForUpdates
            // 
            CheckForUpdates.Name = "CheckForUpdates";
            CheckForUpdates.Size = new System.Drawing.Size(276, 22);
            CheckForUpdates.Text = "Check for updates";
            CheckForUpdates.Click += CheckForUpdates_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(273, 6);
            // 
            // DownloadConvSrc
            // 
            DownloadConvSrc.Name = "DownloadConvSrc";
            DownloadConvSrc.Size = new System.Drawing.Size(276, 22);
            DownloadConvSrc.Text = "Download the converter's source code";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(TMIDIs);
            groupBox1.Controls.Add(FNVal);
            groupBox1.Controls.Add(CMIDIs);
            groupBox1.Controls.Add(FPVal);
            groupBox1.Controls.Add(COS);
            groupBox1.Controls.Add(TLVal);
            groupBox1.Controls.Add(ESFL);
            groupBox1.Controls.Add(TLLab);
            groupBox1.Controls.Add(SVal);
            groupBox1.Controls.Add(SLab);
            groupBox1.Controls.Add(TVal);
            groupBox1.Controls.Add(TLab);
            groupBox1.Controls.Add(NCVal);
            groupBox1.Controls.Add(NCLab);
            groupBox1.Controls.Add(FPLab);
            groupBox1.Controls.Add(FNLab);
            groupBox1.Location = new System.Drawing.Point(12, 394);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(760, 134);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Information";
            // 
            // TMIDIs
            // 
            TMIDIs.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            TMIDIs.Location = new System.Drawing.Point(595, 60);
            TMIDIs.Name = "TMIDIs";
            TMIDIs.Size = new System.Drawing.Size(160, 19);
            TMIDIs.TabIndex = 7;
            TMIDIs.Text = "Total MIDIs: 0";
            TMIDIs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FNVal
            // 
            FNVal.BackColor = System.Drawing.SystemColors.Control;
            FNVal.BorderStyle = System.Windows.Forms.BorderStyle.None;
            FNVal.Location = new System.Drawing.Point(85, 19);
            FNVal.Multiline = false;
            FNVal.Name = "FNVal";
            FNVal.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            FNVal.Size = new System.Drawing.Size(503, 15);
            FNVal.TabIndex = 2;
            FNVal.TabStop = false;
            FNVal.Text = "No MIDI selected";
            // 
            // CMIDIs
            // 
            CMIDIs.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            CMIDIs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CMIDIs.Location = new System.Drawing.Point(595, 15);
            CMIDIs.Name = "CMIDIs";
            CMIDIs.Size = new System.Drawing.Size(160, 23);
            CMIDIs.TabIndex = 4;
            CMIDIs.Text = "Convert MIDIs";
            CMIDIs.UseVisualStyleBackColor = true;
            CMIDIs.Click += CMIDIs_Click;
            // 
            // FPVal
            // 
            FPVal.BackColor = System.Drawing.SystemColors.Control;
            FPVal.BorderStyle = System.Windows.Forms.BorderStyle.None;
            FPVal.Location = new System.Drawing.Point(85, 37);
            FPVal.Multiline = false;
            FPVal.Name = "FPVal";
            FPVal.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            FPVal.Size = new System.Drawing.Size(503, 15);
            FPVal.TabIndex = 3;
            FPVal.TabStop = false;
            FPVal.Text = "No MIDI selected";
            // 
            // COS
            // 
            COS.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            COS.Location = new System.Drawing.Point(595, 82);
            COS.Name = "COS";
            COS.Size = new System.Drawing.Size(160, 23);
            COS.TabIndex = 6;
            COS.Text = "Change output settings";
            COS.UseVisualStyleBackColor = true;
            COS.Click += COS_Click;
            // 
            // TLVal
            // 
            TLVal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TLVal.Location = new System.Drawing.Point(82, 55);
            TLVal.Name = "TLVal";
            TLVal.Size = new System.Drawing.Size(506, 15);
            TLVal.TabIndex = 11;
            TLVal.Text = "0:00.000";
            // 
            // ESFL
            // 
            ESFL.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            ESFL.Location = new System.Drawing.Point(595, 105);
            ESFL.Name = "ESFL";
            ESFL.Size = new System.Drawing.Size(160, 23);
            ESFL.TabIndex = 7;
            ESFL.Text = "Edit SoundFonts list";
            ESFL.UseVisualStyleBackColor = true;
            ESFL.Click += ESFL_Click;
            // 
            // TLLab
            // 
            TLLab.AutoSize = true;
            TLLab.Location = new System.Drawing.Point(6, 55);
            TLLab.Name = "TLLab";
            TLLab.Size = new System.Drawing.Size(47, 15);
            TLLab.TabIndex = 10;
            TLLab.Text = "Length:";
            // 
            // SVal
            // 
            SVal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            SVal.Location = new System.Drawing.Point(82, 109);
            SVal.Name = "SVal";
            SVal.Size = new System.Drawing.Size(506, 15);
            SVal.TabIndex = 9;
            SVal.Text = "0.00 B";
            // 
            // SLab
            // 
            SLab.AutoSize = true;
            SLab.Location = new System.Drawing.Point(6, 109);
            SLab.Name = "SLab";
            SLab.Size = new System.Drawing.Size(30, 15);
            SLab.TabIndex = 8;
            SLab.Text = "Size:";
            // 
            // TVal
            // 
            TVal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TVal.Location = new System.Drawing.Point(82, 73);
            TVal.Name = "TVal";
            TVal.Size = new System.Drawing.Size(506, 15);
            TVal.TabIndex = 7;
            TVal.Text = "0";
            // 
            // TLab
            // 
            TLab.AutoSize = true;
            TLab.Location = new System.Drawing.Point(6, 73);
            TLab.Name = "TLab";
            TLab.Size = new System.Drawing.Size(42, 15);
            TLab.TabIndex = 6;
            TLab.Text = "Tracks:";
            // 
            // NCVal
            // 
            NCVal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            NCVal.Location = new System.Drawing.Point(82, 91);
            NCVal.Name = "NCVal";
            NCVal.Size = new System.Drawing.Size(506, 15);
            NCVal.TabIndex = 5;
            NCVal.Text = "0";
            // 
            // NCLab
            // 
            NCLab.AutoSize = true;
            NCLab.Location = new System.Drawing.Point(6, 91);
            NCLab.Name = "NCLab";
            NCLab.Size = new System.Drawing.Size(70, 15);
            NCLab.TabIndex = 4;
            NCLab.Text = "Note count:";
            // 
            // FPLab
            // 
            FPLab.AutoSize = true;
            FPLab.Location = new System.Drawing.Point(6, 37);
            FPLab.Name = "FPLab";
            FPLab.Size = new System.Drawing.Size(56, 15);
            FPLab.TabIndex = 1;
            FPLab.Text = "Full path:";
            // 
            // FNLab
            // 
            FNLab.AutoSize = true;
            FNLab.Location = new System.Drawing.Point(6, 19);
            FNLab.Name = "FNLab";
            FNLab.Size = new System.Drawing.Size(58, 15);
            FNLab.TabIndex = 0;
            FNLab.Text = "Filename:";
            // 
            // MIDIQueue
            // 
            MIDIQueue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            MIDIQueue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            MIDIQueue.FormattingEnabled = true;
            MIDIQueue.IntegralHeight = false;
            MIDIQueue.ItemHeight = 15;
            MIDIQueue.Location = new System.Drawing.Point(12, 27);
            MIDIQueue.Name = "MIDIQueue";
            MIDIQueue.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            MIDIQueue.Size = new System.Drawing.Size(760, 361);
            MIDIQueue.TabIndex = 1;
            MIDIQueue.SelectedIndexChanged += MIDIQueue_SelectedIndexChanged;
            MIDIQueue.DragDrop += MIDIQueue_DragDrop;
            MIDIQueue.DragEnter += MIDIQueue_DragEnter;
            MIDIQueue.KeyDown += MIDIQueue_KeyDown;
            // 
            // OCContextMenu
            // 
            OCContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { AddMIDIsToQueueRC, RemoveMIDIsFromQueueRC, ClearQueueRC });
            OCContextMenu.Name = "OCContextMenu";
            OCContextMenu.Size = new System.Drawing.Size(236, 70);
            // 
            // AddMIDIsToQueueRC
            // 
            AddMIDIsToQueueRC.Name = "AddMIDIsToQueueRC";
            AddMIDIsToQueueRC.Size = new System.Drawing.Size(235, 22);
            AddMIDIsToQueueRC.Text = "Add MIDIs to the queue";
            AddMIDIsToQueueRC.Click += AddMIDIsToQueue_Click;
            // 
            // RemoveMIDIsFromQueueRC
            // 
            RemoveMIDIsFromQueueRC.Name = "RemoveMIDIsFromQueueRC";
            RemoveMIDIsFromQueueRC.Size = new System.Drawing.Size(235, 22);
            RemoveMIDIsFromQueueRC.Text = "Remove MIDIs from the queue";
            RemoveMIDIsFromQueueRC.Click += RemoveMIDIsFromQueue_Click;
            // 
            // ClearQueueRC
            // 
            ClearQueueRC.Name = "ClearQueueRC";
            ClearQueueRC.Size = new System.Drawing.Size(235, 22);
            ClearQueueRC.Text = "Clear queue";
            ClearQueueRC.Click += ClearQueue_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(784, 540);
            ContextMenuStrip = OCContextMenu;
            Controls.Add(MIDIQueue);
            Controls.Add(groupBox1);
            Controls.Add(OCMenu);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "MainWindow";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "OmniConverter";
            Load += MainWindow_Load;
            OCMenu.ResumeLayout(false);
            OCMenu.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            OCContextMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip OCMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItem1;
        private System.Windows.Forms.ToolStripMenuItem AddMIDIsToQueue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox MIDIQueue;
        private System.Windows.Forms.Label FPLab;
        private System.Windows.Forms.Label FNLab;
        private System.Windows.Forms.Label NCVal;
        private System.Windows.Forms.Label NCLab;
        private System.Windows.Forms.Label TVal;
        private System.Windows.Forms.Label TLab;
        private System.Windows.Forms.Label SVal;
        private System.Windows.Forms.Label SLab;
        private System.Windows.Forms.Button CMIDIs;
        private System.Windows.Forms.Button COS;
        private System.Windows.Forms.Button ESFL;
        private System.Windows.Forms.Label TMIDIs;
        private System.Windows.Forms.Label TLVal;
        private System.Windows.Forms.Label TLLab;
        private System.Windows.Forms.RichTextBox FPVal;
        private System.Windows.Forms.RichTextBox FNVal;
        private System.Windows.Forms.ToolStripMenuItem RemoveMIDIsFromQueue;
        private System.Windows.Forms.ToolStripMenuItem ClearQueue;
        private System.Windows.Forms.ToolStripMenuItem ExitFromConverter;
        private System.Windows.Forms.ToolStripMenuItem menuItem7;
        private System.Windows.Forms.ToolStripMenuItem InfoAboutConverter;
        private System.Windows.Forms.ToolStripMenuItem CreateIssueGitHub;
        private System.Windows.Forms.ToolStripMenuItem CheckForUpdates;
        private System.Windows.Forms.ToolStripMenuItem DownloadConvSrc;
        private System.Windows.Forms.ContextMenuStrip OCContextMenu;
        private System.Windows.Forms.ToolStripMenuItem AddMIDIsToQueueRC;
        private System.Windows.Forms.ToolStripMenuItem RemoveMIDIsFromQueueRC;
        private System.Windows.Forms.ToolStripMenuItem ClearQueueRC;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    }
}

