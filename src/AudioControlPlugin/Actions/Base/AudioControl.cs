namespace Loupedeck.AudioControlPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using WindowsCoreAudio;
    using WindowsCoreAudio.API;

    internal static class AudioControl
    {
        public static MMAudio MMAudio { get; } = new MMAudio();

        public enum EndpointType
        {
            Device,
            Session
        }

        public static bool TryGetEndpointType(string endpointId, out EndpointType? type)
        {
            if (!string.IsNullOrEmpty(endpointId))
            {
                type = endpointId.Contains('|') ? EndpointType.Session : EndpointType.Device;
                return true;
            }
            type = null;
            return false;
        }

        public static bool TryGetAudioControl(string endpointId, out IAudioControl audioControl)
        {
            if (!string.IsNullOrEmpty(endpointId))
            {
                if (TryGetEndpointType(endpointId, out EndpointType? type))
                {
                    if (type == EndpointType.Device)
                    {
                        if (AudioControl.MMAudio.Devices.FirstOrDefault(x => x.Id == endpointId) is IAudioControlDevice device)
                        {
                            audioControl = device;
                            return true;
                        }
                    }
                    else if (type == EndpointType.Session)
                    {
                        AudioSessionInstanceIdentifier asii = new AudioSessionInstanceIdentifier(endpointId);
                        IEnumerable<AudioSessionControl> sessions = AudioControl.MMAudio.RenderSessions.Where(x => x.IsSystemSoundsSession == true || x.State != AudioSessionState.Expired).OrderByDescending(x => x.State);
                        if (!string.IsNullOrEmpty(asii.ExePath))
                        {
                            if (asii.ProcessId > 0 && sessions.FirstOrDefault(x => x.AudioSessionInstanceIdentifier.ExePath == asii.ExePath && x.AudioSessionInstanceIdentifier.ProcessId == asii.ProcessId) is IAudioControlSession session1)
                            {
                                audioControl = session1;
                                return true;
                            }
                            else if (sessions.FirstOrDefault(x => x.AudioSessionInstanceIdentifier.ExePath == asii.ExePath) is IAudioControlSession session2)
                            {
                                audioControl = session2;
                                return true;
                            }
                            else if (sessions.FirstOrDefault(x => x.AudioSessionInstanceIdentifier.ProcessId == asii.ProcessId) is IAudioControlSession session3)
                            {
                                audioControl = session3;
                                return true;
                            }
                        }
                        else
                        {
                            if (sessions.FirstOrDefault(x => x.SessionInstanceIdentifier.Equals(asii.ToString())) is IAudioControlSession session)
                            {
                                audioControl = session;
                                return true;
                            }
                        }
                    }
                }
            }
            audioControl = null;
            return false;
        }

        public static AudioImageData CreateAudioData(IAudioControl audioControl, bool highlighted = false)
        {
            AudioImageData audioData = new AudioImageData();

            audioData.Id = audioControl.Id;
            audioData.DisplayName = audioControl.DisplayName;
            audioData.UnmutedIconPath = audioControl.IconPath;

            audioData.Highlighted = highlighted;

            audioData.Muted = audioControl.Muted;
            audioData.VolumeScalar = audioControl.VolumeScalar;

            if (audioControl is IAudioControlDevice device)
            {
                if (audioControl is AudioControl)
                {
                    audioData.IsActive = false;
                }
                else
                {
                    audioData.IsActive = device.State == DeviceState.Active;
                }
                if (PluginSettings.IsWindowsIconStyle)
                {
                    if (device.DataFlow == DataFlow.Capture)
                    {
                        audioData.UnmutedIconPath = "@" + CaptureDevice.GetUnmutedIconPath(audioControl.VolumeScalar) + "," + Color.LimeGreen.Name;
                        audioData.MutedIconPath = "@" + CaptureDevice.GetMutedIconPath() + "," + Color.Red.Name;
                    }
                    else if (device.DataFlow == DataFlow.Render)
                    {
                        audioData.UnmutedIconPath = "@" + RenderDevice.GetUnmutedIconPath(audioControl.VolumeScalar) + "," + Color.LimeGreen.Name;
                        audioData.MutedIconPath = "@" + RenderDevice.GetMutedIconPath() + "," + Color.Red.Name;
                    }
                }
                if (audioData.IsActive)
                {
                    if (device.DataFlow == DataFlow.Capture)
                    {
                        audioData.IsCommunicationsDefault = device.Id == AudioControl.MMAudio.DefaultCommunicationsCapture.Id;
                        audioData.IsMultimediaDefault = device.Id == AudioControl.MMAudio.DefaultMultimediaCapture.Id;
                    }
                    else if (device.DataFlow == DataFlow.Render)
                    {
                        audioData.IsCommunicationsDefault = device.Id == AudioControl.MMAudio.DefaultCommunicationsRender.Id;
                        audioData.IsMultimediaDefault = device.Id == AudioControl.MMAudio.DefaultMultimediaRender.Id;
                    }
                    audioData.Volume = (float)Math.Round(device.Volume, 2);
                }
            }
            else if (audioControl is IAudioControlSession session)
            {
                audioData.IsActive = session.State != AudioSessionState.Expired || session.IsSystemSoundsSession;
                if (!session.IsSystemSoundsSession)
                {
                    audioData.UnmutedIconPath = session.ExePath;
                    audioData.MutedIconPath = null;
                }
            }

            if (audioData.IsActive && PluginSettings.PeakMeterEnabled)
            {
                float[] peakValues = audioControl.PeakValues.ToArray();
                if (peakValues.Length >= 2)
                {
                    audioData.PeakL = (float)Math.Round(peakValues[0], 2);
                    audioData.PeakR = (float)Math.Round(peakValues[1], 2);
                }
                else if (peakValues.Length == 1)
                {
                    audioData.PeakL = (float)Math.Round(peakValues[0], 2);
                    audioData.PeakR = (float)Math.Round(peakValues[0], 2);
                }
            }

            return audioData;
        }

        public static void ToggleMute(IAudioControl audioControl)
        {
            audioControl.Muted = !audioControl.Muted;
        }

        public static void SetVolume(IAudioControl audioControl, int diff)
        {
            bool isScalar = true;
            float fltDiff, targetVolume;
            if (audioControl is IAudioControlDevice device && device.DataFlow == DataFlow.Capture)
            {
                float range = Math.Abs(device.MinDecibels) + Math.Abs(device.MaxDecibels);
                float steps = range / device.IncrementDecibels;
                if (steps <= 100.0f)
                {
                    isScalar = false;
                }
            }

            if (isScalar)
            {
                fltDiff = diff / 100.0f;
                targetVolume = (float)Math.Round(audioControl.VolumeScalar + fltDiff, 2);
                if (targetVolume < 0.0f)
                {
                    audioControl.VolumeScalar = 0.0f;
                }
                else if (targetVolume > 1.0f)
                {
                    audioControl.VolumeScalar = 1.0f;
                }
                else
                {
                    audioControl.VolumeScalar = targetVolume;
                }
            }
            else
            {
                IAudioControlDevice audioControlDevice = audioControl as IAudioControlDevice;
                fltDiff = audioControlDevice.IncrementDecibels * diff;
                targetVolume = audioControlDevice.Volume + fltDiff;
                if (targetVolume < audioControlDevice.MinDecibels)
                {
                    audioControlDevice.Volume = audioControlDevice.MinDecibels;
                }
                else if (targetVolume > audioControlDevice.MaxDecibels)
                {
                    audioControlDevice.Volume = audioControlDevice.MaxDecibels;
                }
                else
                {
                    audioControlDevice.Volume = targetVolume;
                }
            }
        }
    }
}
