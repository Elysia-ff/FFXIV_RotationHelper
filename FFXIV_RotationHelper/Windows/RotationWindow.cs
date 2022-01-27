using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FFXIV_RotationHelper
{
    public partial class RotationWindow : Form
    {
        private RotationData loadedData;
        private List<SkillData> skillList;
        private readonly List<PictureBox> pictureList;
        private int currentIdx = 0;

        public bool IsLoaded { get { return loadedData != null; } }
        public string IsLoadedURL { get { return IsLoaded ? loadedData.URL : string.Empty; } }
        public bool IsPlaying { get { return Visible && IsLoaded; } }
        private int IconHeight { get { return ClientSize.Height - cGrip; } }

        private const int interval = 20;

        public event Action OnRotationEnded;

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

        // Resize
        // https://stackoverflow.com/a/2575452
        private const int cGrip = 12;      // Grip size

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Properties.Settings.Default.Resizable)
            {
                Rectangle rc = new Rectangle(ClientSize.Width - cGrip, ClientSize.Height - cGrip, cGrip, cGrip);
                ControlPaint.DrawSizeGrip(e.Graphics, Color.Black, rc);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32());
                pos = PointToClient(pos);
                if (Properties.Settings.Default.Resizable && pos.X >= ClientSize.Width - cGrip && pos.Y >= ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT

                    return;
                }
            }

            base.WndProc(ref m);
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
                Size = Properties.Settings.Default.WindowSize;
                currentIdx = 0;
                MakePictureBox();
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
            skillList = DB.Get(data);
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
                        Size = new Size(IconHeight, IconHeight),
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
            if (pictureList.Count < skillList.Count)
            {
                return;
            }

            for (int i = 0, idx = 0; i < skillList.Count; ++i)
            {
                if (i < currentIdx)
                {
                    pictureList[i].Visible = false;
                }
                else if (i < pictureList.Count)
                {
                    pictureList[i].Visible = true;
                    pictureList[i].Location = new Point((IconHeight + interval) * idx++, 0);
                    pictureList[i].Size = new Size(IconHeight, IconHeight);
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
            {
                return;
            }

            SkillData skillData = skillList[currentIdx];
            if (DB.IsSameAction(loadedData.Class, logData.GameIdx, skillData.DBIdx))
            {
                ++currentIdx;
                if (currentIdx >= skillList.Count)
                {
                    if (Properties.Settings.Default.RestartOnEnd)
                    {
                        currentIdx = 0;
                    }
                    else
                    {
                        OnRotationEnded?.Invoke();
                        return;
                    }
                }

                Reposition();
            }
        }

        public void Reset()
        {
            currentIdx = 0;
            Reposition();
        }

        private void RotationWindow_SizeChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                Properties.Settings.Default.WindowSize = Size;
                Properties.Settings.Default.Save();

                Reposition();
                Refresh();
            }
        }
    }
}
