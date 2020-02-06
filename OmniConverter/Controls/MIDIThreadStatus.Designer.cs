namespace OmniConverter
{
    partial class MIDIThreadStatus
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
            this.MIDIPB = new System.Windows.Forms.ProgressBar();
            this.MIDIT = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // MIDIPB
            // 
            this.MIDIPB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MIDIPB.Location = new System.Drawing.Point(3, 19);
            this.MIDIPB.Name = "MIDIPB";
            this.MIDIPB.Size = new System.Drawing.Size(287, 13);
            this.MIDIPB.TabIndex = 1;
            // 
            // MIDIT
            // 
            this.MIDIT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MIDIT.BackColor = System.Drawing.SystemColors.Control;
            this.MIDIT.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MIDIT.Location = new System.Drawing.Point(3, 3);
            this.MIDIT.Multiline = false;
            this.MIDIT.Name = "MIDIT";
            this.MIDIT.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.MIDIT.Size = new System.Drawing.Size(287, 13);
            this.MIDIT.TabIndex = 3;
            this.MIDIT.TabStop = false;
            this.MIDIT.Text = "N/A";
            // 
            // MIDIThreadStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MIDIT);
            this.Controls.Add(this.MIDIPB);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MIDIThreadStatus";
            this.Size = new System.Drawing.Size(293, 35);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.ProgressBar MIDIPB;
        private System.Windows.Forms.RichTextBox MIDIT;
    }
}
