namespace WindowsCoreAudio
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using WindowsCoreAudio.API;

    public class WindowEnumerator
    {
        private readonly List<IntPtr> _handles;

        public WindowEnumerator()
        {
            this._handles = new List<IntPtr>();
        }

        private bool EnumWindowCallback(IntPtr hWnd, IntPtr param)
        {
            this._handles.Add(hWnd);
            return true;
        }

        public IntPtr[] GetHandles(IntPtr hWndParent)
        {
            this._handles.Clear();
            if (hWndParent == IntPtr.Zero)
            {
                User32.EnumWindows(this.EnumWindowCallback, IntPtr.Zero);
            }
            else
            {
                User32.EnumChildWindows(hWndParent, this.EnumWindowCallback, IntPtr.Zero);
            }
            return this._handles.ToArray();
        }

        public static IntPtr GetForegroundWindow()
        {
            return User32.GetForegroundWindow();
        }

        public static string GetWindowTitle(IntPtr hWnd)
        {
            int length = User32.GetWindowTextLength(hWnd) + 1;
            StringBuilder title = new StringBuilder(length);
            User32.GetWindowText(hWnd, title, length);
            return title.ToString();
        }
    }
}
