using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FFXIV_RotationHelper
{
    public static class NativeMethods
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        internal static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hwnd, int index);
        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void MoveFormWithMouse(this Form form)
        {
            ReleaseCapture();
            SendMessage(form.Handle, WM_NCLBUTTONDOWN, (IntPtr)HT_CAPTION, IntPtr.Zero);
        }

        public static void SetClickThrough(this Form form, bool clickthrough)
        {
            IntPtr hwnd = form.Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            int newStyle = clickthrough ? extendedStyle | WS_EX_TRANSPARENT : extendedStyle & ~WS_EX_TRANSPARENT;

            SetWindowLong(hwnd, GWL_EXSTYLE, newStyle);
        }
    }
}
