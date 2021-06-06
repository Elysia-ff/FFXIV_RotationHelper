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
using System.Linq;
using System.Drawing;

namespace FFXIV_RotationHelper
{
    public partial class FFXIV_RotationHelper : UserControl, IActPluginV1
    {
        private Label lblStatus;
        private readonly RotationWindow rotationWindow;
        private readonly SaveURLForm saveURLForm;
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;

        public FFXIV_RotationHelper()
        {
            rotationWindow = new RotationWindow();
            rotationWindow.OnRotationEnded += () => { StartBtn_Click(null, EventArgs.Empty); };
            saveURLForm = new SaveURLForm(this);

            InitializeComponent();
            isClickthroughCheckBox.Checked = Properties.Settings.Default.Clickthrough;
            sizeComboBox.SelectedItem = Properties.Settings.Default.Size.ToString();

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


            LoadData();

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

        #region GridManagement
        public void LoadData()
        {
            gvSelectedUrls.RowsRemoved -= GvSelectedUrls_RowsRemoved;
            gvSelectedUrls.Rows.Clear();
            gvSelectedUrls.RowsRemoved += GvSelectedUrls_RowsRemoved;

            string[] data = Properties.Settings.Default.LastURLs.Split(Utility.keySeparator);
            for (int i = 0; i < data.Length; ++i)
            {
                string[] value = data[i].Split(Utility.valueSeparator);
                var valueLength = value.Length;
                if (valueLength >= 2)
                {
                    string url = value[0];
                    string memo = value[1];
                    // Default Loop to false if not present or use saved value if found
                    bool loop = false;
                    if (valueLength >= 3) {
                        bool.TryParse(value[2], out loop); 
                    }

                    gvSelectedUrls.Rows.Add(url, memo, loop);
                }
            }
        }

        public List<Rotation> InitRotation()
        {

            var rotations = new List<Rotation>();
            string[] data = Properties.Settings.Default.LastURLs.Split(Utility.keySeparator);
            for (int i = 0; i < data.Length; ++i)
            {
                string[] value = data[i].Split(Utility.valueSeparator);
                var valueLength = value.Length;
                if (valueLength >= 2)
                {
                    string url = value[0];
                    // Default Loop to false if not present or use saved value if found
                    bool loop = false;
                    if (valueLength >= 3)
                    {
                        bool.TryParse(value[2], out loop);
                    }

                    rotations.Add(new Rotation { Url = url, Loop = loop });
                }
            }

            return rotations;
        }

        private void GvSelectedUrls_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            Save();
        }

        private void GvSelectedUrls_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Save();
        }

        private void GvSelectedUrls_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                LoadData();
            }
        }

        private void Save()
        {
            StringBuilder stringBuilder = new StringBuilder();
            DataGridViewRowCollection row = gvSelectedUrls.Rows;
            for (int i = 0; i < row.Count; ++i)
            {
                string url = Utility.ObjectToString(row[i].Cells[0].Value);
                if (url.Length <= 0)
                {
                    continue;
                }

                string memo = Utility.ObjectToString(row[i].Cells[1].Value);
                string loop = Utility.ObjectToString(row[i].Cells[2].Value);

                stringBuilder.Append(url);
                stringBuilder.Append(Utility.valueSeparator);
                stringBuilder.Append(memo);
                stringBuilder.Append(Utility.valueSeparator);
                stringBuilder.Append(loop);
                
                stringBuilder.Append(Utility.keySeparator);
            }

            if (stringBuilder.Length > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }

            Properties.Settings.Default.LastURLs = stringBuilder.ToString();
            Properties.Settings.Default.Save();
        }

        private void Save(StringBuilder stringBuilder)
        {
            Properties.Settings.Default.LastURLs = stringBuilder.ToString();
            Properties.Settings.Default.Save();
        }

        private void GvSelectedUrls_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                    !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {

                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = gvSelectedUrls.DoDragDrop(
                    gvSelectedUrls.Rows[rowIndexFromMouseDown],
                    DragDropEffects.Move);
                }
            }
        }

        private void GvSelectedUrls_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            rowIndexFromMouseDown = gvSelectedUrls.HitTest(e.X, e.Y).RowIndex;
            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                               e.Y - (dragSize.Height / 2)),
                                    dragSize);
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void GvSelectedUrls_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void GvSelectedUrls_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = gvSelectedUrls.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            rowIndexOfItemUnderMouseToDrop =
                gvSelectedUrls.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {

                DataGridViewRow rowToMove = e.Data.GetData(
                    typeof(DataGridViewRow)) as DataGridViewRow;

                if (rowIndexOfItemUnderMouseToDrop < 0 || rowIndexOfItemUnderMouseToDrop==gvSelectedUrls.Rows.Count-1)
                {
                    return;
                }
                gvSelectedUrls.Rows.RemoveAt(rowIndexFromMouseDown);
                gvSelectedUrls.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);
                loadBtn.Enabled = true; // Order has changed, so re-enable loadBtn

            }
        }

        private void GvSelectedUrls_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var columnName = gvSelectedUrls.Columns[e.ColumnIndex].Name;
            if (columnName == "URL" || columnName == "Loop")
            {
                loadBtn.Enabled = true; // Leaving this kind of dumb for now, if URL changes or checkbox changes, re-enable loadBtn
            }
        }

        private void GvSelectedUrls_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (gvSelectedUrls.IsCurrentCellDirty)
            {
                gvSelectedUrls.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
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

            var rotations = InitRotation();
            if (rotations == null || rotations.Count <= 0)
            {
                return;
            }

            loadBtn.Enabled = false;
            startBtn.Enabled = false;

            foreach (var rotation in rotations)
            {
                rotation.Data = await GetRotationAsync(rotation.Url);
            }
            
            rotationWindow.LoadData(rotations);

            if (rotations.Exists(x=>x.Data==null||x.Data.Sequence==null||x.Data.Sequence.Count<=0))
            {
                MessageBox.Show(this, "Couldn't load the rotation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var firstClass = rotations[0].Data.Class;
            if (!rotations.All<Rotation>(x=>x.Data.Class== firstClass))
            {
                MessageBox.Show(this, "Couldn't load the rotation.  Class in rotation must be identical.");
                return;
            }

            startBtn.Enabled = true;
            UpdateStatusLabel();
        }

        public void SetURLs(StringBuilder urls)
        {

            Save(urls);
            LoadData();
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

                // Since grid initiates Save, shouldn't need this here anymore.
                // TODO: Remove if no longer needed
                // Properties.Settings.Default.lastURL = data.URL; // Fix Save
                // Properties.Settings.Default.Save();

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