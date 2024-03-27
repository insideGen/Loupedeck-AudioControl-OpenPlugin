namespace WindowsInterop.CoreAudio
{
    using System;

    public class AudioPolicyConfig : IDisposable
    {
        private const string MMDEVAPI_TOKEN = @"\\?\SWD#MMDEVAPI#";
        private const string DEVINTERFACE_AUDIO_RENDER = "#{e6327cad-dcec-4949-ae8a-991e976a79d2}";
        private const string DEVINTERFACE_AUDIO_CAPTURE = "#{2eef81be-33fa-4800-9670-1cd474972c3f}";

        private readonly IAudioPolicyConfig audioPolicyConfigInterface;

        public AudioPolicyConfig()
        {
            if (Environment.OSVersion.Version.Build >= 21390)
            {
                this.audioPolicyConfigInterface = new AudioPolicyConfig2();
            }
            else
            {
                this.audioPolicyConfigInterface = new AudioPolicyConfig1();
            }
        }

        private string Encode(string deviceId, DataFlow flow)
        {
            return $"{MMDEVAPI_TOKEN}{deviceId}{(flow == DataFlow.Render ? DEVINTERFACE_AUDIO_RENDER : DEVINTERFACE_AUDIO_CAPTURE)}";
        }

        private string Decode(string deviceId)
        {
            if (deviceId.StartsWith(MMDEVAPI_TOKEN))
            {
                deviceId = deviceId.Remove(0, MMDEVAPI_TOKEN.Length);
            }

            if (deviceId.EndsWith(DEVINTERFACE_AUDIO_RENDER))
            {
                deviceId = deviceId.Remove(deviceId.Length - DEVINTERFACE_AUDIO_RENDER.Length);
            }

            if (deviceId.EndsWith(DEVINTERFACE_AUDIO_CAPTURE))
            {
                deviceId = deviceId.Remove(deviceId.Length - DEVINTERFACE_AUDIO_CAPTURE.Length);
            }

            return deviceId;
        }

        public void SetPersistedDefaultAudioEndpoint(int processId, DataFlow flow, string deviceId)
        {
            IntPtr hstring = IntPtr.Zero;

            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                string deviceLongId = this.Encode(deviceId, flow);
                Win32.Combase.WindowsCreateString(deviceLongId, deviceLongId.Length, out hstring);
            }

            this.audioPolicyConfigInterface.SetPersistedDefaultAudioEndpoint(processId, flow, Role.Multimedia, hstring);
            this.audioPolicyConfigInterface.SetPersistedDefaultAudioEndpoint(processId, flow, Role.Console, hstring);

            if (hstring != IntPtr.Zero)
            {
                Win32.Combase.WindowsDeleteString(hstring);
            }
        }

        public bool GetPersistedDefaultAudioEndpoint(int processId, DataFlow flow, out string deviceId)
        {
            bool result = this.audioPolicyConfigInterface.GetPersistedDefaultAudioEndpoint(processId, flow, Role.Multimedia | Role.Console, out string deviceLongId);
            deviceId = this.Decode(deviceLongId);
            return result;
        }

        public bool ClearAllPersistedApplicationDefaultEndpoints()
        {
            return this.audioPolicyConfigInterface.ClearAllPersistedApplicationDefaultEndpoints();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.audioPolicyConfigInterface.Dispose();
        }

        ~AudioPolicyConfig()
        {
            this.Dispose();
        }
    }
}
