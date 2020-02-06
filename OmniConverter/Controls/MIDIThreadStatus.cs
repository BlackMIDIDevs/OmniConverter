using System;
using System.Windows.Forms;

namespace OmniConverter
{
    public partial class MIDIThreadStatus : UserControl
    {
        public MIDIThreadStatus(String MIDITitle)
        {
            InitializeComponent();

            MIDIT.Text = MIDITitle;
        }

        public void UpdatePB(Int32 PBV)
        {
            MIDIPB.Value = PBV.LimitToRange(0, 100);
        }
    }
}
