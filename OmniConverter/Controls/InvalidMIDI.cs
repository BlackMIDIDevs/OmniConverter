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
            BrokenMIDITitle.Text = $"ERROR ON \"{MIDITitle}\"!";
            BrokenReason.Text = Error;
        }
    }
}
