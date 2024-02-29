namespace WindowsCoreAudio.API
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class User32
    {
        public enum CLASS_LONG_INDEX : int
        {
            GCL_HICONSM = -34,
            GCL_HICON = -14
        }

        public const int WM_GETICON = 0x7F;

        public const int ICON_SMALL = 0;
        public const int ICON_BIG = 1;
        public const int ICON_SMALL2 = 2;

        /// <summary>
        /// Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-destroyicon"></a></remarks>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which the user is currently working).
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getforegroundwindow"></a></remarks>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified window and, optionally,
        /// the identifier of the process that created the window.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowthreadprocessid"></a></remarks>
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        /// <summary>
        /// Retrieves the specified 32-bit (DWORD) value from the WNDCLASSEX structure associated with the specified window.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getclasslongw"></a></remarks>
        [DllImport("user32.dll", EntryPoint = "GetClassLongW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetClassLong(IntPtr hWnd, CLASS_LONG_INDEX index);

        /// <summary>
        /// Retrieves the specified value from the WNDCLASSEX structure associated with the specified window.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getclasslongptrw"></a></remarks>
        [DllImport("user32.dll", EntryPoint = "GetClassLongPtrW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetClassLongPtr(IntPtr hWnd, CLASS_LONG_INDEX index);

        /// <summary>
        /// Sends the specified message to a window or windows.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendmessage"></a></remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        /// <summary>
        /// Enumerates all top-level windows on the screen by passing the handle to each window,
        /// in turn, to an application-defined callback function.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumwindows"></a></remarks>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EnumWindows(EnumProc enumFunc, IntPtr param);
        public delegate bool EnumProc(IntPtr hWnd, IntPtr param);

        /// <summary>
        /// Enumerates the child windows that belong to the specified parent window by passing the handle
        /// to each child window, in turn, to an application-defined callback function.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumchildwindows"></a></remarks>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc enumFunc, IntPtr param);
        public delegate bool EnumChildProc(IntPtr hWnd, IntPtr param);

        /// <summary>
        /// Copies the text of the specified window's title bar (if it has one) into a buffer.
        /// If the specified window is a control, the text of the control is copied. However,
        /// GetWindowText cannot retrieve the text of a control in another application.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowtextw"></a></remarks>
        [DllImport("user32.dll", EntryPoint = "GetWindowTextW", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// Retrieves the length, in characters, of the specified window's title bar text (if the window has a title bar).
        /// If the specified window is a control, the function retrieves the length of the text within the control.
        /// However, GetWindowTextLength cannot retrieve the length of the text of an edit control in another application.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowtextlengthw"></a></remarks>
        [DllImport("user32.dll", EntryPoint = "GetWindowTextLengthW", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);
    }
}
