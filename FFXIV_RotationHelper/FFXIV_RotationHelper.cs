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
        private AssemblyResolver assemblyResolver;

        private Label lblStatus;
        private RotationWindow rotationWindow;

        public FFXIV_RotationHelper()
        {
            assemblyResolver = new AssemblyResolver(this);

            InitializeComponent();
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

            rotationWindow = new RotationWindow();
#if DEBUG
            urlTextBox.Text = "http://ffxivrotations.com/1w7v";
#endif

            ActGlobals.oFormActMain.BeforeLogLineRead -= OFormActMain_BeforeLogLineRead;
            ActGlobals.oFormActMain.BeforeLogLineRead += OFormActMain_BeforeLogLineRead;
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
            // TODO : RotationWindow 가 켜진 상태에서 로딩 시도 시 에러 팝업 
            if (rotationWindow.Visible)
            {
                return;
            }

            string url = urlTextBox.Text;
            if (url == null || url.Length <= 0)
                return;

            loadBtn.Enabled = false;
            startBtn.Enabled = false;

            // TODO : 데이터 다운로드 실패 시 에러 팝업
            //try
            RotationData data = await GetRotationAsync(url);
            rotationWindow.LoadData(data);

            startBtn.Enabled = true;
            //catch
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
                    PlayerData.Instance.SetPlayer(logLine);
                    break;
                case LogDefine.Type.AddCombatant:
                    PlayerData.Instance.SetPet(logLine);
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
#endif
    }
}