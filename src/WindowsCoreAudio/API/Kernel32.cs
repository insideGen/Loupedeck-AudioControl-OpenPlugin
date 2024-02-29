namespace WindowsCoreAudio.API
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class Kernel32
    {
        public const int WAIT_TIMEOUT = 0x00000102;
        public const int MAX_AUMID_LEN = 512;

        /// <summary>
        /// Process Security and Access Rights.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/procthread/process-security-and-access-rights"></a></remarks>
        [Flags]
        public enum ProcessFlags : uint
        {
            PROCESS_QUERY_LIMITED_INFORMATION = 0x00001000,
            SYNCHRONIZE = 0x00100000
        }

        /// <summary>
        /// Describes possible machine architectures.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/sysinfo/image-file-machine-constants"></a></remarks>
        [Flags]
        public enum IMAGE_FILE_MACHINE : int
        {
            I386 = 0x014C,
            AMD64 = 0x8664,
            ARM64 = 0xAA64
        }

        /// <summary>
        /// Represents package identification information, such as name, version, and publisher.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/appmodel/ns-appmodel-package_id"></a></remarks>
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct PACKAGE_ID
        {
            public uint Reserved { get; }

            public uint ProcessorArchitecture { get; }

            public ulong Version { get; }

            [field: MarshalAs(UnmanagedType.LPWStr)]
            public string Name { get; }

            [field: MarshalAs(UnmanagedType.LPWStr)]
            public string Publisher { get; }

            [field: MarshalAs(UnmanagedType.LPWStr)]
            public string ResourceId { get; }

            [field: MarshalAs(UnmanagedType.LPWStr)]
            public string PublisherId { get; }
        }

        /// <summary>
        /// Opens an existing local process object.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-openprocess"></a></remarks>
        [DllImport("kernel32.dll", PreserveSig = true)]
        public static extern IntPtr OpenProcess(ProcessFlags desiredAccess, bool inheritHandle, int processId);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-closehandle"></a></remarks>
        [DllImport("kernel32.dll", PreserveSig = true)]
        public static extern bool CloseHandle(IntPtr handleObject);

        /// <summary>
        /// Waits until the specified object is in the signaled state or the time-out interval elapses.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-waitforsingleobject"></a></remarks>
        [DllImport("kernel32.dll", PreserveSig = true)]
        public static extern int WaitForSingleObject(IntPtr handle, int milliseconds);

        /// <summary>
        /// Gets the package identifier (ID) for the specified process.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/appmodel/nf-appmodel-getpackageid"></a></remarks>
        [DllImport("kernel32.dll", PreserveSig = true)]
        public static extern HRESULT GetPackageId(IntPtr process, ref int bufferLength, IntPtr packageId);

        /// <summary>
        /// Retrieves information about MS-DOS device names. The function can obtain the current mapping for a particular
        /// MS-DOS device name. The function can also obtain a list of all existing MS-DOS device names.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-querydosdevicea"></a></remarks>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int QueryDosDevice([In] string lpDeviceName, [Out] StringBuilder lpTargetPath, [In] int ucchMax);

        /// <summary>
        /// Retrieves the full name of the executable image for the specified process.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-queryfullprocessimagenamew"></a></remarks>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int QueryFullProcessImageName(IntPtr process, int flags, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder exeName, ref int size);

        /// <summary>
        /// Determines whether the specified process is running under WOW64;
        /// also returns additional machine process and architecture information.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/wow64apiset/nf-wow64apiset-iswow64process2"></a></remarks>
        [DllImport("kernel32.dll", PreserveSig = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process2(IntPtr hProcess, out IMAGE_FILE_MACHINE pProcessMachine, out IMAGE_FILE_MACHINE pNativeMachine);

        /// <summary>
        /// Gets the application user model ID for the specified process.
        /// </summary>
        /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/appmodel/nf-appmodel-getapplicationusermodelid"></a></remarks>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int GetApplicationUserModelId(IntPtr hProcess, ref int applicationUserModelIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder applicationUserModelId);
    }
}
