namespace WindowsInterop.CoreAudio
{
    using System;

    internal class AudioEndpointVolumeCallback : IAudioEndpointVolumeCallback
    {
        public event EventHandler<IntPtr> Notify;

        public AudioEndpointVolumeCallback()
        {
        }

        void IAudioEndpointVolumeCallback.OnNotify(IntPtr notifyData)
        {
            this.Notify?.Invoke(null, notifyData);
        }
    }
}
