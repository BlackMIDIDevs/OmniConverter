using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Midi;

namespace OmniConverter
{
    public partial class SoundFontsList : Form
    {
        public SoundFontsList()
        {
            InitializeComponent();

            RebindList();
        }

        private void RebindList()
        {
            Debug.PrintToConsole("ok", "Rebinding Program.SFArray.List to SFList...");

            SFList.DataSource = null;
            SFList.DataSource = Program.SFArray.List;
            SFList.DisplayMember = "GetSoundFontPath";

            Debug.PrintToConsole("ok", "SFList bound successfully.");
        }

        private void SoundFontsList_Load(object sender, EventArgs e)
        {

        }

        private void RLCSFL_Click(object sender, EventArgs e)
        {
            CommonSoundFonts.LoadCSF();
            RebindList();
        }

        private void SFList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 NotSFZ = -1;
            SoundFont SF = (SoundFont)SFList.SelectedItem;
            Boolean Valid = (SF != null);

            NotSFZ = Valid ? ((Path.GetExtension(SF.GetSoundFontPath).ToLowerInvariant() != ".sfz") ? -1 : 0) : 0;
            SP.Minimum = NotSFZ;
            SB.Minimum = NotSFZ;
            DP.Minimum = NotSFZ;

            SP.Value = Valid ? SF.GetSourcePreset : 0;
            SB.Value = Valid ? SF.GetSourceBank : 0;
            DP.Value = Valid ? SF.GetDestinationPreset : 0;
            DB.Value = Valid ? SF.GetDestinationBank : 0;
            DBLSB.Value = Valid ? SF.GetDestinationBankLSB : 0;
            Enabled.Checked = Valid ? SF.IsEnabled : false;
            XGM.Checked = Valid ? SF.GetXGMode : false;

            SFSettings.Enabled = Valid;
            MvU.Enabled = Valid;
            MvD.Enabled = Valid;
            RmvSF.Enabled = Valid;
        }

        private void ApplySettings_Click(object sender, EventArgs e)
        {
            SoundFont SF = (SoundFont)SFList.SelectedItem;
            if (SF != null)
                SF.SetNewValues((Int32)SP.Value, (Int32)SB.Value, (Int32)DP.Value, (Int32)DB.Value, (Int32)DBLSB.Value, Enabled.Checked, XGM.Checked);
        }

        private void AddSF_Click(object sender, EventArgs e)
        {
            List<String> SFErrs = new List<String>();
            OpenFileDialog SFImport = new OpenFileDialog();
            SFImport.InitialDirectory = (Properties.Settings.Default.LastPathSFImport.ToLowerInvariant() == "null" ?
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) : Properties.Settings.Default.LastPathMIDIImport);
            SFImport.Filter = String.Format("{0}|*.sf1;*.sf2;*.sfz;*.sf2pack;", "SoundFonts");
            SFImport.Multiselect = true;

