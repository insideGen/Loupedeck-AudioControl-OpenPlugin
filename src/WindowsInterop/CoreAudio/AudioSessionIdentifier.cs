namespace WindowsInterop.CoreAudio
{
    using System;

    public class AudioSessionIdentifier
    {
        public string DeviceId { get; } = null;
        public string ExePath { get; } = null;
        public string ExeId { get; } = null;

        public AudioSessionIdentifier(string deviceId, string exePath, string exeId)
        {
            this.DeviceId = deviceId;
            this.ExePath = exePath;
            this.ExeId = exeId;
        }

        public AudioSessionIdentifier(string value)
        {
            string[] level1 = value.Split('|');
            string[] level2 = level1[1].Split(new string[] { "%b" }, StringSplitOptions.None);
            this.DeviceId = level1[0];
            this.ExePath = level2[0] == "#" ? null : level2[0];
            this.ExeId = level2[1];
        }

        public override string ToString()
        {
            string exePath = this.ExePath is null ? "#" : this.ExePath;
            return $"{this.DeviceId}|{exePath}%b{this.ExeId}";
        }

        public string ToString2()
        {
            return $"{this.DeviceId}|{this.ExePath}";
        }
    }

    public class AudioSessionInstanceIdentifier : AudioSessionIdentifier
    {
        public int Reserved { get; } = -1;
        public int ProcessId { get; } = -1;

        public AudioSessionInstanceIdentifier(string deviceId, string exePath, string exeId, int processId) : base(deviceId, exePath, exeId)
        {
            this.Reserved = 1;
            this.ProcessId = processId;
        }

        public AudioSessionInstanceIdentifier(string value) : base(value)
        {
            string[] level1 = value.Split('|');
            if (level1.Length > 2)
            {
                string[] level2 = level1[2].Split(new string[] { "%b" }, StringSplitOptions.None);
                try
                {
                    this.Reserved = int.Parse(level2[0]);
                }
                catch
                {
                    this.Reserved = 1;
                }
                try
                {
                    this.ProcessId = int.Parse(level2[1]);
                }
                catch
                {
                    this.ProcessId = -1;
                }
            }
        }

        public override string ToString()
        {
            string processId = this.ProcessId == -1 ? "#" : this.ProcessId.ToString();
            return $"{base.ToString()}|{this.Reserved}%b{processId}";
        }
    }
}
