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
            BrokenMIDITitle = new System.Windows.Forms.RichTextBox();
            BrokenReason = new System.Windows.Forms.RichTextBox();
            SuspendLayout();
            // 
            // BrokenMIDITitle
            // 
            BrokenMIDITitle.BackColor = System.Drawing.SystemColors.Control;
            BrokenMIDITitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            BrokenMIDITitle.Dock = System.Windows.Forms.DockStyle.Top;
            BrokenMIDITitle.EnableAutoDragDrop = true;
            BrokenMIDITitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            BrokenMIDITitle.ForeColor = System.Drawing.Color.DarkRed;
            BrokenMIDITitle.Location = new System.Drawing.Point(0, 0);
            BrokenMIDITitle.Multiline = false;
            BrokenMIDITitle.Name = "BrokenMIDITitle";
            BrokenMIDITitle.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            BrokenMIDITitle.Size = new System.Drawing.Size(293, 17);
            BrokenMIDITitle.TabIndex = 3;
            BrokenMIDITitle.TabStop = false;
            BrokenMIDITitle.Text = "N/A";
            // 
            // BrokenReason
            // 
            BrokenReason.BackColor = System.Drawing.SystemColors.Control;
            BrokenReason.BorderStyle = System.Windows.Forms.BorderStyle.None;
            BrokenReason.Dock = System.Windows.Forms.DockStyle.Bottom;
            BrokenReason.Location = new System.Drawing.Point(0, 17);
            BrokenReason.Multiline = false;
            BrokenReason.Name = "BrokenReason";
            BrokenReason.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            BrokenReason.Size = new System.Drawing.Size(293, 17);
            BrokenReason.TabIndex = 4;
            BrokenReason.TabStop = false;
            BrokenReason.Text = "N/A";
            // 
            // InvalidMIDI
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(BrokenReason);
            Controls.Add(BrokenMIDITitle);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "InvalidMIDI";
            Size = new System.Drawing.Size(293, 34);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.RichTextBox BrokenMIDITitle;
        private System.Windows.Forms.RichTextBox BrokenReason;
    }
}
