namespace WindowsCoreAudio
{
    using System.Runtime.InteropServices;

    using WindowsCoreAudio.API;

    public class AudioEndpointVolumeStepInformation
    {
        private readonly uint step;
        private readonly uint stepCount;

        internal AudioEndpointVolumeStepInformation(IAudioEndpointVolume parent)
        {
            Marshal.ThrowExceptionForHR(parent.GetVolumeStepInfo(out this.step, out this.stepCount));
        }

        public uint Step
        {
            get
            {
                return this.step;
            }
        }

        public uint StepCount
        {
            get
            {
                return this.stepCount;
            }
        }
    }
}
