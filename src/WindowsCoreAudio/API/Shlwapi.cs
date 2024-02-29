namespace WindowsCoreAudio.API
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class Shlwapi
    {
        /// <summary>
        /// Extracts a specified text resource when given that resource in the form
        /// of an indirect string (a string that begins with the '@' symbol).
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/shlwapi/nf-shlwapi-shloadindirectstring"></a></remarks>
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true)]
        internal static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);
    }
}
