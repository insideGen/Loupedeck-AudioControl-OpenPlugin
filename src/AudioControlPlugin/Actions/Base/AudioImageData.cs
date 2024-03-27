namespace Loupedeck.AudioControlPlugin
{
    using System;

    using WindowsInterop.CoreAudio;

    public class AudioImageData : IActionImageData, IEquatable<AudioImageData>
    {
        public string Id { get; set; }
        public DataFlow DataFlow { get; set; }

        public bool NotFound { get; set; }
        public string DisplayName { get; set; }
        public string UnmutedIconPath { get; set; }
        public string MutedIconPath { get; set; }
        public bool Highlighted { get; set; }
        public bool IsActive { get; set; }

        public bool Muted { get; set; }
        public float Volume { get; set; }
        public float VolumeScalar { get; set; }
        public float PeakL { get; set; }
        public float PeakR { get; set; }

        public bool IsCommunicationsDefault { get; set; }
        public bool IsMultimediaDefault { get; set; }

        public bool Equals(AudioImageData other)
        {
            bool equals = true;
            equals &= this.NotFound == other.NotFound;
            equals &= this.DisplayName == other.DisplayName;
            equals &= this.UnmutedIconPath == other.UnmutedIconPath;
            equals &= this.MutedIconPath == other.MutedIconPath;
            equals &= this.Highlighted == other.Highlighted;
            equals &= this.IsActive == other.IsActive;
            equals &= this.Muted == other.Muted;
            equals &= this.Volume == other.Volume;
            equals &= this.VolumeScalar == other.VolumeScalar;
            equals &= this.PeakL == other.PeakL;
            equals &= this.PeakR == other.PeakR;
            equals &= this.IsCommunicationsDefault == other.IsCommunicationsDefault;
            equals &= this.IsMultimediaDefault == other.IsMultimediaDefault;
            return equals;
        }

        public override int GetHashCode() => (this.NotFound, this.DisplayName, this.UnmutedIconPath, this.MutedIconPath, this.Highlighted, this.IsActive, this.Muted, this.Volume, this.VolumeScalar, this.PeakL, this.PeakR, this.IsCommunicationsDefault, this.IsMultimediaDefault).GetHashCode();

        public override bool Equals(object obj) => obj is AudioImageData other && this.Equals(other);

        bool IEquatable<IActionImageData>.Equals(IActionImageData other) => this.Equals(other);

        public static bool operator ==(AudioImageData left, AudioImageData right) => left.Equals(right);

        public static bool operator !=(AudioImageData left, AudioImageData right) => !left.Equals(right);
    }
}
