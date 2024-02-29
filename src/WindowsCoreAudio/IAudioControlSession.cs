namespace WindowsCoreAudio
{
    using WindowsCoreAudio.API;

    public interface IAudioControlSession : IAudioControl
    {
        AudioSessionState State { get; }
        bool IsSystemSoundsSession { get; }
        string ExePath { get; }
        int ProcessId { get; }
    }
}
