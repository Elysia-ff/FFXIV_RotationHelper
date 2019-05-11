using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFXIV_RotationHelper
{
    public partial class SaveURLForm : Form
    {
        private FFXIV_RotationHelper helper;

        private readonly char keySeparator = '\t';
        private readonly char valueSeparator = '\n';

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

            string[] data = Properties.Settings.Default.SavedURLs.Split(keySeparator);
            for (int i = 0; i < data.Length; ++i)
            {
                string[] value = data[i].Split(valueSeparator);

                if (value.Length >= 2)
                {
                    string url = value[0];
                    string memo = value[1];

                    dataGridView.Rows.Add(url, memo);
                }
            }
        }

        private string ObjectToString(object o)
        {
            return o == null ? string.Empty : o.ToString();
        }

        private void Save()
        {
            StringBuilder stringBuilder = new StringBuilder();
            DataGridViewRowCollection row = dataGridView.Rows;
            for (int i = 0; i < row.Count; ++i)
            {
                string url = ObjectToString(row[i].Cells[0].Value);
                if (url.Length <= 0)
                    continue;

                string memo = ObjectToString(row[i].Cells[1].Value);

                stringBuilder.Append(url);
                stringBuilder.Append(valueSeparator);
                stringBuilder.Append(memo);

                stringBuilder.Append(keySeparator);
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
            if (row.Count > 0)
            {
                string url = ObjectToString(row[0].Cells[0].Value);
                helper.SetURL(url);

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
