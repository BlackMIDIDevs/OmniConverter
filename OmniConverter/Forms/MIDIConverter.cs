using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Un4seen.Bass;
using CSCore;
using CSCore.Codecs.WAV;

namespace OmniConverter
{
    public partial class MIDIConverter : Form
    {
        private Converter Cnv;
        public Boolean StopRequested = false;

        public MIDIConverter(String OutputPath)
        {
            InitializeComponent();

            Cnv = new Converter(this, ThreadsPanel, OutputPath);
        }

        private void MIDIConverter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Alt || Control.ModifierKeys == Keys.F4)
            {
                e.Cancel = true;
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            CancelBtn.Enabled = false;
            new Thread(() =>
            {
                Int32 SleepCount = 0;
                DialogResult DR1 = 0, DR2 = 0;

                if (Cnv.IsStillRendering())
                {
                    Debug.PrintToConsole("wrn", "CThread is still alive! Asking if user wants to quit.");

                    this.Invoke((MethodInvoker)delegate { 
                        DR1 = MessageBox.Show("Are you sure you want to terminate the conversion process?", "The converter is still processing data", MessageBoxButtons.YesNo, MessageBoxIcon.Warning); 
                    });
                    
                    switch (DR1)
                    {
                        case DialogResult.Yes:
                            StopRequested = true;
                            Cnv.RequestStop();

                            while (Cnv.IsStillRendering())
                            {
                                SleepCount++;
                                Thread.Sleep(10);

                                if (SleepCount >= 500) break;

                                Debug.PrintToConsole("wrn", "CThread is still alive! Waiting...");
                            }

                            if (SleepCount >= 5000)
                            {
                                Debug.PrintToConsole("err", "CThread is still alive!");

                                this.Invoke((MethodInvoker)delegate {
                                    DR2 = MessageBox.Show("The conversion threads seem to have got stuck, are you sure you want to continue?\n\nThis could cause unexpected behavior.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                });

                                switch (DR2)
                                {
                                    case DialogResult.Yes:
                                        Cnv.ForceStop();
                                        break;
                                    default:
                                    case DialogResult.No:
                                        return;
                                }
                            }

                            Debug.PrintToConsole("ok", "CThread is not active anymore.");
                            break;
                        default:
                        case DialogResult.No:
                            return;
                    }
                }

                this.Invoke((MethodInvoker)delegate { Close(); });
            }).Start();
        }

        /*
         * These two SoundFont functions are needed to avoid loading the same SoundFont's samples twice,
         * since BASSMIDI isn't really multithread-friendly
         */

        // Yes

        private void Check_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Cnv == null) return;

                if (!Cnv.IsStillRendering())
                {
                    switch (Cnv.GetStatus())
                    {
                        default:
                        case "prep":
                            StatusLab.Text = "The rendering thread is booting up.\n\nPlease wait...";
                            PB.Style = ProgressBarStyle.Marquee;
                            PB.Value = 0;
                            TPB.Style = ProgressBarStyle.Marquee;
                            TPB.Value = 0;
                            break;
                        case "crsh":
                            StatusLab.Text = Cnv.GetError();
                            PB.Style = ProgressBarStyle.Blocks;
                            PB.State = wyDay.Controls.ProgressBarState.Error;
                            PB.Value = 100;
                            TPB.Style = ProgressBarStyle.Blocks;
                            TPB.State = wyDay.Controls.ProgressBarState.Error;
                            TPB.Value = 100;
                            break;
                    }
                }
                else
                {
                    UInt64 ValidFiles = Cnv.MDV.GetValidMIDIsCount();
                    UInt64 InvalidFiles = Cnv.MDV.GetInvalidMIDIsCount();
                    UInt64 TotalFiles = Cnv.MDV.GetTotalMIDIsCount();

                    Int64 Tracks = Cnv.MDV.GetTotalTracks();
                    Int64 CurrentTrack = Cnv.MDV.GetCurrentTrack();

                    switch (Cnv.GetStatus())
                    {
                        default:
                        case "prep":
                            StatusLab.Text = "The converter is preparing itself for the conversion process.\n\nPlease wait...";
                            PB.Style = ProgressBarStyle.Marquee;
                            PB.Value = 0;
                            PB.Size = new System.Drawing.Size(PB.Size.Width, 26);
                            TPB.Visible = false;
                            break;
                        case "mconv":
                            StatusLab.Text = String.Format("{0} file(s) out of {1} have been converted.\nRendered {2} track(s) out of {3}.\nPlease wait...",
                                (ValidFiles + InvalidFiles).ToString("N0", new CultureInfo("is-IS")),
                                TotalFiles.ToString("N0", new CultureInfo("is-IS")),
                                CurrentTrack.ToString("N0", new CultureInfo("is-IS")), Tracks.ToString("N0", new CultureInfo("is-IS")));

                            PB.Style = ProgressBarStyle.Blocks;
                            PB.Value = Convert.ToInt32(Math.Round((ValidFiles + InvalidFiles) * 100.0 / TotalFiles));
                            PB.Size = new System.Drawing.Size(PB.Size.Width, 13);
                            TPB.Visible = true;
                            TPB.Style = ProgressBarStyle.Blocks;
                            TPB.Value = Convert.ToInt32(Math.Round(CurrentTrack * 100.0 / Tracks));
                            break;
                        case "sconv":
                            StatusLab.Text = String.Format("{0} file(s) out of {1} have been converted.\n\nPlease wait...",
                                (ValidFiles + InvalidFiles).ToString("N0", new CultureInfo("is-IS")),
                                TotalFiles.ToString("N0", new CultureInfo("is-IS")),
                                CurrentTrack.ToString("N0", new CultureInfo("is-IS")), Tracks.ToString("N0", new CultureInfo("is-IS")));

                            PB.Style = ProgressBarStyle.Blocks;
                            PB.Value = Convert.ToInt32(Math.Round((ValidFiles + InvalidFiles) * 100.0 / TotalFiles));
                            PB.Size = new System.Drawing.Size(PB.Size.Width, 26);
                            TPB.Visible = false;
                            break;
                        case "aout":
                            StatusLab.Text = "Writing final audio file to disk.\n\nPlease do not turn off the computer during the process...";
                            PB.Style = ProgressBarStyle.Blocks;
                            PB.Value = Convert.ToInt32(Math.Round((ValidFiles + InvalidFiles) * 100.0 / TotalFiles));
                            TPB.Style = ProgressBarStyle.Blocks;
                            TPB.Value = Convert.ToInt32(Math.Round(CurrentTrack * 100.0 / Tracks));
                            break;
                    }
                }

            }
            catch { }
        }

        private void MIDIConverter_Load(object sender, EventArgs e)
        {

        }
    }
}
