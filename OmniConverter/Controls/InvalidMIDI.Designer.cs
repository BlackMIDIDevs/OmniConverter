namespace OmniConverter
{
    partial class InvalidMIDI
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
            this.BrokenMIDITitle = new System.Windows.Forms.RichTextBox();
            this.BrokenReason = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // BrokenMIDITitle
            // 
            this.BrokenMIDITitle.BackColor = System.Drawing.SystemColors.Control;
            this.BrokenMIDITitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.BrokenMIDITitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.BrokenMIDITitle.EnableAutoDragDrop = true;
            this.BrokenMIDITitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BrokenMIDITitle.ForeColor = System.Drawing.Color.DarkRed;
            this.BrokenMIDITitle.Location = new System.Drawing.Point(0, 0);
            this.BrokenMIDITitle.Multiline = false;
            this.BrokenMIDITitle.Name = "BrokenMIDITitle";
            this.BrokenMIDITitle.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.BrokenMIDITitle.Size = new System.Drawing.Size(293, 17);
            this.BrokenMIDITitle.TabIndex = 3;
            this.BrokenMIDITitle.TabStop = false;
            this.BrokenMIDITitle.Text = "N/A";
            // 
            // BrokenReason
            // 
            this.BrokenReason.BackColor = System.Drawing.SystemColors.Control;
            this.BrokenReason.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.BrokenReason.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BrokenReason.Location = new System.Drawing.Point(0, 17);
            this.BrokenReason.Multiline = false;
            this.BrokenReason.Name = "BrokenReason";
            this.BrokenReason.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.BrokenReason.Size = new System.Drawing.Size(293, 17);
            this.BrokenReason.TabIndex = 4;
            this.BrokenReason.TabStop = false;
            this.BrokenReason.Text = "N/A";
            // 
            // InvalidMIDI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BrokenReason);
            this.Controls.Add(this.BrokenMIDITitle);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "InvalidMIDI";
            this.Size = new System.Drawing.Size(293, 34);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox BrokenMIDITitle;
        private System.Windows.Forms.RichTextBox BrokenReason;
    }
}
