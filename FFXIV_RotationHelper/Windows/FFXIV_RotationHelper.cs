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
    public partial class FFXIV_RotationHelper : UserControl, IActPluginV1
    {
        private Label lblStatus;
        private RotationWindow rotationWindow;
        private SaveURLForm saveURLForm;

        public FFXIV_RotationHelper()
        {
            rotationWindow = new RotationWindow();
            saveURLForm = new SaveURLForm(this);

            InitializeComponent();
            isClickthroughCheckBox.Checked = Properties.Settings.Default.Clickthrough;
            restartCheckBox.Checked = Properties.Settings.Default.RestartOnEnd;
            sizeComboBox.SelectedItem = Properties.Settings.Default.Size.ToString();
        }

        #region IActPluginV1 Method
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            pluginScreenSpace.Controls.Add(this);
            Dock = DockStyle.Fill;
            lblStatus = pluginStatusText;
            lblStatus.Text = "Plugin Started";

            urlTextBox.Text = Properties.Settings.Default.lastURL.ToString();

#if USE_ON_LOG_LINE
            ActGlobals.oFormActMain.OnLogLineRead -= OFormActMain_OnLogLineRead;
            ActGlobals.oFormActMain.OnLogLineRead += OFormActMain_OnLogLineRead;
#endif
            ActGlobals.oFormActMain.BeforeLogLineRead -= OFormActMain_BeforeLogLineRead;
            ActGlobals.oFormActMain.BeforeLogLineRead += OFormActMain_BeforeLogLineRead;

            SetStatusLabel();

            System.Timers.Timer timer = new System.Timers.Timer(6000);
            timer.Elapsed += delegate
            {
                timer.Close();
                Initailize();
            };
            timer.Start();
        }

        private void Initailize()
        {
            if (!string.IsNullOrEmpty(PlayerData.Instance.Name))
                return;

            Invoke(() =>
            {
                IActPluginV1 o = null;
                foreach (var x in ActGlobals.oFormActMain.ActPlugins)
                {
                    if (x.pluginFile.Name.ToUpper() == "FFXIV_ACT_Plugin.dll".ToUpper() && x.cbEnabled.Checked)
                    {
                        o = x.pluginObj;
                        if (o != null)
                        {
                            try
                            {
                                var pluginType = o.GetType();

                                o.DeInitPlugin();
                                x.pluginObj = o = null;
                                System.Threading.Thread.Sleep(500);
                                x.tpPluginSpace.Controls.Clear();
                                System.Threading.Thread.Sleep(500);

                                IActPluginV1 main = (IActPluginV1)Activator.CreateInstance(pluginType);

                                main.InitPlugin(x.tpPluginSpace, x.lblPluginStatus);
                                x.pluginObj = o = main;
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
            });
        }

        public static void Invoke(Action action)
        {
            if (ActGlobals.oFormActMain != null &&
                ActGlobals.oFormActMain.IsHandleCreated &&
                !ActGlobals.oFormActMain.IsDisposed)
            {
                if (ActGlobals.oFormActMain.InvokeRequired)
                {
                    ActGlobals.oFormActMain.Invoke((MethodInvoker)delegate
                    {
                        action();
                    });
                }
                else
                {
                    action();
                }
            }
        }

        public void DeInitPlugin()
        {
            lblStatus.Text = "No Status";

#if USE_ON_LOG_LINE
            ActGlobals.oFormActMain.OnLogLineRead -= OFormActMain_OnLogLineRead;
#endif
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
                await DB.LoadAsync();

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

#if USE_ON_LOG_LINE
        private void OFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            if (logInfo.logLine.Length <= 18)
                return;

            string logType = logInfo.logLine.Substring(15, 3);
            if (logType.Equals("15:") || logType.Equals("16:"))
            {
                string[] logLine = logInfo.logLine.Split(':');

                LogData log = new LogData(logLine, false);
                if (log.IsValid)
                {
                    if (rotationWindow.Visible)
                    {
                        rotationWindow.OnActionCasted(log);
                    }
                }
            }
        }
#endif

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

                case LogDefine.Type.RemoveCombatant:
                    if (PlayerData.Instance.RemovePet(logLine))
                    {
                        petText.Text = "Not Found";
                    }
                    break;
#if !USE_ON_LOG_LINE
                case LogDefine.Type.Ability:
                case LogDefine.Type.AOEAbility:
                    LogData log = new LogData(logLine, true);
                    if (log.IsValid)
                    {
                        if (rotationWindow.Visible)
                        {
                            rotationWindow.OnActionCasted(log);
                        }
                    }
                    break;
#endif
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

        private void SizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string offsetStr = sizeComboBox.SelectedItem.ToString();
            Properties.Settings.Default.Size = offsetStr;
            Properties.Settings.Default.Save();

            if (rotationWindow.Visible)
            {
                rotationWindow.SetSize(offsetStr);
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

        private void LogInsertBtn_Click(object sender, EventArgs e)
        {
            LogLineEventArgs args = new LogLineEventArgs(logLineBox.Text, 0, DateTime.Now, string.Empty, true);
#if USE_ON_LOG_LINE
            OFormActMain_OnLogLineRead(false, args);
#endif
            OFormActMain_BeforeLogLineRead(false, args);
        }
#endif
    }
}