using System;
using System.Windows.Forms;

namespace OmniConverter
{
    public partial class InvalidMIDI : UserControl
    {
        public InvalidMIDI(String MIDITitle, String Error, System.Drawing.Color Color)
        {
            InitializeComponent();

            BrokenMIDITitle.ForeColor = Color;
            BrokenMIDITitle.Text = String.Format("Could not load {0}!", MIDITitle);
            BrokenReason.Text = String.Format("Reason: {0}", Error);
        }
    }
}