            Debug.PrintToConsole("ok", "Spawned SFImport window.");
            if (SFImport.ShowDialog() == DialogResult.OK)
            {
                Debug.PrintToConsole("ok", "Returned OK, analyzing SoundFont(s)...");

                Properties.Settings.Default.LastPathSFImport = Path.GetDirectoryName(SFImport.FileNames[0]);
                Properties.Settings.Default.Save();

                foreach (String SF in SFImport.FileNames)
                {
                    Debug.PrintToConsole("ok", String.Format("Current SoundFont = {0}", SF));

                    // Check if valid
                    int SFH = BassMidi.BASS_MIDI_FontInit(SF, BASSFlag.BASS_DEFAULT);
                    BASSError Err = Bass.BASS_ErrorGetCode();

                    if (Err == 0)
                    {
                        Debug.PrintToConsole("ok", "The SoundFont is valid.");

                        Int32 NotSFZ = (Path.GetExtension(SF) != ".sfz") ? -1 : 0;
                        int[] TV = new int[] { NotSFZ, NotSFZ, 0, NotSFZ, 0, 0 };
                        BassMidi.BASS_MIDI_FontFree(SFH);

                        // Split filename in case of automatic preset/bank assign values
                        Match match = Regex.Match(Path.GetFileNameWithoutExtension(SF), @"\d{3}\.\d{3}\.\d{3}\.\d{3}\.\d{1}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            Debug.PrintToConsole("ok", "The SoundFont's name has valid bank and preset values. Parsing...");

                            String[] Values = Path.GetFileNameWithoutExtension(SF).Split('-')[0].Split('.');
                            MessageBox.Show(Values.ToString());
                            for (int i = 0; i < Values.Length && i < TV.Length; i++)
                            {
                                Int32 T = -1;
                                if (Int32.TryParse(Values[i], out T))
                                    TV[i] = T;
                            }

                            Debug.PrintToConsole("ok", "Values parsed.");
                        }

                        Debug.PrintToConsole("ok", String.Format("SFP = {0}", SF));
                        Debug.PrintToConsole("ok", String.Format("SP = {0}", TV[1]));
                        Debug.PrintToConsole("ok", String.Format("SB = {0}", TV[0]));
                        Debug.PrintToConsole("ok", String.Format("DP = {0}", TV[2]));
                        Debug.PrintToConsole("ok", String.Format("DB = {0}", TV[3]));
                        Debug.PrintToConsole("ok", String.Format("DBLSB = {0}", TV[4]));
                        Debug.PrintToConsole("ok", String.Format("XG = {0}", Convert.ToBoolean(TV[5])));
                        Debug.PrintToConsole("ok", String.Format("Enabled = {0}", true));

                        Program.SFArray.List.Add(new SoundFont(SF, TV[1], TV[0], TV[3], TV[2], TV[4], true, Convert.ToBoolean(TV[5])));
                    }
                    else
                    {
                        Debug.PrintToConsole("err", String.Format("SoundFont invalid: {0}", SF));
                        SFErrs.Add(SF);
                    }
                }

                if (SFErrs.Count > 0)
                {
                    Debug.ShowMsgBox("Invalid SoundFont(s) detected", String.Format(
                            "The following SoundFont(s) were not valid, and have not been added to the list.\n\n{0}\n\nPress OK to continue",
                            string.Join(Environment.NewLine, SFErrs.ToArray())), null, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                RebindList();
            }
        }

        private void RmvSF_Click(object sender, EventArgs e)
        {
            foreach (SoundFont SF in SFList.SelectedItems)
                Program.SFArray.List.Remove(SF);

            RebindList();
        }

        private void MoveSoundFonts(MoveDirection D)
        {
            List<Int32> SelectedItemsBefore = new List<Int32>();

            foreach (SoundFont SF in SFList.SelectedItems)
            {
                Int32 OldIndex = Program.SFArray.List.IndexOf(SF);
                Int32 NewIndex = Program.SFArray.List.Move(OldIndex, D);
                if (NewIndex != -1) SelectedItemsBefore.Add(NewIndex);

                Debug.PrintToConsole("ok", String.Format("Moved SoundFont {0}. Direction = {1}", SF.GetSoundFontPath, D));
            }

            SFList.ClearSelected();
            foreach (Int32 SF in SelectedItemsBefore)
                SFList.SetSelected(SF, true);

            RebindList();
        }

        private void MvU_Click(object sender, EventArgs e)
        {
            MoveSoundFonts(MoveDirection.Up);
        }

        private void MvD_Click(object sender, EventArgs e)
        {
            MoveSoundFonts(MoveDirection.Down);
        }

        private void SFList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                RmvSF_Click(null, null);
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SFList_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            SoundFont SF = (SoundFont)SFList.Items[e.Index];
            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
            g.DrawString(SF.GetSoundFontPath, e.Font, new SolidBrush(SF.IsEnabled ? e.ForeColor : SystemColors.GrayText), new PointF(e.Bounds.X, e.Bounds.Y));

            e.DrawFocusRectangle();
        }
    }
}
