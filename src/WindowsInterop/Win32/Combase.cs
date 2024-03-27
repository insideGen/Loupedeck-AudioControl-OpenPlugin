namespace WindowsInterop.Win32
{
    using System;
    using System.Runtime.InteropServices;

    public static class Combase
    {
        /// <summary>
        /// Gets the activation factory for the specified runtime class.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/roapi/nf-roapi-rogetactivationfactory"></a></remarks>
        [DllImport("combase.dll", PreserveSig = false)]
        public static extern HRESULT RoGetActivationFactory([MarshalAs(UnmanagedType.HString)] string activatableClassId, [In] ref Guid iid, [Out, MarshalAs(UnmanagedType.IInspectable)] out object factory);

        /// <summary>
        /// Creates a new HSTRING based on the specified source string.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winstring/nf-winstring-windowscreatestring"></a></remarks>
        [DllImport("combase.dll", PreserveSig = false)]
        public static extern HRESULT WindowsCreateString([MarshalAs(UnmanagedType.LPWStr)] string src, [In] int length, [Out] out IntPtr hstring);

        /// <summary>
        /// Decrements the reference count of a string buffer.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winstring/nf-winstring-windowsdeletestring"></a></remarks>
        [DllImport("combase.dll", PreserveSig = false)]
        public static extern HRESULT WindowsDeleteString([In] IntPtr hstring);
    }
}
