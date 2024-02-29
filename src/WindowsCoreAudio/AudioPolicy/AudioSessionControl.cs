namespace WindowsCoreAudio
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    using WindowsCoreAudio.API;

    public class AudioSessionControl : IAudioControlSession, IDisposable
    {
        public event EventHandler<(string displayName, Guid eventContext)> DisplayNameChanged;
        public event EventHandler<(string iconPath, Guid eventContext)> IconPathChanged;
        public event EventHandler<(float volume, bool isMuted, Guid eventContext)> SimpleVolumeChanged;
        public event EventHandler<(int channelCount, IntPtr newVolumes, int channelIndex, Guid eventContext)> ChannelVolumeChanged;
        public event EventHandler<(Guid groupingId, Guid eventContext)> GroupingParamChanged;
        public event EventHandler<AudioSessionState> StateChanged;
        public event EventHandler<AudioSessionDisconnectReason> SessionDisconnected;

        private readonly IAudioSessionControl2 audioSessionControlInterface;
        private readonly AudioSessionEvents audioSessionEvents;

        private string _displayName = null;
        private string _iconPath = null;

        public AudioMeterInformation AudioMeterInformation { get; private set; }

        public SimpleAudioVolume SimpleAudioVolume { get; private set; }

        public int ProcessId { get; private set; }

        public string ExePath { get; private set; }

        public string SessionIdentifier { get; private set; }

        public AudioSessionIdentifier AudioSessionIdentifier { get; private set; }

        public string SessionInstanceIdentifier { get; private set; }

        public AudioSessionInstanceIdentifier AudioSessionInstanceIdentifier { get; private set; }

        public bool IsSystemSoundsSession { get; private set; }

        public AudioSessionState State { get; private set; }

        public string DisplayName
        {
            get => this._displayName;
            private set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this._displayName = value;
                    Marshal.ThrowExceptionForHR(this.audioSessionControlInterface.SetDisplayName(value, Guid.Empty));
                }
            }
        }

        public string IconPath
        {
            get => this._iconPath;
            private set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this._iconPath = value;
                    Marshal.ThrowExceptionForHR(this.audioSessionControlInterface.SetIconPath(value, Guid.Empty));
                }
            }
        }

        public AudioSessionControl(IAudioSessionControl audioSessionControl)
        {
            this.audioSessionControlInterface = audioSessionControl as IAudioSessionControl2;
            if (this.audioSessionControlInterface is IAudioMeterInformation meters)
            {
                this.AudioMeterInformation = new AudioMeterInformation(meters);
            }
            if (this.audioSessionControlInterface is ISimpleAudioVolume volume)
            {
                this.SimpleAudioVolume = new SimpleAudioVolume(volume);
            }
            this.LoadProperties();
            this.audioSessionEvents = new AudioSessionEvents();
            this.audioSessionEvents.DisplayNameChanged += this.OnDisplayNameChanged;
            this.audioSessionEvents.IconPathChanged += this.OnIconPathChanged;
            this.audioSessionEvents.SimpleVolumeChanged += this.OnSimpleVolumeChanged;
            this.audioSessionEvents.ChannelVolumeChanged += this.OnChannelVolumeChanged;
            this.audioSessionEvents.GroupingParamChanged += this.OnGroupingParamChanged;
            this.audioSessionEvents.StateChanged += this.OnStateChanged;
            this.audioSessionEvents.SessionDisconnected += this.OnSessionDisconnected;
            Marshal.ThrowExceptionForHR(this.audioSessionControlInterface.RegisterAudioSessionNotification(this.audioSessionEvents));
        }

        private int GetProcessId()
        {
            Marshal.ThrowExceptionForHR(this.audioSessionControlInterface.GetProcessId(out int pid));
            this.ProcessId = pid;
            return pid;
        }
        
        private AudioSessionState GetState()
        {
            Marshal.ThrowExceptionForHR(this.audioSessionControlInterface.GetState(out AudioSessionState state));
            this.State = state;
            return state;
        }

        public void LoadProperties()
        {
            this.GetProcessId();

            this.GetState();

            Marshal.ThrowExceptionForHR(this.audioSessionControlInterface.GetSessionIdentifier(out string sessionId));
            this.SessionIdentifier = sessionId;
            this.AudioSessionIdentifier = new AudioSessionIdentifier(this.SessionIdentifier);

            Marshal.ThrowExceptionForHR(this.audioSessionControlInterface.GetSessionInstanceIdentifier(out string sessionInstanceId));
            this.SessionInstanceIdentifier = sessionInstanceId;
            this.AudioSessionInstanceIdentifier = new AudioSessionInstanceIdentifier(this.SessionInstanceIdentifier);

            Marshal.ThrowExceptionForHR(this.audioSessionControlInterface.GetDisplayName(out string displayName));
            if (!string.IsNullOrEmpty(displayName) && displayName.StartsWith("@"))
            {
                StringBuilder sb = new StringBuilder(512);
                if (Shlwapi.SHLoadIndirectString(displayName, sb, sb.Capacity, IntPtr.Zero) == 0)
                {
                    this._displayName = sb.ToString();
                }
            }

            Marshal.ThrowExceptionForHR(this.audioSessionControlInterface.GetIconPath(out string iconPath));
            if (!string.IsNullOrEmpty(iconPath) && iconPath.StartsWith("@"))
            {
                this._iconPath = Environment.ExpandEnvironmentVariables(iconPath.TrimStart('@'));
            }

            this.IsSystemSoundsSession = this.audioSessionControlInterface.IsSystemSoundsSession() == 0;

            if (!this.IsSystemSoundsSession)
            {
                if (!string.IsNullOrEmpty(this.AudioSessionIdentifier.ExePath))
                {
                    this.ExePath = DevicePathMapper.FromDevicePath(this.AudioSessionIdentifier.ExePath);
                    if (File.Exists(this.ExePath))
                    {
                        this._displayName = FileVersionInfo.GetVersionInfo(this.ExePath).ProductName;
                        if (string.IsNullOrEmpty(this._displayName))
                        {
                            this._displayName = Path.GetFileNameWithoutExtension(this.ExePath);
                        }
                    }
                }
            }
        }

        public Guid GetGroupingParam()
        {
            Marshal.ThrowExceptionForHR(this.audioSessionControlInterface.GetGroupingParam(out Guid groupingId));
            return groupingId;
        }

        public void SetGroupingParam(Guid groupingId, Guid context)
        {
            Marshal.ThrowExceptionForHR(this.audioSessionControlInterface.SetGroupingParam(groupingId, context));
        }

        private void OnDisplayNameChanged(object sender, (string displayName, Guid eventContext) e)
        {
            this.DisplayNameChanged?.Invoke(this, e);
        }

        private void OnIconPathChanged(object sender, (string iconPath, Guid eventContext) e)
        {
            this.IconPathChanged?.Invoke(this, e);
        }

        private void OnSimpleVolumeChanged(object sender, (float volume, bool isMuted, Guid eventContext) e)
        {
            this.SimpleVolumeChanged?.Invoke(this, e);
        }

        private void OnChannelVolumeChanged(object sender, (int channelCount, IntPtr newVolumes, int channelIndex, Guid eventContext) e)
        {
            this.ChannelVolumeChanged?.Invoke(this, e);
        }

        private void OnGroupingParamChanged(object sender, (Guid groupingId, Guid eventContext) e)
        {
            this.GroupingParamChanged?.Invoke(this, e);
        }

        private void OnStateChanged(object sender, AudioSessionState state)
        {
            this.State = state;
            this.StateChanged?.Invoke(this, state);
        }

        private void OnSessionDisconnected(object sender, AudioSessionDisconnectReason disconnectReason)
        {
            this.SessionDisconnected?.Invoke(this, disconnectReason);
        }

        string IAudioControl.Id { get => this.SessionInstanceIdentifier; }
        bool IAudioControl.Muted { get => this.SimpleAudioVolume.Mute; set => this.SimpleAudioVolume.Mute = value; }
        float IAudioControl.VolumeScalar { get => this.SimpleAudioVolume.Volume; set => this.SimpleAudioVolume.Volume = value; }
        float[] IAudioControl.PeakValues { get => this.AudioMeterInformation.PeakValues.ToArray(); }

        public void Dispose()
        {
            try
            {
                Marshal.ThrowExceptionForHR(this.audioSessionControlInterface.UnregisterAudioSessionNotification(this.audioSessionEvents));
                //Marshal.ReleaseComObject(this.audioSessionControlInterface);
            }
            catch (Exception)
            {
            }
            this.AudioMeterInformation?.Dispose();
            this.SimpleAudioVolume?.Dispose();
            GC.SuppressFinalize(this);
        }

        ~AudioSessionControl()
        {
            this.Dispose();
        }
    }
}
