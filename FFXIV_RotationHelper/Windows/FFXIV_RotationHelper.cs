using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FFXIV_RotationHelper
{
    public partial class FFXIV_RotationHelper : UserControl, IActPluginV1
    {
        private Label lblStatus;
        private readonly RotationWindow rotationWindow;
        private readonly SaveURLForm saveURLForm;

        public FFXIV_RotationHelper()
        {
            rotationWindow = new RotationWindow();
            rotationWindow.OnRotationEnded += () => { StartBtn_Click(null, EventArgs.Empty); };
            saveURLForm = new SaveURLForm(this);

            InitializeComponent();
            isClickthroughCheckBox.Checked = Properties.Settings.Default.Clickthrough;
            restartCheckBox.Checked = Properties.Settings.Default.RestartOnEnd;
            resizableCheckBox.Checked = Properties.Settings.Default.Resizable;

            Command.Bind("rotationtoggle", new Method(() =>
            {
                if (startBtn.Enabled)
                {
                    Invoke(new Action(() =>
                    {
                        StartBtn_Click(null, EventArgs.Empty);
                    }));
                }
            }));
            Command.Bind("rotationreset", new Method(() =>
            {
                if (rotationWindow.IsPlaying)
                {
                    rotationWindow.Reset();
                }
            }));
        }

        #region IActPluginV1 Method
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            pluginScreenSpace.Controls.Add(this);
            Dock = DockStyle.Fill;
            lblStatus = pluginStatusText;
            lblStatus.Text = "Plugin Started";

            urlTextBox.Text = Properties.Settings.Default.lastURL.ToString();

            ActGlobals.oFormActMain.BeforeLogLineRead -= OFormActMain_BeforeLogLineRead;
            ActGlobals.oFormActMain.BeforeLogLineRead += OFormActMain_BeforeLogLineRead;

            UpdateStatusLabel();
        }

        public void DeInitPlugin()
        {
            lblStatus.Text = "No Status";

            ActGlobals.oFormActMain.BeforeLogLineRead -= OFormActMain_BeforeLogLineRead;
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
            {
                return;
            }

            loadBtn.Enabled = false;
            startBtn.Enabled = false;

            RotationData data = await GetRotationAsync(url);
            if (data == null || data.Class == null || data.Sequence == null || data.Sequence.Count <= 0)
            {
                MessageBox.Show(this, "Couldn't load the rotation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            rotationWindow.LoadData(data);
            startBtn.Enabled = true;
            UpdateStatusLabel();
        }

        public void SetURL(string text)
        {
            urlTextBox.Text = text;
        }

        private void URLTextBox_TextChanged(object sender, EventArgs e)
        {
            loadBtn.Enabled = !rotationWindow.IsLoadedURL.Equals(urlTextBox.Text);
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            saveURLForm.Location = Location;
            saveURLForm.ShowDialog();
        }

        private async Task<RotationData> GetRotationAsync(string url)
        {
            if (!DB.IsLoaded)
            {
                await DB.LoadAsync();
            }

            string convertedURL = URLConverter.Convert(url);
            HttpWebRequest request = WebRequest.Create(convertedURL) as HttpWebRequest;
            using (HttpWebResponse response = await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null) as HttpWebResponse)
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                string content = await streamReader.ReadToEndAsync();
                RotationData data = JsonConvert.DeserializeObject<RotationData>(content);
                data.Initialize(url);

                Properties.Settings.Default.lastURL = data.URL;
                Properties.Settings.Default.Save();

                return data;
            }
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
            string[] logLine = logInfo.originalLogLine.Split('|');
            if (!int.TryParse(logLine[0], out int logCode))
            {
                return;
            }

            LogDefine.Type logType = (LogDefine.Type)logCode;
            switch (logType)
            {
                case LogDefine.Type.Chat:
                    // 0    1                                   2     3  4
                    // 00 | 2021-05-14T19:09:04.0000000+09:00 | 0038 | | end | 643004e1d4abc57ce3a15931e1139f8e
                    if (logLine.Length >= 5)
                    {
                        if (logLine[2].Equals(LogDefine.ECHO_CHAT_CODE))
                        {
                            Command.Execute(logLine[4].ToLower());
                        }
                    }

                    break;

                case LogDefine.Type.ChangePrimaryPlayer:
                    if (PlayerData.SetPlayer(logLine))
                    {
                        nameText.Text = PlayerData.Name;
                        UpdateStatusLabel();
                    }
                    break;

                case LogDefine.Type.AddCombatant:
                    if (PlayerData.SetPet(logLine))
                    {
                        petText.Text = PlayerData.PetName;
                    }
                    break;

                case LogDefine.Type.RemoveCombatant:
                    if (PlayerData.RemovePet(logLine))
                    {
                        petText.Text = "Not Found";
                    }
                    break;

                case LogDefine.Type.Ability:
                case LogDefine.Type.AOEAbility:
                    LogData log = new LogData(logLine);
                    if (log.IsValid && rotationWindow.Visible)
                    {
                        rotationWindow.OnActionCasted(log);
                    }
                    break;
            }
        }

        private void ThanksToLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://ffxivrotations.com");
        }

        private void UpdateStatusLabel()
        {
            if (!rotationWindow.IsLoaded)
            {
                statusLabel.Text = "Not initialized - Please load your rotation.";
            }
            else if (string.IsNullOrEmpty(PlayerData.Name))
            {
                statusLabel.Text = "Not found player info. - Please restart the ACT.";
            }
            else
            {
                statusLabel.Text = "Initialized - Ready to start.";
            }
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

        private void ResizableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Resizable = resizableCheckBox.Checked;
            Properties.Settings.Default.Save();
            rotationWindow.Refresh();
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

                idxStr.Append((int)list[i].DBIdx);
                idxStr.Append(",");
            }

            if (stringBuilder.Length > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }

            if (idxStr.Length > 0)
            {
                idxStr.Remove(idxStr.Length - 1, 1);
                Clipboard.SetText(idxStr.ToString());
            }

            debugLabel.Text = stringBuilder.ToString();
        }

        private void LogInsertBtn_Click(object sender, EventArgs e)
        {
            LogLineEventArgs args = new LogLineEventArgs(logLineBox.Text, 0, DateTime.Now, string.Empty, true);
            OFormActMain_BeforeLogLineRead(false, args);
        }		
#endif
	}
}