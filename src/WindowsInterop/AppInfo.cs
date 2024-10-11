namespace WindowsInterop
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    using WindowsInterop.ModernApp;
    using WindowsInterop.Win32;

    public class AppInfo
    {
        public int ProcessId { get; private set; }
        public string ExePath { get; private set; }
        public string DisplayName { get; private set; }
        public string LogoPath { get; private set; }

        public AppInfo()
        {
        }

        public static AppInfo FromPath(string path)
        {
            AppInfo appInfo = null;
            if (!string.IsNullOrWhiteSpace(path))
            {
                string displayName = string.Empty;
                if (File.Exists(path))
                {
                    displayName = FileVersionInfo.GetVersionInfo(path).ProductName;
                }
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = Path.GetFileNameWithoutExtension(path);
                }
                appInfo = new AppInfo();
                appInfo.ProcessId = 0;
                appInfo.ExePath = path;
                appInfo.DisplayName = displayName;
                appInfo.LogoPath = path;
            }
            return appInfo;
        }

        public static AppInfo FromProcess(IntPtr hProcess)
        {
            AppInfo appInfo = null;
            if (hProcess != IntPtr.Zero)
            {
                int capacity = 2000;
                StringBuilder builder = new StringBuilder(capacity);
                if (Kernel32.QueryFullProcessImageName(hProcess, 0, builder, ref capacity) != 0)
                {
                    appInfo = FromPath(builder.ToString());
                }
                if (AppxPackage.IsPackagedProcess(hProcess))
                {
                    if (appInfo == null)
                    {
                        appInfo = new AppInfo();
                    }
                    AppxPackage appxPackage = AppxPackage.FromProcess(hProcess);
                    appInfo.ExePath = appxPackage.Path;
                    if (!appxPackage.DisplayName.StartsWith("ms-resource:"))
                    {
                        appInfo.DisplayName = appxPackage.DisplayName;
                    }
                    appInfo.LogoPath = appxPackage.ApplicationUserModelId;
                }
                if (appInfo != null)
                {
                    appInfo.ProcessId = Kernel32.GetProcessId(hProcess);
                }
            }
            return appInfo;
        }

        public static AppInfo FromProcess(int processId)
        {
            AppInfo appInfo = null;
            if (processId > 0)
            {
                IntPtr hProcess = Kernel32.OpenProcess(Kernel32.ProcessFlags.PROCESS_QUERY_LIMITED_INFORMATION, false, processId);
                if (hProcess != IntPtr.Zero)
                {
                    try
                    {
                        appInfo = FromProcess(hProcess);
                    }
                    finally
                    {
                        Kernel32.CloseHandle(hProcess);
                    }
                }
            }
            return appInfo;
        }
    }
}
