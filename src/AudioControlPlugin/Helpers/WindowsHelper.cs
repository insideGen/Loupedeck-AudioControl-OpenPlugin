namespace Loupedeck.AudioControlPlugin
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    using WindowsInterop;
    using WindowsInterop.ModernApp;
    using WindowsInterop.Win32;

    internal static class WindowsHelper
    {
        public static bool TryGetProcessInfo(IntPtr hWnd, out int processId, out string fullProcessName)
        {
            bool result = false;
            processId = -1;
            fullProcessName = null;
            try
            {
                if (hWnd != null)
                {
                    if (User32.GetWindowThreadProcessId(hWnd, out processId) != 0)
                    {
                        int capacity = 2000;
                        StringBuilder builder = new StringBuilder(capacity);
                        IntPtr processPtr = Kernel32.OpenProcess(Kernel32.ProcessFlags.PROCESS_QUERY_LIMITED_INFORMATION, false, processId);
                        if (processPtr != null)
                        {
                            if (Kernel32.QueryFullProcessImageName(processPtr, 0, builder, ref capacity) != 0)
                            {
                                fullProcessName = builder.ToString();
                                result = true;
                            }
                            Kernel32.CloseHandle(processPtr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Warning($"TryGetFullProcessName: {ex.Message}");
            }
            return result;
        }

        public static bool TryGetForegroundProcessInfo(out int processId, out string fullProcessName)
        {
            bool result = false;
            processId = -1;
            fullProcessName = null;
            try
            {
                IntPtr hWnd = User32.GetForegroundWindow();
                if (hWnd != null)
                {
                    if (TryGetProcessInfo(hWnd, out processId, out fullProcessName))
                    {
                        if (fullProcessName.Contains("ApplicationFrameHost"))
                        {
                            WindowEnumerator windowEnumerator = new WindowEnumerator();
                            IntPtr[] handles = windowEnumerator.GetHandles(hWnd);
                            foreach (IntPtr child in handles)
                            {
                                if (TryGetProcessInfo(child, out processId, out fullProcessName))
                                {
                                    if (!fullProcessName.Contains("ApplicationFrameHost"))
                                    {
                                        result = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    Kernel32.CloseHandle(hWnd);
                }
            }
            catch (Exception ex)
            {
                PluginLog.Warning($"TryGetForegroundProcessInfo: {ex.Message}");
            }
            return result;
        }

        public static bool IsPackagedProcess(int processId)
        {
            bool isPackagedProcess = false;
            IntPtr processPtr = Kernel32.OpenProcess(Kernel32.ProcessFlags.PROCESS_QUERY_LIMITED_INFORMATION, false, processId);
            if (processPtr != IntPtr.Zero)
            {
                try
                {
                    int bufferSize = 0;
                    if (Kernel32.GetPackageId(processPtr, ref bufferSize, IntPtr.Zero) == HRESULT.ERROR_INSUFFICIENT_BUFFER)
                    {
                        IntPtr packageIdPtr = Marshal.AllocHGlobal(bufferSize);
                        try
                        {
                            if (Kernel32.GetPackageId(processPtr, ref bufferSize, packageIdPtr) == HRESULT.S_OK)
                            {
                                PackageId packageId = Marshal.PtrToStructure<PackageId>(packageIdPtr);
                                isPackagedProcess = packageId.Publisher.Length > 0;
                            }
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(packageIdPtr);
                        }
                    }
                }
                finally
                {
                    Kernel32.CloseHandle(processPtr);
                }
            }
            return isPackagedProcess;
        }

        private static bool TryGetAppUserModelIdByPid(int processId, out string appUserModelId)
        {
            bool result = false;
            appUserModelId = null;
            var processPtr = Kernel32.OpenProcess(Kernel32.ProcessFlags.PROCESS_QUERY_LIMITED_INFORMATION | Kernel32.ProcessFlags.SYNCHRONIZE, false, processId);
            if (processPtr != IntPtr.Zero)
            {
                try
                {
                    int amuidBufferLength = Kernel32.MAX_AUMID_LEN;
                    StringBuilder amuidBuffer = new StringBuilder(amuidBufferLength);
                    Kernel32.GetApplicationUserModelId(processPtr, ref amuidBufferLength, amuidBuffer);
                    appUserModelId = amuidBuffer.ToString();
                    if (!string.IsNullOrWhiteSpace(appUserModelId))
                    {
                        result = true;
                    }
                }
                finally
                {
                    Kernel32.CloseHandle(processPtr);
                }
            }
            return result;
        }
    }
}
