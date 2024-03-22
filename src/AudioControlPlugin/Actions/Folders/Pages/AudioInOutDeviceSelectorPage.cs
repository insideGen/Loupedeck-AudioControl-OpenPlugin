namespace Loupedeck.AudioControlPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
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
                PluginImage.GetImageSize(imageSize, out int width, out int height);
                using (Icon icon = PluginImage.GetIcon(audioControl.IconPath))
                using (Bitmap iconBmp = icon?.ToBitmap())
                using (Bitmap image = new Bitmap(width, height))
                using (Graphics graphics = Graphics.FromImage(image))
                using (Font valueFont = new Font("Calibri", 11, FontStyle.Bold))
                using (Brush whiteBrush = new SolidBrush(Color.White.BlueLightFilter()))
                using (Pen whitePen = new Pen(whiteBrush))
                using (StringFormat format = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
                {
                    graphics.Clear(Color.Black);

                    int margin1 = 10;
                    int margin2 = 0;
                    int iconHeight = 32;

                    if (icon != null && iconBmp != null)
                    {
                        iconBmp.BlueLightFilter();
                        graphics.DrawImage(iconBmp, (image.Width - icon.Width) / 2, margin1, icon.Width, icon.Height);
                        iconHeight = icon.Height;
                    }

                    RectangleF valueRectangle = new RectangleF(0, margin1 + iconHeight + margin2, image.Width, image.Height - (margin1 + iconHeight + margin2));
                    graphics.DrawString(audioControl.DisplayName, valueFont, whiteBrush, valueRectangle, format);

                    return PluginImage.ToBitmapImage(image);
                }
            }
            return PluginImage.DrawTextImage(actionParameter, true, imageSize);
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
