namespace OmniConverter
{
    partial class TaskStatus
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

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            MIDIPB = new System.Windows.Forms.ProgressBar();
            MIDIT = new System.Windows.Forms.RichTextBox();
            SuspendLayout();
            // 
            // MIDIPB
            // 
            MIDIPB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            MIDIPB.Location = new System.Drawing.Point(3, 19);
            MIDIPB.Name = "MIDIPB";
            MIDIPB.Size = new System.Drawing.Size(287, 13);
            MIDIPB.TabIndex = 1;
            // 
            // MIDIT
            // 
            MIDIT.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            MIDIT.BackColor = System.Drawing.SystemColors.Control;
            MIDIT.BorderStyle = System.Windows.Forms.BorderStyle.None;
            MIDIT.Location = new System.Drawing.Point(3, 3);
            MIDIT.Multiline = false;
            MIDIT.Name = "MIDIT";
            MIDIT.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            MIDIT.Size = new System.Drawing.Size(287, 16);
            MIDIT.TabIndex = 3;
            MIDIT.TabStop = false;
            MIDIT.Text = "N/A";
            // 
            // MIDIThreadStatus
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(MIDIT);
            Controls.Add(MIDIPB);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "MIDIThreadStatus";
            Size = new System.Drawing.Size(293, 35);
            ResumeLayout(false);
        }

        #endregion
        public System.Windows.Forms.ProgressBar MIDIPB;
        private System.Windows.Forms.RichTextBox MIDIT;
    }
}
