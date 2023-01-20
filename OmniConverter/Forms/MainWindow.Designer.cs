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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.OCMenuIcons = new wyDay.Controls.VistaMenu(this.components);
            this.OCMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.AddMIDIsToQueue = new System.Windows.Forms.MenuItem();
            this.RemoveMIDIsFromQueue = new System.Windows.Forms.MenuItem();
            this.ClearQueue = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.ExitFromConverter = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.InfoAboutConverter = new System.Windows.Forms.MenuItem();
            this.CreateIssueGitHub = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.CheckForUpdates = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.DownloadConvSrc = new System.Windows.Forms.MenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TMIDIs = new System.Windows.Forms.Label();
            this.FNVal = new System.Windows.Forms.RichTextBox();
            this.CMIDIs = new System.Windows.Forms.Button();
            this.FPVal = new System.Windows.Forms.RichTextBox();
            this.COS = new System.Windows.Forms.Button();
            this.TLVal = new System.Windows.Forms.Label();
            this.ESFL = new System.Windows.Forms.Button();
            this.TLLab = new System.Windows.Forms.Label();
            this.SVal = new System.Windows.Forms.Label();
            this.SLab = new System.Windows.Forms.Label();
            this.TVal = new System.Windows.Forms.Label();
            this.TLab = new System.Windows.Forms.Label();
            this.NCVal = new System.Windows.Forms.Label();
            this.NCLab = new System.Windows.Forms.Label();
            this.FPLab = new System.Windows.Forms.Label();
            this.FNLab = new System.Windows.Forms.Label();
            this.MIDIQueue = new System.Windows.Forms.ListBox();
            this.OCContextMenu = new System.Windows.Forms.ContextMenu();
            this.AddMIDIsToQueueRC = new System.Windows.Forms.MenuItem();
            this.RemoveMIDIsFromQueueRC = new System.Windows.Forms.MenuItem();
            this.ClearQueueRC = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.OCMenuIcons)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OCMenuIcons
            // 
            this.OCMenuIcons.ContainerControl = this;
            // 
            // OCMenu
            // 
            this.OCMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem7});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.AddMIDIsToQueue,
            this.RemoveMIDIsFromQueue,
            this.ClearQueue,
            this.menuItem5,
            this.ExitFromConverter});
            this.menuItem1.Text = "File";
            // 
            // AddMIDIsToQueue
            // 
            this.AddMIDIsToQueue.Index = 0;
            this.AddMIDIsToQueue.Text = "Add MIDIs to the queue";
            this.AddMIDIsToQueue.Click += new System.EventHandler(this.AddMIDIsToQueue_Click);
            // 
            // RemoveMIDIsFromQueue
            // 
            this.RemoveMIDIsFromQueue.Index = 1;
            this.RemoveMIDIsFromQueue.Text = "Remove MIDIs from the queue";
            this.RemoveMIDIsFromQueue.Click += new System.EventHandler(this.RemoveMIDIsFromQueue_Click);
            // 
            // ClearQueue
            // 
            this.ClearQueue.Index = 2;
            this.ClearQueue.Text = "Clear queue";
            this.ClearQueue.Click += new System.EventHandler(this.ClearQueue_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 3;
            this.menuItem5.Text = "-";
            // 
            // ExitFromConverter
            // 
            this.ExitFromConverter.Index = 4;
            this.ExitFromConverter.Text = "Exit";
            this.ExitFromConverter.Click += new System.EventHandler(this.ExitFromConverter_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 1;
            this.menuItem7.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.InfoAboutConverter,
            this.CreateIssueGitHub,
            this.menuItem4,
            this.CheckForUpdates,
            this.menuItem9,
            this.DownloadConvSrc});
            this.menuItem7.Text = "?";
            // 
            // InfoAboutConverter
            // 
            this.InfoAboutConverter.Index = 0;
            this.InfoAboutConverter.Text = "Information about the converter";
            this.InfoAboutConverter.Click += new System.EventHandler(this.InfoAboutConverter_Click);
            // 
            // CreateIssueGitHub
            // 
            this.CreateIssueGitHub.Index = 1;
            this.CreateIssueGitHub.Text = "Create an issue on GitHub";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            this.menuItem4.Text = "-";
            // 
            // CheckForUpdates
            // 
            this.CheckForUpdates.Index = 3;
            this.CheckForUpdates.Text = "Check for updates";
            this.CheckForUpdates.Click += new System.EventHandler(this.CheckForUpdates_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 4;
            this.menuItem9.Text = "-";
            // 
            // DownloadConvSrc
            // 
            this.DownloadConvSrc.Index = 5;
            this.DownloadConvSrc.Text = "Download the converter\'s source code";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.TMIDIs);
            this.groupBox1.Controls.Add(this.FNVal);
            this.groupBox1.Controls.Add(this.CMIDIs);
            this.groupBox1.Controls.Add(this.FPVal);
            this.groupBox1.Controls.Add(this.COS);
            this.groupBox1.Controls.Add(this.TLVal);
            this.groupBox1.Controls.Add(this.ESFL);
            this.groupBox1.Controls.Add(this.TLLab);
            this.groupBox1.Controls.Add(this.SVal);
            this.groupBox1.Controls.Add(this.SLab);
            this.groupBox1.Controls.Add(this.TVal);
            this.groupBox1.Controls.Add(this.TLab);
            this.groupBox1.Controls.Add(this.NCVal);
            this.groupBox1.Controls.Add(this.NCLab);
            this.groupBox1.Controls.Add(this.FPLab);
            this.groupBox1.Controls.Add(this.FNLab);
            this.groupBox1.Location = new System.Drawing.Point(12, 394);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(760, 134);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Information";
            // 
            // TMIDIs
            // 
            this.TMIDIs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TMIDIs.Location = new System.Drawing.Point(595, 60);
            this.TMIDIs.Name = "TMIDIs";
            this.TMIDIs.Size = new System.Drawing.Size(160, 19);
            this.TMIDIs.TabIndex = 7;
            this.TMIDIs.Text = "Total MIDIs: 0";
            this.TMIDIs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FNVal
            // 
            this.FNVal.BackColor = System.Drawing.SystemColors.Control;
            this.FNVal.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FNVal.Location = new System.Drawing.Point(85, 19);
            this.FNVal.Multiline = false;
            this.FNVal.Name = "FNVal";
            this.FNVal.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.FNVal.Size = new System.Drawing.Size(503, 15);
            this.FNVal.TabIndex = 2;
            this.FNVal.TabStop = false;
            this.FNVal.Text = "No MIDI selected";
            // 
            // CMIDIs
            // 
            this.CMIDIs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CMIDIs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CMIDIs.Location = new System.Drawing.Point(595, 15);
            this.CMIDIs.Name = "CMIDIs";
            this.CMIDIs.Size = new System.Drawing.Size(160, 23);
            this.CMIDIs.TabIndex = 4;
            this.CMIDIs.Text = "Convert MIDIs";
            this.CMIDIs.UseVisualStyleBackColor = true;
            this.CMIDIs.Click += new System.EventHandler(this.CMIDIs_Click);
            // 
            // FPVal
            // 
            this.FPVal.BackColor = System.Drawing.SystemColors.Control;
            this.FPVal.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FPVal.Location = new System.Drawing.Point(85, 37);
            this.FPVal.Multiline = false;
            this.FPVal.Name = "FPVal";
            this.FPVal.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.FPVal.Size = new System.Drawing.Size(503, 15);
            this.FPVal.TabIndex = 3;
            this.FPVal.TabStop = false;
            this.FPVal.Text = "No MIDI selected";
            // 
            // COS
            // 
            this.COS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.COS.Location = new System.Drawing.Point(595, 82);
            this.COS.Name = "COS";
            this.COS.Size = new System.Drawing.Size(160, 23);
            this.COS.TabIndex = 6;
            this.COS.Text = "Change output settings";
            this.COS.UseVisualStyleBackColor = true;
            this.COS.Click += new System.EventHandler(this.COS_Click);
            // 
            // TLVal
            // 
            this.TLVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TLVal.Location = new System.Drawing.Point(82, 55);
            this.TLVal.Name = "TLVal";
            this.TLVal.Size = new System.Drawing.Size(506, 15);
            this.TLVal.TabIndex = 11;
            this.TLVal.Text = "0:00.000";
            // 
            // ESFL
            // 
            this.ESFL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ESFL.Location = new System.Drawing.Point(595, 105);
            this.ESFL.Name = "ESFL";
            this.ESFL.Size = new System.Drawing.Size(160, 23);
            this.ESFL.TabIndex = 7;
            this.ESFL.Text = "Edit SoundFonts list";
            this.ESFL.UseVisualStyleBackColor = true;
            this.ESFL.Click += new System.EventHandler(this.ESFL_Click);
            // 
            // TLLab
            // 
            this.TLLab.AutoSize = true;
            this.TLLab.Location = new System.Drawing.Point(6, 55);
            this.TLLab.Name = "TLLab";
            this.TLLab.Size = new System.Drawing.Size(47, 15);
            this.TLLab.TabIndex = 10;
            this.TLLab.Text = "Length:";
            // 
            // SVal
            // 
            this.SVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SVal.Location = new System.Drawing.Point(82, 109);
            this.SVal.Name = "SVal";
            this.SVal.Size = new System.Drawing.Size(506, 15);
            this.SVal.TabIndex = 9;
            this.SVal.Text = "0.00 B";
            // 
            // SLab
            // 
            this.SLab.AutoSize = true;
            this.SLab.Location = new System.Drawing.Point(6, 109);
            this.SLab.Name = "SLab";
            this.SLab.Size = new System.Drawing.Size(30, 15);
            this.SLab.TabIndex = 8;
            this.SLab.Text = "Size:";
            // 
            // TVal
            // 
            this.TVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TVal.Location = new System.Drawing.Point(82, 73);
            this.TVal.Name = "TVal";
            this.TVal.Size = new System.Drawing.Size(506, 15);
            this.TVal.TabIndex = 7;
            this.TVal.Text = "0";
            // 
            // TLab
            // 
            this.TLab.AutoSize = true;
            this.TLab.Location = new System.Drawing.Point(6, 73);
            this.TLab.Name = "TLab";
            this.TLab.Size = new System.Drawing.Size(42, 15);
            this.TLab.TabIndex = 6;
            this.TLab.Text = "Tracks:";
            // 
            // NCVal
            // 
            this.NCVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NCVal.Location = new System.Drawing.Point(82, 91);
            this.NCVal.Name = "NCVal";
            this.NCVal.Size = new System.Drawing.Size(506, 15);
            this.NCVal.TabIndex = 5;
            this.NCVal.Text = "0";
            // 
            // NCLab
            // 
            this.NCLab.AutoSize = true;
            this.NCLab.Location = new System.Drawing.Point(6, 91);
            this.NCLab.Name = "NCLab";
            this.NCLab.Size = new System.Drawing.Size(70, 15);
            this.NCLab.TabIndex = 4;
            this.NCLab.Text = "Note count:";
            // 
            // FPLab
            // 
            this.FPLab.AutoSize = true;
            this.FPLab.Location = new System.Drawing.Point(6, 37);
            this.FPLab.Name = "FPLab";
            this.FPLab.Size = new System.Drawing.Size(56, 15);
            this.FPLab.TabIndex = 1;
            this.FPLab.Text = "Full path:";
            // 
            // FNLab
            // 
            this.FNLab.AutoSize = true;
            this.FNLab.Location = new System.Drawing.Point(6, 19);
            this.FNLab.Name = "FNLab";
            this.FNLab.Size = new System.Drawing.Size(58, 15);
            this.FNLab.TabIndex = 0;
            this.FNLab.Text = "Filename:";
            // 
            // MIDIQueue
            // 
            this.MIDIQueue.AllowDrop = true;
            this.MIDIQueue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MIDIQueue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MIDIQueue.FormattingEnabled = true;
            this.MIDIQueue.IntegralHeight = false;
            this.MIDIQueue.ItemHeight = 15;
            this.MIDIQueue.Location = new System.Drawing.Point(12, 12);
            this.MIDIQueue.Name = "MIDIQueue";
            this.MIDIQueue.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.MIDIQueue.Size = new System.Drawing.Size(760, 376);
            this.MIDIQueue.TabIndex = 1;
            this.MIDIQueue.SelectedIndexChanged += new System.EventHandler(this.MIDIQueue_SelectedIndexChanged);
            this.MIDIQueue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MIDIQueue_KeyDown);
            this.MIDIQueue.DragEnter += new System.Windows.Forms.DragEventHandler(this.MIDIQueue_DragEnter);
            this.MIDIQueue.DragDrop += new System.Windows.Forms.DragEventHandler(this.MIDIQueue_DragDrop);
            // 
            // OCContextMenu
            // 
            this.OCContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.AddMIDIsToQueueRC,
            this.RemoveMIDIsFromQueueRC,
            this.ClearQueueRC});
            // 
            // AddMIDIsToQueueRC
            // 
            this.AddMIDIsToQueueRC.Index = 0;
            this.AddMIDIsToQueueRC.Text = "Add MIDIs to the queue";
            this.AddMIDIsToQueueRC.Click += new System.EventHandler(this.AddMIDIsToQueue_Click);
            // 
            // RemoveMIDIsFromQueueRC
            // 
            this.RemoveMIDIsFromQueueRC.Index = 1;
            this.RemoveMIDIsFromQueueRC.Text = "Remove MIDIs from the queue";
            this.RemoveMIDIsFromQueueRC.Click += new System.EventHandler(this.RemoveMIDIsFromQueue_Click);
            // 
            // ClearQueueRC
            // 
            this.ClearQueueRC.Index = 2;
            this.ClearQueueRC.Text = "Clear queue";
            this.ClearQueueRC.Click += new System.EventHandler(this.ClearQueue_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 540);
            this.Controls.Add(this.MIDIQueue);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OmniConverter";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.OCMenuIcons)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private wyDay.Controls.VistaMenu OCMenuIcons;
        private System.Windows.Forms.MainMenu OCMenu;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem AddMIDIsToQueue;
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
        private System.Windows.Forms.MenuItem RemoveMIDIsFromQueue;
        private System.Windows.Forms.MenuItem ClearQueue;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem ExitFromConverter;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem InfoAboutConverter;
        private System.Windows.Forms.MenuItem CreateIssueGitHub;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem CheckForUpdates;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem DownloadConvSrc;
        private System.Windows.Forms.ContextMenu OCContextMenu;
        private System.Windows.Forms.MenuItem AddMIDIsToQueueRC;
        private System.Windows.Forms.MenuItem RemoveMIDIsFromQueueRC;
        private System.Windows.Forms.MenuItem ClearQueueRC;
    }
}

