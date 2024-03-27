namespace WindowsInterop.CoreAudio
{
    using System;
    using System.Runtime.InteropServices;

    public class AudioPolicyConfig1 : IAudioPolicyConfig, IDisposable
    {
        private readonly IAudioPolicyConfig1 audioPolicyConfigInterface;

        public AudioPolicyConfig1()
        {
            Guid iid = typeof(IAudioPolicyConfig1).GUID;
            if (WindowsInterop.Win32.Combase.RoGetActivationFactory("Windows.Media.Internal.AudioPolicyConfig", ref iid, out object factory) == WindowsInterop.HRESULT.S_OK)
            {
                this.audioPolicyConfigInterface = factory as IAudioPolicyConfig1;
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

        ~AudioPolicyConfig1()
        {
            this.Dispose();
        }
    }
}
