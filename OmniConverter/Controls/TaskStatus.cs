using MIDIModificationFramework;
using System;
using System.Windows.Forms;

namespace OmniConverter
{
    public partial class TaskStatus : UserControl
    {
        string OgTitle = string.Empty;

        public TaskStatus(String MIDITitle)
        {
            InitializeComponent();

            OgTitle = MIDITitle;
            MIDIT.Text = OgTitle;
        }

        public void UpdateTitle(string Text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => UpdateTitle(Text)), null);
                return;
            }

            if (!string.IsNullOrEmpty(Text))
                MIDIT.Text = $"{OgTitle} - {Text}";
            else
                MIDIT.Text = OgTitle;
        }

        public void UpdatePBStyle(ProgressBarStyle PBM)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => UpdatePBStyle(PBM)), null);
                return;
            }

            MIDIPB.Style = PBM;
        }

        public void UpdatePB(int PBV)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => UpdatePB(PBV)), null);
                return;
            }

            MIDIPB.Value = PBV.LimitToRange(0, 100);
        }
    }
}
