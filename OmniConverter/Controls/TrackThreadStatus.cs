using System;
using System.Windows.Forms;

namespace OmniConverter
{
    public partial class TrackThreadStatus : UserControl
    {
        private bool IsInUse = false;

        public TrackThreadStatus()
        {
            InitializeComponent();
        }

        public void UpdatePB(Int32 PBV)
        {
            TrckPB.Value = PBV.LimitToRange(0, 100);
        }

        public bool IsFree() { return !IsInUse; }

        public void Use(Int32 Track) 
        { 
            IsInUse = true;
            Trck.Text = String.Format("Track {0}", Track);
        }

        public void Done() { IsInUse = false; }
    }
}
