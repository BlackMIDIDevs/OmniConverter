using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace OmniConverter
{
    public partial class TrackThreadStatus : UserControl
    {
        private bool IsInUse = false;

        public TrackThreadStatus(Int64 Track)
        {
            InitializeComponent();

            Trck.Text = String.Format("Track {0}", Track);
        }

        public void UpdatePB(Int32 PBV)
        {
            TrckPB.Value = PBV.LimitToRange(0, 100);
        }

        public bool IsFree() { return !IsInUse; }

        public void Done() { IsInUse = false; }
    }
}
