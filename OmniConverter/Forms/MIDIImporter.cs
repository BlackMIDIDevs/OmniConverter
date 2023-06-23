using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace OmniConverter
{
    public partial class MIDIImporter : Form
    {
        private Boolean CheckStop = false;
        private Boolean IgnoreInvalidMIDIs = false;
        private string[] MIDIsToLoad;
        private Thread MIDIAnalyzerT, MIDIPathChecker;

        private ulong ValidFiles = 0;
        private ulong InvalidFiles = 0;
        private ulong TotalFiles = 0;

        private CancellationTokenSource CTS = null;
        public DialogResult Result { get; set; }

        public MIDIImporter(String[] MIDIs, Boolean StartUp)
        {
            InitializeComponent();

            IgnoreInvalidMIDIs = StartUp;
            MIDIsToLoad = MIDIs;
        }

        private void MIDIImporter_Load(object sender, EventArgs e)
        {
            MIDIPathChecker = new Thread(() =>
            {
                foreach (string MIDIPath in MIDIsToLoad)
                {
                    if (CheckStop) break;
                    CheckCount(MIDIPath);
                }

                Debug.PrintToConsole("ok", String.Format("Amount of files to analyze: {0}", TotalFiles));

                if (!CheckStop)
                {
                    MIDIAnalyzerT = new Thread(MIDIAnalyzerF);
                    MIDIAnalyzerT.IsBackground = true;
                    MIDIAnalyzerT.Start();
                }
            });

            MIDIPathChecker.Start();
        }

        private void MIDIImporter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Alt || Control.ModifierKeys == Keys.F4)
            {
                e.Cancel = true;
            }
        }

        private void MIDIAnalyzerF()
        {
            // Get last ID from array
            Int32 Index = 0;
            Int64 CurrentMaxIndex = 0;

            if (Program.MIDIList.Count > 0)
            {
                Index = Enumerable.Range(0, Program.MIDIList.Count).Aggregate((max, i) => ((MIDI)Program.MIDIList[max]).ID > ((MIDI)Program.MIDIList[i]).ID ? max : i);
                CurrentMaxIndex = ((MIDI)Program.MIDIList[Index]).ID;
            }

            Int32 MT = Properties.Settings.Default.MultiThreadedMode ? Properties.Settings.Default.MultiThreadedLimitV : 1;
            CTS = new CancellationTokenSource();
            ParallelOptions PO = new ParallelOptions { MaxDegreeOfParallelism = MT, CancellationToken = CTS.Token };

            try
            {
                Parallel.ForEach(MIDIsToLoad, PO, (str, LS) =>
                {
                    try
                    {
                        CheckDirectory(ref CurrentMaxIndex, str);

                        PO.CancellationToken.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception EX)
                    {
                        Debug.ShowMsgBox(
                                "Error while checking MIDIs",
                                "An error has occured while checking the imported MIDIs.",
                                EX.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }
            catch (OperationCanceledException) { }
            finally { CTS.Dispose(); CTS = null; }


            if (InvalidFiles > 0 && !IgnoreInvalidMIDIs && !CheckStop)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    Check.Enabled = false;
                    PB.State = wyDay.Controls.ProgressBarState.Error;
                    PB.Value = 100;
                    CancelBtn.Text = "Confirm";
                    StatusLab.Text = String.Format("Out of {0} files, {1} were valid and {2} were not.",
                        (ValidFiles + InvalidFiles).ToString("N0", new CultureInfo("is-IS")),
                        ValidFiles.ToString("N0", new CultureInfo("is-IS")),
                        InvalidFiles.ToString("N0", new CultureInfo("is-IS")));
                });
            }
            else this.Invoke((MethodInvoker)delegate { Close(); });
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Int32 SleepCount = 0;
                DialogResult DR1 = 0, DR2 = 0;

                if (MIDIAnalyzerT.IsAlive)
                {
                    Debug.PrintToConsole("wrn", "MIDIAnalyzerT is still alive! Asking if user wants to quit.");

                    this.Invoke((MethodInvoker)delegate
                    {
                        CancelBtn.Enabled = false;
                        DR1 = MessageBox.Show("Are you sure you want to terminate the analysis process?", "The converter is still analyzing data", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    });

                    switch (DR1)
                    {
                        case DialogResult.Yes:
                            CTS.Cancel();
                            CheckStop = true;

                            while (MIDIAnalyzerT.IsAlive)
                            {
                                SleepCount++;
                                Thread.Sleep(10);

                                if (SleepCount >= 500) break;

                                Debug.PrintToConsole("wrn", "MIDIAnalyzer is still alive! Waiting...");
                            }

                            if (SleepCount >= 5000)
                            {
                                Debug.PrintToConsole("err", "CThread is still alive!");

                                this.Invoke((MethodInvoker)delegate
                                {
                                    DR2 = MessageBox.Show("The analysis threads seem to have got stuck, are you sure you want to continue?\n\nThis could cause unexpected behavior.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                });

                                switch (DR2)
                                {
                                    case DialogResult.Yes:
                                        MIDIAnalyzerT.Abort();
                                        break;
                                    default:
                                    case DialogResult.No:
                                        this.Invoke((MethodInvoker)delegate { CancelBtn.Enabled = true; });
                                        return;
                                }
                            }

                            Debug.PrintToConsole("ok", "MIDIAnalyzer is not active anymore.");
                            break;
                        default:
                        case DialogResult.No:
                            return;
                    }
                }

                if (!CheckStop) this.Invoke((MethodInvoker)delegate { Close(); });
            }).Start();
        }

        private void CheckCount(String Target)
        {
            try
            {
                foreach (String A in GetFiles(Target))
                    TotalFiles++;
            }
            catch (Exception EX)
            {
                Debug.ShowMsgBox(
                    "Error while checking MIDIs",
                    "An error has occured while checking the imported MIDIs.",
                    EX.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);

                Close();
            }
        }

        // Check if file is valid
        private String GetInfoMIDI(ref long CMI, string str, out MIDI MIDIStruct)
        {
            // Set MIDIStruct as null first
            String ID = IDGenerator.GetID();
            MIDIStruct = null;

            Debug.PrintToConsole("ok", String.Format("{0} - Analysis of {1}", ID, str));

            try
            {
                TaskStatus MIDIT = new TaskStatus(Path.GetFileName(str));
                MIDIT.Dock = DockStyle.Top;
                LogPanel.Invoke((MethodInvoker)delegate
                {
                    Debug.PrintToConsole("ok", "Added MIDIThreadStatus control for MIDI.");
                    LogPanel.Controls.Add(MIDIT);
                });

                MIDIT.Invoke((MethodInvoker)delegate
                {
                    MIDIT.UpdateTitle($"Loading...");
                    MIDIT.UpdatePBStyle(ProgressBarStyle.Marquee);
                });

                MIDIStruct = MIDI.LoadFromFile(CMI, str, Path.GetFileName(str), (p, t) =>
                {
                    MIDIT.Invoke((MethodInvoker)delegate
                    {
                        MIDIT.UpdateTitle($"{p}/{t}");
                        MIDIT.UpdatePBStyle(ProgressBarStyle.Blocks);
                        MIDIT.UpdatePB((100 * p) / t);
                    });
                });

                LogPanel.Invoke((MethodInvoker)delegate
                {
                    Debug.PrintToConsole("ok", "Removed MIDIThreadStatus control for MIDI.");
                    LogPanel.Controls.Remove(MIDIT);
                });

                Debug.PrintToConsole("ok", String.Format("{0} - Analysis finished for MIDI {1}.", ID, str));
                return "No error.";
            }
            catch (Exception ex)
            {
                return String.Format("A {0} exception has occured while loading the file.", ex.InnerException.ToString());
            }
        }

        private void CheckDirectory(ref Int64 CMI, String Target)
        {
            try
            {
                foreach (String file in GetFiles(Target))
                {
                    if (CheckStop) return;
                    CheckFile(ref CMI, file);
                }
            }
            catch (Exception EX)
            {
                Debug.ShowMsgBox(
                    "Error while checking MIDIs",
                    "An error has occured while checking the imported MIDIs.",
                    EX.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);

                Close();
            }
        }

        private void CheckFile(ref Int64 CMI, String str)
        {
            MIDI MIDIInfo = null;
            String ErrorReason = "";

            if (Path.GetExtension(str).ToLower() == ".mid" ||
                Path.GetExtension(str).ToLower() == ".midi" ||
                Path.GetExtension(str).ToLower() == ".kar" ||
                Path.GetExtension(str).ToLower() == ".rmi" ||
                Path.GetExtension(str).ToLower() == ".riff")
            {
                ErrorReason = GetInfoMIDI(ref CMI, str, out MIDIInfo);

                if (MIDIInfo != null)
                {
                    Program.MIDIList.Add(MIDIInfo);
                    ValidFiles++;
                    return;
                }
            }
            else ErrorReason = "Unrecognized file extension.";

            this.Invoke((MethodInvoker)delegate
            {
                InvalidMIDI MIDIT = new InvalidMIDI(str, ErrorReason, Program.Error);
                MIDIT.Dock = DockStyle.Top;
                LogPanel.Controls.Add(MIDIT);
            });

            InvalidFiles++;
        }

        // Code by Mac Gravell, edited by Keppy
        // https://stackoverflow.com/a/929418
        IEnumerable<String> GetFiles(String Target)
        {
            Queue<string> AnalyzeQueue = new Queue<string>();

            // Add target of queue to the queue
            AnalyzeQueue.Enqueue(Target);

            // Do this while the queue list still contains items
            while (AnalyzeQueue.Count > 0)
            {
                // Dequeue the item that is going to be analyzed
                Target = AnalyzeQueue.Dequeue();

                try
                {
                    // Add each subdir to the queue
                    if (CheckStop) break;
                    foreach (string subDir in Directory.GetDirectories(Target))
                    {
                        if (CheckStop) break;
                        AnalyzeQueue.Enqueue(subDir);
                    }
                }
                catch { }

                string[] Files = null;
                try
                {
                    // Add files from the directory of the queued item
                    Files = Directory.GetFiles(Target);
                }
                catch { }

                // If the function detected items, return them to the calling foreach loop
                if (Files != null)
                {
                    for (int i = 0; i < Files.Length; i++)
                    {
                        if (CheckStop) break;
                        yield return Files[i];
                    }
                }

                // If the queued item is actually a direct path to the file, return it to the foreach loop
                if (System.IO.File.Exists(Target)) yield return Target;
            }
        }

        private void Check_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!MIDIAnalyzerT.IsAlive) return;

                StatusLab.Text = String.Format("Parsed {0} file(s) out of {1}.\nPlease wait...",
                   (ValidFiles + InvalidFiles).ToString("N0", new CultureInfo("is-IS")),
                   TotalFiles.ToString("N0", new CultureInfo("is-IS")));

                PB.Style = ProgressBarStyle.Blocks;
                PB.Value = Convert.ToInt32(Math.Round((ValidFiles + InvalidFiles) * 100.0 / TotalFiles));
            }
            catch { }
        }

        public ulong TotalValidFiles() { return ValidFiles; }
    }
}
