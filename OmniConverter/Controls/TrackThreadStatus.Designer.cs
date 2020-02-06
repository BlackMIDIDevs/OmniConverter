namespace OmniConverter
{
    partial class TrackThreadStatus
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
            this.Trck = new System.Windows.Forms.Label();
            this.TrckPB = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // Trck
            // 
            this.Trck.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Trck.AutoSize = true;
            this.Trck.Location = new System.Drawing.Point(2, 10);
            this.Trck.Name = "Trck";
            this.Trck.Size = new System.Drawing.Size(70, 15);
            this.Trck.TabIndex = 0;
            this.Trck.Text = "Track 65.535";
            // 
            // TrckPB
            // 
            this.TrckPB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrckPB.Location = new System.Drawing.Point(87, 4);
            this.TrckPB.Name = "TrckPB";
            this.TrckPB.Size = new System.Drawing.Size(202, 27);
            this.TrckPB.TabIndex = 1;
            // 
            // TrackThreadStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TrckPB);
            this.Controls.Add(this.Trck);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TrackThreadStatus";
            this.Size = new System.Drawing.Size(293, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Trck;
        public System.Windows.Forms.ProgressBar TrckPB;
    }
}
