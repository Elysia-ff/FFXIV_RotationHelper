using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace FFXIV_RotationHelper
{
    public partial class FFXIV_RotationHelper : UserControl, IActPluginV1, IWin32Window
    {
        private AssemblyResolver assemblyResolver;

        private Label lblStatus;
        private RotationWindow rotationWindow;

        public FFXIV_RotationHelper()
        {
            assemblyResolver = new AssemblyResolver(this);
            rotationWindow = new RotationWindow();

            InitializeComponent();
            isClickthroughCheckBox.Checked = Properties.Settings.Default.Clickthrough;
            restartCheckBox.Checked = Properties.Settings.Default.RestartOnEnd;
        }

        #region IActPluginV1 Method
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            Assembly.Load("CsvHelper.dll");
            Assembly.Load("Newtonsoft.Json.dll");

            pluginScreenSpace.Controls.Add(this);
            Dock = DockStyle.Fill;
            lblStatus = pluginStatusText;
            lblStatus.Text = "Plugin Started";
            
#if DEBUG
            urlTextBox.Text = "http://ffxivrotations.com/1w7v";
#endif

            ActGlobals.oFormActMain.BeforeLogLineRead -= OFormActMain_BeforeLogLineRead;
            ActGlobals.oFormActMain.BeforeLogLineRead += OFormActMain_BeforeLogLineRead;

            SetStatusLabel();
        }

        public void DeInitPlugin()
        {
            lblStatus.Text = "No Status";

            ActGlobals.oFormActMain.BeforeLogLineRead -= OFormActMain_BeforeLogLineRead;
            PlayerData.Free();
        }
        #endregion

        private async void LoadBtn_Click(object sender, EventArgs e)
        {
            if (rotationWindow.Visible)
            {
                DialogResult result = MessageBox.Show(this, "You have to stop the rotation.\nProceed?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if (result == DialogResult.OK)
                {
                    startBtn.PerformClick();
                }
                else
                {
                    return;
                }
            }

            string url = urlTextBox.Text;
            if (url == null || url.Length <= 0)
                return;

            loadBtn.Enabled = false;
            startBtn.Enabled = false;

            RotationData data = await GetRotationAsync(url);
            rotationWindow.LoadData(data);
            if (data == null || data.Sequence == null || data.Sequence.Count <= 0)
            {
                MessageBox.Show(this, "Couldn't load the rotation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            startBtn.Enabled = true;
            SetStatusLabel();
        }

        private void URLTextBox_TextChanged(object sender, EventArgs e)
        {
            loadBtn.Enabled = !rotationWindow.IsLoadedURL.Equals(urlTextBox.Text);
        }

        private async Task<RotationData> GetRotationAsync(string url)
        {
            if (!DB.IsLoaded)
                await DB.LoadAsync();

            string convertedURL = URLConverter.Convert(url);
            HttpWebRequest request = WebRequest.Create(convertedURL) as HttpWebRequest;
            HttpWebResponse response = await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null) as HttpWebResponse;

            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            string content = await streamReader.ReadToEndAsync();
            RotationData data = JsonConvert.DeserializeObject<RotationData>(content, new SequenceConverter());
            data.URL = url;

            return data;
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            if (!rotationWindow.Visible)
            {
                if (rotationWindow.IsLoaded)
                {
                    rotationWindow.Show();
                    startBtn.Text = "Stop";
                }
            }
            else
            {
                rotationWindow.Hide();
                startBtn.Text = "Start";
            }
        }

        private void OFormActMain_BeforeLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            string[] logLine = logInfo.logLine.Split('|');
            if (!int.TryParse(logLine[0], out int logCode))
                return;

            LogDefine.Type logType = (LogDefine.Type)logCode;
            switch (logType)
            {
                case LogDefine.Type.ChangePrimaryPlayer:
                    if (PlayerData.Instance.SetPlayer(logLine))
                    {
                        nameText.Text = PlayerData.Instance.Name;
                        SetStatusLabel();
                    }
                    break;

                case LogDefine.Type.AddCombatant:
                    if (PlayerData.Instance.SetPet(logLine))
                    {
                        petText.Text = PlayerData.Instance.PetName;
                    }
                    break;

                case LogDefine.Type.Ability:
                case LogDefine.Type.AOEAbility:
                    LogData log = new LogData(logLine);
                    if (log.IsValid)
                    {
                        if (rotationWindow.Visible)
                        {
                            rotationWindow.OnActionCasted(log);
                        }
                    }
                    break;
            }
        }

        private void ThanksToLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://ffxivrotations.com");
        }

        private void SetStatusLabel()
        {
            if (!rotationWindow.IsLoaded)
            {
                statusLabel.Text = "Not initialized - Please load your rotation.";
            }
            else if (string.IsNullOrEmpty(PlayerData.Instance.Name))
            {
                statusLabel.Text = "Not found player info. - Please restart the ACT.";
            }
            else
            {
                statusLabel.Text = "Initialized - Ready to start.";
            }
        }

#if DEBUG
        private void DebugTextBox_TextChanged(object sender, EventArgs e)
        {
            List<SkillData> list = DB.Find(debugTextBox.Text);
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder idxStr = new StringBuilder();
            for (int i = 0; i < list.Count; ++i)
            {
                stringBuilder.Append(list[i]);
                stringBuilder.Append("\n");

                idxStr.Append(list[i].Idx);
                idxStr.Append(",");
            }

            if (stringBuilder.Length > 0)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            if (idxStr.Length > 0)
            {
                idxStr.Remove(idxStr.Length - 1, 1);
                Clipboard.SetText(idxStr.ToString());
            }

            debugLabel.Text = stringBuilder.ToString();
        }

        private void IsClickthroughCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Clickthrough = isClickthroughCheckBox.Checked;
            Properties.Settings.Default.Save();

            if (rotationWindow.Visible)
            {
                rotationWindow.SetClickThrough(Properties.Settings.Default.Clickthrough);
            }
        }

        private void RestartCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RestartOnEnd = restartCheckBox.Checked;
            Properties.Settings.Default.Save();
        }
#endif
    }
}