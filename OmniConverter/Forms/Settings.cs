using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows.Forms;

namespace OmniConverter
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            MaxVoices.Value = Properties.Settings.Default.VoiceLimit;
            FrequencyBox.Text = Properties.Settings.Default.Frequency.ToString();

            SincInter.Checked = Properties.Settings.Default.SincInter;
            FXDisable.Checked = Properties.Settings.Default.DisableEffects;
            NoteOff1.Checked = Properties.Settings.Default.NoteOff1;
            EnableLoudMax.Checked = Properties.Settings.Default.LoudMax;

            EnableRCOverride.Checked = Properties.Settings.Default.RVOverrideToggle;
            ReverbV.Value = Properties.Settings.Default.ReverbValue;
            ChorusV.Value = Properties.Settings.Default.ChorusValue;

            MTMode.Checked = Properties.Settings.Default.MultiThreadedMode;
            PerTrackMode.Checked = Properties.Settings.Default.PerTrackExport;
            PerTrackExportEach.Checked = Properties.Settings.Default.PerTrackSeparateFiles;
            PerTrackStorage.Checked = Properties.Settings.Default.PerTrackCreateFolder;

            MTLimit.Checked = Properties.Settings.Default.MultiThreadedLimit;
            MTLimitVal.Maximum = Environment.ProcessorCount;
            MTLimitVal.Value = Properties.Settings.Default.MultiThreadedLimitV.LimitToRange(1, Environment.ProcessorCount);

            AutoOutputFolder.Checked = Properties.Settings.Default.AutoOutputFolder;
            AOFPath.Text =
                (Properties.Settings.Default.AOFPath.ToLowerInvariant() == "null" ?
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) : Properties.Settings.Default.AOFPath);

            DoActionAfterRender.Checked = Properties.Settings.Default.DoActionAfterRender;
            DoActionAfterRenderVal.SelectedIndex = Properties.Settings.Default.DoActionAfterRenderV.LimitToRange(0, DoActionAfterRenderVal.Items.Count - 1);

            MTMode_CheckedChanged(sender, e);
            MTLimit_CheckedChanged(sender, e);
            AutoOutputFolder_CheckedChanged(sender, e);
            DoActionAfterRender_CheckedChanged(sender, e);
        }

        private void MTMode_CheckedChanged(object sender, EventArgs e)
        {
            PerTrackMode.Enabled = MTMode.Checked;
            PerTrackMode_CheckedChanged(sender, e);
        }

        private void PerTrackMode_CheckedChanged(object sender, EventArgs e)
        {
            PerTrackExportEach.Enabled = PerTrackMode.Checked;
            PerTrackExportEach_CheckedChanged(sender, e);
        }

        private void PerTrackExportEach_CheckedChanged(object sender, EventArgs e)
        {
            PerTrackStorage.Enabled = PerTrackExportEach.Checked;
        }

        private void MTLimit_CheckedChanged(object sender, EventArgs e)
        {
            MTLimitLab.Enabled = MTLimit.Checked;
            MTLimitVal.Enabled = MTLimit.Checked;
        }

        private void AutoOutputFolder_CheckedChanged(object sender, EventArgs e)
        {
            AOFPath.Enabled = AutoOutputFolder.Checked;
            AOFBrowse.Enabled = AutoOutputFolder.Checked;
        }

        private void AOFBrowse_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog MIDIExport = new CommonOpenFileDialog();
            MIDIExport.IsFolderPicker = true;

            if (MIDIExport.ShowDialog() == CommonFileDialogResult.Ok)
                AOFPath.Text = MIDIExport.FileName;
        }

        private void DoActionAfterRender_CheckedChanged(object sender, EventArgs e)
        {
            DoActionAfterRenderVal.Enabled = DoActionAfterRender.Checked;
        }

        private void EnableRCOverride_CheckedChanged(object sender, EventArgs e)
        {
            ReverbL.Enabled = EnableRCOverride.Checked;
            ReverbV.Enabled = EnableRCOverride.Checked;
            ChorusL.Enabled = EnableRCOverride.Checked;
            ChorusV.Enabled = EnableRCOverride.Checked;
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.VoiceLimit = (Int32)MaxVoices.Value;
            Properties.Settings.Default.Frequency = Int32.Parse(FrequencyBox.SelectedItem.ToString());

            Properties.Settings.Default.SincInter = SincInter.Checked;
            Properties.Settings.Default.DisableEffects = FXDisable.Checked;
            Properties.Settings.Default.NoteOff1 = NoteOff1.Checked;
            Properties.Settings.Default.LoudMax = EnableLoudMax.Checked;

            Properties.Settings.Default.RVOverrideToggle = EnableRCOverride.Checked;
            Properties.Settings.Default.ReverbValue = (Int32)ReverbV.Value;
            Properties.Settings.Default.ChorusValue = (Int32)ChorusV.Value;

            Properties.Settings.Default.MultiThreadedMode = MTMode.Checked;
            Properties.Settings.Default.PerTrackExport = PerTrackMode.Checked;
            Properties.Settings.Default.PerTrackSeparateFiles = PerTrackExportEach.Checked;
            Properties.Settings.Default.PerTrackCreateFolder = PerTrackStorage.Checked;

            Properties.Settings.Default.MultiThreadedLimit = MTLimit.Checked;
            Properties.Settings.Default.MultiThreadedLimitV = (Int32)MTLimitVal.Value;

            Properties.Settings.Default.AutoOutputFolder = AutoOutputFolder.Checked;
            Properties.Settings.Default.AOFPath = AOFPath.Text;

            Properties.Settings.Default.DoActionAfterRender = DoActionAfterRender.Checked;
            Properties.Settings.Default.DoActionAfterRenderV = DoActionAfterRenderVal.SelectedIndex;

            Properties.Settings.Default.Save();

            Close();
        }
    }
}
