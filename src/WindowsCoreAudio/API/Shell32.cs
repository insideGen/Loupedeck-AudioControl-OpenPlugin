namespace WindowsCoreAudio.API
{
    using System;
    using System.Runtime.InteropServices;

    public static class Shell32
    {
        /// <summary>
        /// The ExtractIconEx function creates an array of handles to large or small icons
        /// extracted from the specified executable file, DLL, or icon file.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-extracticonexw"></a></remarks>
        [DllImport("shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int ExtractIconEx(string file, int iconIndex, out IntPtr iconLarge, out IntPtr iconSmall, int icons);
    }
}
