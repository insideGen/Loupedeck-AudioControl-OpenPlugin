namespace Loupedeck.AudioControlPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using WindowsCoreAudio;
    using WindowsCoreAudio.API;

    internal class AudioInOutDeviceSelectorPage : FolderPage
    {
        private IAudioControlSession Session { get; }
        private DataFlow Flow { get; }

        public AudioInOutDeviceSelectorPage(Folder parent, IAudioControlSession session, DataFlow flow) : base(parent)
        {
            this.Session = session;
            this.Flow = flow;
        }

        public override IEnumerable<string> GetButtonPressActionNames(DeviceType deviceType)
        {
            List<string> actionNames = new List<string>();
            actionNames.Add("Default");
            actionNames.AddRange(AudioControl.MMAudio.Devices.Where(x => x.DataFlow == this.Flow && x.State == DeviceState.Active).Select(x => x.Id));
            return actionNames;
        }

        public override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (AudioControl.TryGetAudioControl(actionParameter, out IAudioControl audioControl))
            {
                return PluginImage.DrawTextImage(audioControl.DisplayName, imageSize);
            }
            return PluginImage.DrawTextImage(actionParameter, imageSize);
        }

        public override BitmapImage GetAdjustmentImage(string actionParameter, PluginImageSize imageSize)
        {
            return PluginImage.DrawBlackImage(imageSize);
        }

        public override bool ProcessTouchEvent(string actionParameter, DeviceTouchEvent touchEvent)
        {
            if (touchEvent.EventType == DeviceTouchEventType.Tap)
            {
                if (AudioControl.TryGetAudioControl(actionParameter, out IAudioControl audioControl))
                {
                    AudioControl.MMAudio.AudioPolicyConfig.SetPersistedDefaultAudioEndpoint(this.Session.ProcessId, this.Flow, audioControl.Id);
                }
                else
                {
                    AudioControl.MMAudio.AudioPolicyConfig.SetPersistedDefaultAudioEndpoint(this.Session.ProcessId, this.Flow, null);
                }
                this.LeavePage();
            }
            return false;
        }
    }
}
