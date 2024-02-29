namespace WindowsCoreAudio
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using WindowsCoreAudio.API;

    public class MMAudio : IDisposable
    {
        private readonly MMDeviceEnumerator _deviceEnumerator;

        public event EventHandler<DefaultDeviceEventArgs> DefaultDeviceChanged;
        public event EventHandler<DeviceStateEventArgs> DeviceStateChanged;
        public event EventHandler<string> DevicePropertyChanged;
        public event EventHandler<AudioSessionState> SessionStateChanged;

        public AudioPolicyConfig AudioPolicyConfig { get; }

        public MMDevice DefaultMultimediaCapture { get; private set; }
        public MMDevice DefaultCommunicationsCapture { get; private set; }
        public MMDevice DefaultMultimediaRender { get; private set; }
        public MMDevice DefaultCommunicationsRender { get; private set; }

        public ObservableCollection<MMDevice> Devices { get; }

        public IEnumerable<MMDevice> CaptureDevices {
            get
            {
                return this.Devices.Where(x => x.DataFlow == DataFlow.Capture);
            }
        }

        public IEnumerable<MMDevice> RenderDevices
        {
            get
            {
                return this.Devices.Where(x => x.DataFlow == DataFlow.Render);
            }
        }

        public IEnumerable<AudioSessionControl> RenderSessions
        {
            get
            {
                return this.RenderDevices.Where(x => x.State == DeviceState.Active).SelectMany(x => x.AudioSessionManager.Sessions);
            }
        }

        public MMAudio()
        {
            this._deviceEnumerator = new MMDeviceEnumerator();
            this._deviceEnumerator.DefaultDeviceChanged += this.OnDefaultDeviceChanged;
            this._deviceEnumerator.DeviceAdded += this.OnDeviceAdded;
            this._deviceEnumerator.DeviceRemoved += this.OnDeviceRemoved;
            this._deviceEnumerator.DeviceStateChanged += this.OnDeviceStateChanged;
            this._deviceEnumerator.DevicePropertyChanged += this.OnDevicePropertyChanged;

            this.AudioPolicyConfig = new AudioPolicyConfig();

            this.DefaultMultimediaCapture = this._deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
            this.DefaultCommunicationsCapture = this._deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);

            this.DefaultMultimediaRender = this._deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            this.DefaultCommunicationsRender = this._deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications);

            this.Devices = new ObservableCollection<MMDevice>(this._deviceEnumerator.EnumAudioEndpoints(DataFlow.All, DeviceState.All));

            foreach (MMDevice device in this.Devices)
            {
                device.PropertyChanged += (sender, e) => this.DevicePropertyChanged?.Invoke(this, e);
            }

            foreach (MMDevice device in this.RenderDevices.Where(x => x.State == DeviceState.Active))
            {
                if (device.AudioSessionManager != null)
                {
                    device.AudioSessionManager.SessionCreated += this.OnDeviceSessionCreated;
                    if (device.AudioSessionManager.Sessions != null)
                    {
                        foreach (AudioSessionControl session in device.AudioSessionManager.Sessions)
                        {
                            session.StateChanged += this.OnSessionStateChanged;
                            session.SessionDisconnected += this.OnSessionDisconnected;
                        }
                    }
                }
            }
        }

        public void SetDefaultAudioEndpoint(string deviceId, Role role)
        {
            this._deviceEnumerator.SetDefaultAudioEndpoint(deviceId, role);
        }

        private void OnDefaultDeviceChanged(object sender, DefaultDeviceEventArgs e)
        {
            if (this.Devices.FirstOrDefault(x => x.Id == e.DefaultDeviceId) is MMDevice device)
            {
                if (e.Flow == DataFlow.Capture)
                {
                    if (e.Role == Role.Multimedia)
                    {
                        this.DefaultMultimediaCapture = device;
                    }
                    else if (e.Role == Role.Communications)
                    {
                        this.DefaultCommunicationsCapture = device;
                    }
                }
                else if (e.Flow == DataFlow.Render)
                {
                    if (e.Role == Role.Multimedia)
                    {
                        this.DefaultMultimediaRender = device;
                    }
                    else if (e.Role == Role.Communications)
                    {
                        this.DefaultCommunicationsRender = device;
                    }
                }
            }
            this.DefaultDeviceChanged?.Invoke(this, e);
        }

        private void OnDeviceAdded(object sender, DeviceIdEventArgs e)
        {
            if (this.Devices.FirstOrDefault(x => x.Id == e.DeviceId) is null)
            {
                if (this._deviceEnumerator.GetDevice(e.DeviceId) is MMDevice device)
                {
                    device.PropertyChanged += (a, b) => this.DevicePropertyChanged?.Invoke(this, b);
                    if (device.State == DeviceState.Active)
                    {
                        if (device.AudioSessionManager != null)
                        {
                            device.AudioSessionManager.SessionCreated += this.OnDeviceSessionCreated;
                            if (device.AudioSessionManager.Sessions != null)
                            {
                                foreach (AudioSessionControl session in device.AudioSessionManager.Sessions)
                                {
                                    session.StateChanged += this.OnSessionStateChanged;
                                    session.SessionDisconnected += this.OnSessionDisconnected;
                                }
                            }
                        }
                    }
                    this.Devices.Add(device);
                }
            }
        }

        private void OnDeviceRemoved(object sender, DeviceIdEventArgs e)
        {
            if (this.Devices.FirstOrDefault(x => x.Id == e.DeviceId) is MMDevice device)
            {
                this.Devices.Remove(device);
                device.Dispose();
            }
        }

        private void OnDeviceStateChanged(object sender, DeviceStateEventArgs e)
        {
            if (this.Devices.FirstOrDefault(x => x.Id == e.DeviceId) is MMDevice device)
            {
                device.GetState();
            }
            this.DeviceStateChanged?.Invoke(this, e);
        }
        
        private void OnDevicePropertyChanged(object sender, PropertyValueEventArgs e)
        {
            if (this.Devices.FirstOrDefault(x => x.Id == e.DeviceId) is MMDevice device)
            {
                device.LoadProperties();
            }
        }

        private void OnDeviceSessionCreated(object sender, AudioSessionControl e)
        {
            e.StateChanged += this.OnSessionStateChanged;
            e.SessionDisconnected += this.OnSessionDisconnected;
        }

        private void OnSessionStateChanged(object sender, AudioSessionState e)
        {
            this.SessionStateChanged?.Invoke(sender, e);
        }

        private void OnSessionDisconnected(object sender, AudioSessionDisconnectReason e)
        {
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            foreach (MMDevice device in this.Devices)
            {
                device.Dispose();
            }
            this.Devices.Clear();

            this.AudioPolicyConfig.Dispose();

            this._deviceEnumerator.Dispose();
        }

        ~MMAudio()
        {
            this.Dispose();
        }
    }
}
