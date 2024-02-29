namespace WindowsCoreAudio
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.InteropServices;

    using WindowsCoreAudio.API;

    public class AudioSessionManager : IDisposable
    {
        public event EventHandler<AudioSessionControl> SessionCreated;
        public event EventHandler<AudioSessionControl> SessionDisconnected;
        public event EventHandler<AudioSessionState> StateChanged;

        private readonly IAudioSessionManager2 audioSessionManagerComObject;
        private readonly AudioSessionNotification audioSessionNotification;

        public ObservableCollection<AudioSessionControl> Sessions { get; }

        internal AudioSessionManager(IAudioSessionManager audioSessionManagerComObject)
        {
            this.audioSessionManagerComObject = audioSessionManagerComObject as IAudioSessionManager2;
            try
            {
                Marshal.ThrowExceptionForHR(this.audioSessionManagerComObject.GetSessionEnumerator(out IAudioSessionEnumerator sessionEnumeratorComObject));
                this.Sessions = new ObservableCollection<AudioSessionControl>(new AudioSessionCollection(sessionEnumeratorComObject));
                foreach (AudioSessionControl session in this.Sessions)
                {
                    session.StateChanged += this.OnStateChanged;
                    session.SessionDisconnected += this.OnSessionDisconnected;
                }
            }
            catch
            {
                this.Sessions = null;
            }
            this.audioSessionNotification = new AudioSessionNotification();
            this.audioSessionNotification.SessionCreated += this.OnSessionCreated;
            Marshal.ThrowExceptionForHR(this.audioSessionManagerComObject.RegisterSessionNotification(this.audioSessionNotification));
        }

        private void OnSessionCreated(object sender, AudioSessionControl newSession)
        {
            AudioSessionControl audioSessionControl = this.Sessions.FirstOrDefault((AudioSessionControl s) => s.SessionInstanceIdentifier == newSession.SessionInstanceIdentifier);
            if (audioSessionControl == null)
            {
                newSession.StateChanged += this.OnStateChanged;
                newSession.SessionDisconnected += this.OnSessionDisconnected;
                this.Sessions.Add(newSession);
                this.SessionCreated?.Invoke(this, newSession);
            }
            else
            {
                newSession.Dispose();
            }
        }

        private void OnStateChanged(object sender, AudioSessionState state)
        {
            this.StateChanged?.Invoke(sender, state);
        }

        private void OnSessionDisconnected(object sender, AudioSessionDisconnectReason disconnectReason)
        {
            AudioSessionControl disconnectedSession = sender as AudioSessionControl;
            this.SessionDisconnected?.Invoke(sender, disconnectedSession);
            this.Sessions.Remove(disconnectedSession);
            disconnectedSession.Dispose();
        }

        public void Dispose()
        {
            this.Sessions?.ToList().ForEach((AudioSessionControl s) => s.Dispose());
            try
            {
                Marshal.ThrowExceptionForHR(this.audioSessionManagerComObject.UnregisterSessionNotification(this.audioSessionNotification));
                //Marshal.ReleaseComObject(this.audioSessionManagerComObject);
            }
            catch
            {
            }
            GC.SuppressFinalize(this);
        }

        ~AudioSessionManager()
        {
            this.Dispose();
        }
    }
}
