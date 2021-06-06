using System;
using System.Text;
using System.Windows.Forms;

namespace FFXIV_RotationHelper
{
    public partial class SaveURLForm : Form
    {
        private readonly FFXIV_RotationHelper helper;

        public SaveURLForm(FFXIV_RotationHelper _helper)
        {
            helper = _helper;
            InitializeComponent();
        }

        public void LoadData()
        {
            dataGridView.RowsRemoved -= DataGridView_RowsRemoved;
            dataGridView.Rows.Clear();
            dataGridView.RowsRemoved += DataGridView_RowsRemoved;

            string[] data = Properties.Settings.Default.SavedURLs.Split(Utility.keySeparator);
            for (int i = 0; i < data.Length; ++i)
            {
                string[] value = data[i].Split(Utility.valueSeparator);
                if (value.Length >= 2)
                {
                    string url = value[0];
                    string memo = value[1];

                    dataGridView.Rows.Add(url, memo);
                }
            }
        }

        private void Save()
        {
            StringBuilder stringBuilder = new StringBuilder();
            DataGridViewRowCollection row = dataGridView.Rows;
            for (int i = 0; i < row.Count; ++i)
            {
                string url = Utility.ObjectToString(row[i].Cells[0].Value);
                if (url.Length <= 0)
                {
                    continue;
                }

                string memo = Utility.ObjectToString(row[i].Cells[1].Value);

                stringBuilder.Append(url);
                stringBuilder.Append(Utility.valueSeparator);
                stringBuilder.Append(memo);

                stringBuilder.Append(Utility.keySeparator);
            }

            if (stringBuilder.Length > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }

            Properties.Settings.Default.SavedURLs = stringBuilder.ToString();
            Properties.Settings.Default.Save();
        }

        private void SelectBtn_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection row = dataGridView.SelectedRows;
            //Added feature - allow multiple URLs to be selected and passed back to FFXIV_RotationHelper gridview control
            if (row.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < row.Count; ++i)
                {
                    
                    string url = Utility.ObjectToString(row[i].Cells[0].Value);
                    if (url.Length <= 0)
                    {
                        continue;
                    }

                    string memo = Utility.ObjectToString(row[i].Cells[1].Value);

                    stringBuilder.Append(url);
                    stringBuilder.Append(Utility.valueSeparator);
                    stringBuilder.Append(memo);

                    stringBuilder.Append(Utility.keySeparator);
                }

                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                }

                //Pass assembled stringbuilder back to RotationHelper form gridview control
                helper.SetURLs(stringBuilder);

                Close();
            }
        }

        private void DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Save();
        }

        private void DataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            Save();
        }

        private void SaveURLForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                LoadData();
            }
        }
    }
}
