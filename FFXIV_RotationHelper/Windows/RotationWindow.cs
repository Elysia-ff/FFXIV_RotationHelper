using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace FFXIV_RotationHelper
{
    public partial class RotationWindow : Form
    {
        protected override Size DefaultSize { get { return new Size(800, 40); } }

        private List<Rotation> rotations = new List<Rotation>();
        private List<SkillData> skillList = new List<SkillData>();
        private readonly List<PictureBox> pictureList;
        private int currentIdx = 0;
        private int loopCount = 0;
        private string loadedClass = string.Empty;
        public bool IsLoaded { get { return rotations.Exists(x=>x.Data != null); } }
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
                ResetLoop();
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

        public void LoadData(List<Rotation> rotationList)
        {
            rotations = rotationList;
            // Since we validated class was identical across loaded rotations, lets use a kludgy solution to default loadedClass for skill matching later
            loadedClass = rotations[0].Data.Class;

            // Load skill data to rotation list for later concatenation based on later Looping value
            rotations.ForEach(x => x.SkillList = DB.Get(x.Data));
            InitializeLoop();
        }

        private void InitializeLoop()
        {
            // Iterate through each rotation and add skill list.
            currentIdx = 0;
            skillList.Clear();

            foreach (var rotation in rotations)
            {
                // First time through, isLooping will be false and will add all skills in rotation list, subsequent loops will only add those where Loop is true
                if (loopCount==0 || rotation.Loop)
                {
                    skillList.AddRange(rotation.SkillList);
                }
            }

            // Re-init Picture if first time through or second.  Subsequent loops don't need to be reset.
            if (loopCount <= 1)
            {
                pictureList.Clear();
                Controls.Clear();
                // Ensure UI thread safety
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate ()
                    {
                        MakePictureBox();
                    });
                }
                else
                {
                    MakePictureBox();
                }
            }
        }

        // Resets looping flag as well as initializes loops
        private void ResetLoop()
        {
            loopCount = 0;
            InitializeLoop();
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
            // Avoids index out of range race condition with Reset and OnActionCasted that can occur
            if (skillList.Count == pictureList.Count)
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
        }

        public void OnActionCasted(LogData logData)
        {
            if (currentIdx >= skillList.Count)
            {
                return;
            }

            SkillData skillData = skillList[currentIdx];
            if (DB.IsSameAction(loadedClass, logData.GameIdx, skillData.DBIdx))
            {
                ++currentIdx;
                if (currentIdx >= skillList.Count)
                {
                    if (rotations.Exists(x=>x.Loop)) // If there exists a rotation set for looping
                    {
                        loopCount++;
                        InitializeLoop();
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

        public void Reset()
        {
            ResetLoop();
            Reposition();
        }
    }
}
