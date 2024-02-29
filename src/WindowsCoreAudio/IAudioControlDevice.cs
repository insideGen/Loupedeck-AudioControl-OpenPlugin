namespace WindowsCoreAudio
{
    using System.Collections.Generic;

    using WindowsCoreAudio.API;

    public interface IAudioControlDevice : IAudioControl
    {
        DataFlow DataFlow { get; }
        DeviceState State { get; }
        float Volume { get; set; }
        float MinDecibels { get; }
        float MaxDecibels { get; }
        float IncrementDecibels { get; }
        IEnumerable<IAudioControlSession> Sessions { get; }
    }
}
