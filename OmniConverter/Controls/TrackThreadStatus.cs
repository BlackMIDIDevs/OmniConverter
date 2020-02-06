using System;
using System.Windows.Forms;

namespace OmniConverter
{
    public partial class TrackThreadStatus : UserControl
    {
        public TrackThreadStatus(Int32 Track)
        {
            InitializeComponent();

            Trck.Text = String.Format("Track {0}", Track);
        }

        public void UpdatePB(Int32 PBV)
        {
            TrckPB.Value = PBV.LimitToRange(0, 100);
        }
    }
}
