namespace WindowsInterop.CoreAudio
{
    using System;
    using System.Runtime.InteropServices;

    public class AudioPolicyConfig2 : IAudioPolicyConfig, IDisposable
    {
        private readonly IAudioPolicyConfig2 audioPolicyConfigInterface;

        public AudioPolicyConfig2()
        {
            Guid iid = typeof(IAudioPolicyConfig2).GUID;
            if (WindowsInterop.Win32.Combase.RoGetActivationFactory("Windows.Media.Internal.AudioPolicyConfig", ref iid, out object factory) == WindowsInterop.HRESULT.S_OK)
            {
                this.audioPolicyConfigInterface = factory as IAudioPolicyConfig2;
            }
        }

        public bool SetPersistedDefaultAudioEndpoint(int processId, DataFlow flow, Role role, IntPtr deviceId)
        {
            return this.audioPolicyConfigInterface.SetPersistedDefaultAudioEndpoint(processId, flow, role, deviceId) == HRESULT.S_OK;
        }

        public bool GetPersistedDefaultAudioEndpoint(int processId, DataFlow flow, Role role, out string deviceId)
        {
            return this.audioPolicyConfigInterface.GetPersistedDefaultAudioEndpoint(processId, flow, role, out deviceId) == HRESULT.S_OK;
        }

        public bool ClearAllPersistedApplicationDefaultEndpoints()
        {
            return this.audioPolicyConfigInterface.ClearAllPersistedApplicationDefaultEndpoints() == HRESULT.S_OK;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (this.audioPolicyConfigInterface != null)
            {
                Marshal.ReleaseComObject(this.audioPolicyConfigInterface);
            }
        }

        ~AudioPolicyConfig2()
        {
            this.Dispose();
        }
    }
}
