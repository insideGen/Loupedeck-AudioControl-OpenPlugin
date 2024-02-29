namespace WindowsCoreAudio
{
    using System;
    using System.Drawing;

    using WindowsCoreAudio.API;

    public class Window
    {
        public Guid Guid { get; }

        public IntPtr Handle { get; }

        public string Title
        {
            get
            {
                return WindowEnumerator.GetWindowTitle(this.Handle);
            }
        }

        public Icon Icon
        {
            get
            {
                IntPtr iconHandle = User32.GetClassLongPtr(this.Handle, User32.CLASS_LONG_INDEX.GCL_HICON);
                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = User32.SendMessage(this.Handle, User32.WM_GETICON, User32.ICON_BIG, 0);
                }
                if (iconHandle != IntPtr.Zero)
                {
                    try
                    {
                        return Icon.FromHandle(iconHandle);
                    }
                    finally
                    {
                        User32.DestroyIcon(iconHandle);
                    }
                }
                return null;
            }
        }

        public Window(IntPtr handle)
        {
            this.Guid = Guid.NewGuid();
            this.Handle = handle;
        }
    }
}
