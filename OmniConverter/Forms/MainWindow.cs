using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmniConverter
{
    public partial class MainWindow : Form
    {
        public MainWindow(String[] MIDIs)
        {
            InitializeComponent();

            this.Menu = OCMenu;
            MIDIQueue.ContextMenu = OCContextMenu;

            if (MIDIs.Length > 0)
                new MIDIImporter(MIDIs, true).ShowDialog();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            RebindList();
            GetSelectedMIDIInfo();

            OCMenuIcons.SetImage(AddMIDIsToQueue, IconSystem.Add);
            OCMenuIcons.SetImage(RemoveMIDIsFromQueue, IconSystem.Remove);
            OCMenuIcons.SetImage(ClearQueue, IconSystem.Clear);
            OCMenuIcons.SetImage(ExitFromConverter, IconSystem.Sleep);
            OCMenuIcons.SetImage(InfoAboutConverter, IconSystem.Info);
            OCMenuIcons.SetImage(CreateIssueGitHub, IconSystem.Octocat);
            OCMenuIcons.SetImage(CheckForUpdates, IconSystem.Download);
            OCMenuIcons.SetImage(DownloadConvSrc, IconSystem.Empty);
    
            OCMenuIcons.SetImage(AddMIDIsToQueueRC, IconSystem.Add);
            OCMenuIcons.SetImage(RemoveMIDIsFromQueueRC, IconSystem.Remove);
            OCMenuIcons.SetImage(ClearQueueRC, IconSystem.Clear);
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

            TMIDIs.Text = String.Format("Total MIDIs in queue: {0}", MIDIQueue.Items.Count.ToString("N0", new CultureInfo("is-IS")));
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
                Debug.PrintToConsole("ok", String.Format("Removed {0} from list.", Item.Name));
            }

            RebindList();
            GetSelectedMIDIInfo();
        }

        private void ClearQueue_Click(object sender, EventArgs e)
        {
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
                    String HybridShutdown = "";

                    if (OSVer.Major == 6 && OSVer.Minor >= 2 || OSVer.Major >= 10)
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
                            PSI = new System.Diagnostics.ProcessStartInfo("shutdown", String.Format("/s{0} /t 15 /c \"Automatic shutdown through OmniProgram.\"", HybridShutdown));
                            PSI.CreateNoWindow = true;
                            PSI.UseShellExecute = false;
                            System.Diagnostics.Process.Start(PSI);
                            break;
                        case 3:
                            PSI = new System.Diagnostics.ProcessStartInfo("shutdown", "/r /t 0");
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
            new MIDIImporter(files, false).ShowDialog();

            RebindList();
            GetSelectedMIDIInfo();
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
