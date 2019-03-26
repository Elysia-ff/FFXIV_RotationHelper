using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFXIV_RotationHelper
{
    public partial class RotationWindow : Form
    {
        protected override Size DefaultSize { get { return new Size(800, 40); } }

        private RotationData loadedData;
        private List<SkillData> skillList;
        private List<PictureBox> pictureList;
        private int currentIdx = 0;

        public bool IsLoaded { get { return loadedData != null; } }
        public string IsLoadedURL { get { if (!IsLoaded) return string.Empty; return loadedData.URL; } }
        public bool IsPlaying { get; private set; }

        private const int interval = 20;

        // Hide from Alt+Tab
        // https://social.msdn.microsoft.com/Forums/windows/en-US/0eefb6f4-3619-4f7a-b144-48df80e2c603/how-to-hide-form-from-alttab-dialog?forum=winforms
        protected override CreateParams CreateParams
        {
            get
            {
                // Turn on WS_EX_TOOLWINDOW style bit
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        public RotationWindow()
        {
            skillList = new List<SkillData>();
            pictureList = new List<PictureBox>();

            InitializeComponent();

            MouseDown += RotationWindow_MouseDown;
            VisibleChanged += RotationWindow_VisibleChanged;
            LocationChanged += RotationWindow_LocationChanged;
        }

        private void RotationWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.MoveFormWithMouse();
            }
        }

        private void RotationWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                Location = Properties.Settings.Default.Location;
                currentIdx = 0;
                MakePictureBox();
                SetSize(Properties.Settings.Default.Size);
            }
        }

        private void RotationWindow_LocationChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                Properties.Settings.Default.Location = Location;
                Properties.Settings.Default.Save();
            }
        }

        public void LoadData(RotationData data)
        {
            loadedData = data;
            skillList = DB.Get(data.Sequence);
        }

        private void MakePictureBox()
        {
            if (pictureList.Count < skillList.Count)
            {
                int count = skillList.Count - pictureList.Count;
                for (int i = 0; i < count; ++i)
                {
                    PictureBox picture = new PictureBox
                    {
                        Size = new Size(Height, Height),
                        TabStop = false,
                        BackColor = Color.Black,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        WaitOnLoad = false
                    };
                    picture.MouseDown += RotationWindow_MouseDown;
                    Controls.Add(picture);
                    pictureList.Add(picture);
                }
            }

            for (int i = 0; i < skillList.Count; ++i)
            {
                pictureList[i].LoadAsync(skillList[i].IconURL);
            }

            Reposition();
            this.SetClickThrough(Properties.Settings.Default.Clickthrough);
        }

        private void Reposition()
        {
            for (int i = 0, idx = 0; i < skillList.Count; ++i)
            {
                if (i < currentIdx)
                {
                    pictureList[i].Visible = false;
                }
                else if (i < pictureList.Count)
                {
                    pictureList[i].Visible = true;
                    pictureList[i].Location = new Point((Height + interval) * idx++, 0);
                }
                else
                {
                    pictureList[i].Visible = false;
                }
            }

            for (int i = skillList.Count; i < pictureList.Count; ++i)
            {
                pictureList[i].Visible = false;
            }
        }

        public void OnActionCasted(LogData logData)
        {
            if (currentIdx >= skillList.Count)
                return;

            SkillData skillData = skillList[currentIdx];
            if (skillData.Idx == logData.DBCode)
            {
                ++currentIdx;
                if ((currentIdx >= skillList.Count) && Properties.Settings.Default.RestartOnEnd)
                {
                    currentIdx = 0;
                    MakePictureBox();
                }
                else
                {
                    Reposition();
                }
            }
        }

        public void SetSize(string offsetStr)
        {
            float offset = float.Parse(offsetStr) * 0.01f;
            Size defaultSize = DefaultSize;
            int width = (int)(defaultSize.Width * offset);
            int height = (int)(defaultSize.Height * offset);
            SetClientSizeCore(width, height);

            for (int i = 0; i < pictureList.Count; ++i)
            {
                pictureList[i].Size = new Size(height, height);
            }
            Reposition();
        }
    }
}
