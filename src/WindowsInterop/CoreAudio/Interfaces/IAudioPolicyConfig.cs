namespace WindowsInterop.CoreAudio
{
    using System;

    public interface IAudioPolicyConfig : IDisposable
    {
        bool SetPersistedDefaultAudioEndpoint(int processId, DataFlow flow, Role role, IntPtr deviceId);
        bool GetPersistedDefaultAudioEndpoint(int processId, DataFlow flow, Role role, out string deviceId);
        bool ClearAllPersistedApplicationDefaultEndpoints();
    }
}
