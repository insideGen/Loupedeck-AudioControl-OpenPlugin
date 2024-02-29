namespace WindowsCoreAudio
{
    using WindowsCoreAudio.API;

    public readonly struct DeviceStateEventArgs
    {
        public readonly string DeviceId;
        public readonly DeviceState NewState;

        public DeviceStateEventArgs(string deviceId, DeviceState newState)
        {
            this.DeviceId = deviceId;
            this.NewState = newState;
        }
    }
}
