using Microsoft.WindowsAPICodePack.Dialogs;
using Octokit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace OmniConverter
{
    public partial class MainWindow : Form
    {
        public MainWindow(String[] MIDIs)
        {
            InitializeComponent();

            this.MainMenuStrip = OCMenu;
            MIDIQueue.ContextMenuStrip = OCContextMenu;
            VolBar.Value = (int)(Properties.Settings.Default.Volume * 10000);
            VolBar_Scroll(null, null);

            if (MIDIs.Length > 0)
                new MIDIImporter(MIDIs, true).ShowDialog();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            RebindList();
            GetSelectedMIDIInfo();

            Debug.PrintToConsole("ok", "Icons set.");
        }

        private void RebindList()
        {
            Debug.PrintToConsole("ok", "Rebinding Program.MIDIList to MIDIQueue...");

            MIDIQueue.DataSource = null;
            MIDIQueue.DataSource = Program.MIDIList;
            MIDIQueue.DisplayMember = "Name";

            Debug.PrintToConsole("ok", "MIDIQueue bound successfully.");
        }

        private void GetSelectedMIDIInfo()
        {
            Boolean AtLeastOne = MIDIQueue.SelectedItems.Count > 0, MultiSelect = MIDIQueue.SelectedItems.Count > 1, TooMany = MIDIQueue.SelectedItems.Count > 4;

            InfoBox.Text = String.Format("Information (Total MIDIs in queue: {0})", MIDIQueue.Items.Count.ToString("N0", new CultureInfo("is-IS")));
            Debug.PrintToConsole("ok", "GetSelectedMIDIInfo() called.");

            while (AtLeastOne)
            {
                String FN = "", FP = "", NC = "", TL = "", T = "", S = "", Sep = ", ";
                TimeSpan L;

                if (MultiSelect)
                {
                    Int32 I = 0;

                    if (TooMany)
                        break;

                    foreach (MIDI MFile in MIDIQueue.SelectedItems)
                    {
                        Boolean NMT = I < 1;

                        L = MFile.TimeLength;
                        FN += String.Format("{0}{1}", NMT ? "" : Sep, MFile.Name);
                        FP += String.Format("{0}{1}", NMT ? "" : Sep, MFile.Path);
                        NC += String.Format("{0}{1}", NMT ? "" : Sep, MFile.NoteCount.ToString("N0", new CultureInfo("is-IS")));
                        TL += String.Format("{0}{1}:{2}.{3}", NMT ? "" : Sep, L.Minutes, L.Seconds.ToString().PadLeft(2, '0'), L.Milliseconds.ToString().PadLeft(3, '0'));
                        T += String.Format("{0}{1}", NMT ? "" : Sep, MFile.Tracks.ToString("N0", new CultureInfo("is-IS")));
                        S += String.Format("{0}{1}", NMT ? "" : Sep, DataCheck.BytesToHumanReadableSize(MFile.Size));

                        I++;
                    }
                }
                else
                {
                    L = ((MIDI)MIDIQueue.SelectedItem).TimeLength;
                    FN = String.Format("{0}", ((MIDI)MIDIQueue.SelectedItem).Name);
                    FP = String.Format("{0}", ((MIDI)MIDIQueue.SelectedItem).Path);
                    NC = String.Format("{0}", ((MIDI)MIDIQueue.SelectedItem).NoteCount.ToString("N0", new CultureInfo("is-IS")));
                    TL = String.Format("{0}:{1}.{2}", L.Minutes, L.Seconds.ToString().PadLeft(2, '0'), L.Milliseconds.ToString().PadLeft(3, '0'));
                    T = String.Format("{0}", ((MIDI)MIDIQueue.SelectedItem).Tracks.ToString("N0", new CultureInfo("is-IS")));
                    S = String.Format("{0}", DataCheck.BytesToHumanReadableSize(((MIDI)MIDIQueue.SelectedItem).Size));
                }

                FNVal.Text = FN;
                FPVal.Text = FP;
                NCVal.Text = NC;
                TLVal.Text = TL;
                TVal.Text = T;
                SVal.Text = S;

                return;
            }

            FNVal.Text = TooMany ? "Too many MIDIs selected" : "No MIDI selected";
            FPVal.Text = TooMany ? "Too many MIDIs selected" : "No MIDI selected";
            NCVal.Text = "-";
            TLVal.Text = "-:--.---";
            TVal.Text = "-";
            SVal.Text = "-.-- -";
        }

        private void MIDIQueue_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetSelectedMIDIInfo();
        }

        private void AddMIDIsToQueue_Click(object sender, EventArgs e)
        {
            OpenFileDialog MIDIImport = new OpenFileDialog();
            MIDIImport.InitialDirectory = (Properties.Settings.Default.LastPathMIDIImport.ToLowerInvariant() == "null" ?
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) : Properties.Settings.Default.LastPathMIDIImport);
            MIDIImport.Filter = String.Format("{0}|*.mid;*.midi;*.kar;*.rmi;*.riff", "MIDI files");
            MIDIImport.Multiselect = true;

            if (MIDIImport.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.LastPathMIDIImport = Path.GetDirectoryName(MIDIImport.FileNames[0]);
                Properties.Settings.Default.Save();

                new MIDIImporter(MIDIImport.FileNames, false).ShowDialog();

                RebindList();
                GetSelectedMIDIInfo();
            }
        }

        private void RemoveMIDIsFromQueue_Click(object sender, EventArgs e)
        {
            foreach (MIDI Item in MIDIQueue.SelectedItems)
            {
                Program.MIDIList.Remove(Item);
                Item.Dispose();
                Debug.PrintToConsole("ok", String.Format("Removed {0} from list.", Item.Name));
            }

            RebindList();
            GetSelectedMIDIInfo();
        }

        private void ClearQueue_Click(object sender, EventArgs e)
        {
            Program.MIDIList.ForEach(x => x.Dispose());
            Program.MIDIList = new List<MIDI>();
            RebindList();
            GetSelectedMIDIInfo();
        }

        private void ExitFromConverter_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CMIDIs_Click(object sender, EventArgs e)
        {
            String Path = (Properties.Settings.Default.AOFPath.ToLowerInvariant() == "null" ?
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) : Properties.Settings.Default.AOFPath);

            CommonFileDialogResult Result = CommonFileDialogResult.None;
            CommonOpenFileDialog MIDIExport = new CommonOpenFileDialog();
            MIDIExport.InitialDirectory = (Properties.Settings.Default.LastPathAudioExport.ToLowerInvariant() == "null" ?
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) : Properties.Settings.Default.LastPathAudioExport);
            MIDIExport.IsFolderPicker = true;

            if (Program.MIDIList.Count < 1)
            {
                Debug.ShowMsgBox("Warning", "You need to add MIDIs to the queue to start the conversion process.",
                    null, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Program.SFArray.List.Count < 1)
            {
                Debug.ShowMsgBox("Warning", "You need to add SoundFonts to the SoundFonts list to start the conversion process.",
                    null, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Properties.Settings.Default.AutoOutputFolder)
            {
                Debug.PrintToConsole("ok", "AutoOutputFolder is false, the converter will export to the folder selected in MIDIExport.");

                Result = MIDIExport.ShowDialog();
                if (Result == CommonFileDialogResult.Ok)
                {
                    Debug.PrintToConsole("ok", String.Format("MIDIExport.FileName: {0}", MIDIExport.FileName));
                    Properties.Settings.Default.LastPathAudioExport = MIDIExport.FileName;
                    Properties.Settings.Default.Save();

                    Path = MIDIExport.FileName;
                }
            }
            else
            {
                Debug.PrintToConsole("ok", "AutoOutputFolder is true, the converter will export to the specified folder.");
                Debug.PrintToConsole("ok", String.Format("AOFPath: {0}", Properties.Settings.Default.AOFPath));
                Result = CommonFileDialogResult.Ok;
            }

            if (!Directory.Exists(Path))
            {
                Debug.ShowMsgBox("Error", "The output folder does not exist.\n\nPlease check the automatic output folder set in the settings.",
                    null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Result == CommonFileDialogResult.Ok)
            {
                MIDIConverter Conv = new MIDIConverter(Path);
                Conv.ShowDialog();

                if (!Conv.StopRequested && Properties.Settings.Default.DoActionAfterRender)
                {
                    System.Diagnostics.ProcessStartInfo PSI;
                    Version OSVer = Environment.OSVersion.Version;
                    string HybridShutdown = "";

                    if (OSVer.Major == 6 && OSVer.Minor >= 2 || OSVer.Major >= 6)
                        HybridShutdown = " /hybrid";

                    switch (Properties.Settings.Default.DoActionAfterRenderV)
                    {
                        case 0:
                            Application.SetSuspendState(PowerState.Suspend, true, false);
                            break;
                        case 1:
                            Application.SetSuspendState(PowerState.Hibernate, true, false);
                            break;
                        case 2:
                            PSI = new System.Diagnostics.ProcessStartInfo("shutdown", String.Format("/s{0} /t 15 /c \"Automatic shutdown through OmniConverter.\"", HybridShutdown));
                            PSI.CreateNoWindow = true;
                            PSI.UseShellExecute = false;
                            System.Diagnostics.Process.Start(PSI);
                            break;
                        case 3:
                            PSI = new System.Diagnostics.ProcessStartInfo("shutdown", "/r /t 15 /c \"Automatic restart through OmniConverter.\"");
                            PSI.CreateNoWindow = true;
                            PSI.UseShellExecute = false;
                            System.Diagnostics.Process.Start(PSI);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void VolBar_Scroll(object sender, EventArgs e)
        {
            if (sender != null && e != null)
            {
                Properties.Settings.Default.Volume = (float)Convert.ToDouble(VolBar.Value / 10000.0f);
                Properties.Settings.Default.Save();
            }

            VolLab.Text = $"({20 * Math.Log10(Properties.Settings.Default.Volume / 1.0f):0.00}dB) {VolBar.Value / 100.0f:000.00}%";
        }

        private void MIDIQueue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                RemoveMIDIsFromQueue_Click(null, null);
        }

        private void MIDIQueue_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void MIDIQueue_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            Debug.PrintToConsole("ok", $"DragDrop detected, {files.Length} files.");

            if (files.Length > 0)
            {
                var t = new MIDIImporter(files, false);

                t.ShowDialog();

                if (t.TotalValidFiles() > 0)
                {
                    RebindList();
                    GetSelectedMIDIInfo();
                }
            }
        }

        private void COS_Click(object sender, EventArgs e)
        {
            Debug.PrintToConsole("ok", "Settings window requested.");
            new Settings().ShowDialog();
        }

        private void ESFL_Click(object sender, EventArgs e)
        {
            Debug.PrintToConsole("ok", "SoundFontsList window requested.");
            new SoundFontsList().ShowDialog();
        }

        private void InfoAboutConverter_Click(object sender, EventArgs e)
        {
            Debug.PrintToConsole("ok", "InfoWindow window requested.");
            new InfoWindow().ShowDialog();
        }

        private void CheckForUpdates_Click(object sender, EventArgs e)
        {
            UpdateSystem.CheckForUpdates((Control.ModifierKeys == Keys.Shift), false, false);
        }
    }
}
