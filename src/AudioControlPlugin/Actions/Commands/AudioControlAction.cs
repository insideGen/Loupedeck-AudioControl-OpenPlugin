namespace Loupedeck.AudioControlPlugin
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Timers;

    using WindowsInterop.CoreAudio;
    using WindowsInterop.Win32;

    internal class AudioControlAction
    {
        public const string COMMUNICATIONS_NAME = "defaultCommunications";
        public const string COMMUNICATIONS_DISPLAY_NAME = "Default communications";

        public const string MULTIMEDIA_NAME = "defaultMultimedia";
        public const string MULTIMEDIA_DISPLAY_NAME = "Default multimedia";

        public const string FOREGROUND_NAME = "foregroundApplication";
        public const string FOREGROUND_DISPLAY_NAME = "Foreground application";

        public static string ChannelA { get; set; }
        public static string ChannelB { get; set; }
        public static string ChannelC { get; set; }

        private static bool IsHighlighted(string actionParametersString, ActionChannel channel)
        {
            bool highlighted = false;
            if (channel == ActionChannel.A)
            {
                highlighted = actionParametersString == ChannelA;
            }
            else if (channel == ActionChannel.B)
            {
                highlighted = actionParametersString == ChannelB;
            }
            else if (channel == ActionChannel.C)
            {
                highlighted = actionParametersString == ChannelC;
            }
            return highlighted;
        }

        private enum ActionEditorControl
        {
            Channel,
            Type,
            Endpoint
        }

        private enum ActionChannel
        {
            None,
            A,
            B,
            C
        }

        private enum EndpointType
        {
            Capture,
            Render,
            Application
        }

        private IActionEditorAction Parent { get; }
        private ConcurrentDictionary<string, string> KeyValuePairs { get; }
        private ActionImageStore<AudioImageData> ActionImageStore { get; }

        public AudioControlAction(IActionEditorAction action)
        {
            this.Parent = action;

            this.Parent.ActionEditor.AddControlEx(new ActionEditorListbox(name: ActionEditorControl.Type.ToLower(), labelText: ActionEditorControl.Type.ToString())).SetRequired();
            this.Parent.ActionEditor.AddControlEx(new ActionEditorListbox(name: ActionEditorControl.Endpoint.ToLower(), labelText: ActionEditorControl.Endpoint.ToString())).SetRequired();
            this.Parent.ActionEditor.AddControlEx(new ActionEditorListbox(name: ActionEditorControl.Channel.ToLower(), labelText: ActionEditorControl.Channel.ToString())).SetRequired();
            this.Parent.ActionEditor.ControlsStateRequested += this.OnControlsStateRequested;
            this.Parent.ActionEditor.ListboxItemsRequested += this.OnListboxItemsRequested;
            this.Parent.ActionEditor.ControlValueChanged += this.OnControlValueChanged;

            this.KeyValuePairs = new ConcurrentDictionary<string, string>();
            this.ActionImageStore = new ActionImageStore<AudioImageData>(new AudioImageFactory());
        }

        private void OnControlsStateRequested(object sender, ActionEditorControlsStateRequestedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.ActionEditorState.GetControlValue(ActionEditorControl.Channel.ToLower())))
            {
                e.ActionEditorState.SetValue(ActionEditorControl.Channel.ToLower(), ActionChannel.None.ToLower());
            }
            if (string.IsNullOrEmpty(e.ActionEditorState.GetControlValue(ActionEditorControl.Type.ToLower())))
            {
                e.ActionEditorState.SetEnabled(ActionEditorControl.Endpoint.ToLower(), false);
            }
        }

        private void OnListboxItemsRequested(object sender, ActionEditorListboxItemsRequestedEventArgs e)
        {
            if (e.ControlName.Equals(ActionEditorControl.Channel.ToLower()))
            {
                e.AddItem(ActionChannel.None.ToLower(), ActionChannel.None.ToString(), "");
                e.AddItem(ActionChannel.A.ToLower(), ActionChannel.A.ToString(), "");
                e.AddItem(ActionChannel.B.ToLower(), ActionChannel.B.ToString(), "");
                e.AddItem(ActionChannel.C.ToLower(), ActionChannel.C.ToString(), "");
            }
            else if (e.ControlName.Equals(ActionEditorControl.Type.ToLower()))
            {
                e.AddItem(EndpointType.Capture.ToLower(), EndpointType.Capture.ToString(), "");
                e.AddItem(EndpointType.Render.ToLower(), EndpointType.Render.ToString(), "");
                e.AddItem(EndpointType.Application.ToLower(), EndpointType.Application.ToString(), "");
            }
            else if (e.ControlName.Equals(ActionEditorControl.Endpoint.ToLower()))
            {
                string deviceType = e.ActionEditorState.GetControlValue(ActionEditorControl.Type.ToLower());
                if (!string.IsNullOrEmpty(deviceType))
                {
                    this.KeyValuePairs.Clear();
                    if (deviceType.Equals(EndpointType.Capture.ToLower()))
                    {
                        e.AddItem(COMMUNICATIONS_NAME, $"* {COMMUNICATIONS_DISPLAY_NAME}", "");
                        e.AddItem(MULTIMEDIA_NAME, $"* {MULTIMEDIA_DISPLAY_NAME}", "");
                        this.KeyValuePairs.TryAdd(COMMUNICATIONS_NAME, COMMUNICATIONS_DISPLAY_NAME);
                        this.KeyValuePairs.TryAdd(MULTIMEDIA_NAME, MULTIMEDIA_DISPLAY_NAME);
                        foreach (IAudioControl audioControl in AudioControl.MMAudio.CaptureDevices.Where(x => x.State == DeviceState.Active))
                        {
                            e.AddItem(audioControl.Id, audioControl.DisplayName, "");
                            this.KeyValuePairs.TryAdd(audioControl.Id, audioControl.DisplayName);
                        }
                    }
                    else if (deviceType.Equals(EndpointType.Render.ToLower()))
                    {
                        e.AddItem(COMMUNICATIONS_NAME, $"* {COMMUNICATIONS_DISPLAY_NAME}", "");
                        e.AddItem(MULTIMEDIA_NAME, $"* {MULTIMEDIA_DISPLAY_NAME}", "");
                        this.KeyValuePairs.TryAdd(COMMUNICATIONS_NAME, COMMUNICATIONS_DISPLAY_NAME);
                        this.KeyValuePairs.TryAdd(MULTIMEDIA_NAME, MULTIMEDIA_DISPLAY_NAME);
                        foreach (IAudioControl audioControl in AudioControl.MMAudio.RenderDevices.Where(x => x.State == DeviceState.Active))
                        {
                            e.AddItem(audioControl.Id, audioControl.DisplayName, "");
                            this.KeyValuePairs.TryAdd(audioControl.Id, audioControl.DisplayName);
                        }
                    }
                    else if (deviceType.Equals(EndpointType.Application.ToLower()))
                    {
                        e.AddItem(FOREGROUND_NAME, $"* {FOREGROUND_DISPLAY_NAME}", "");
                        this.KeyValuePairs.TryAdd(FOREGROUND_NAME, FOREGROUND_DISPLAY_NAME);
                        foreach (IAudioControl audioControl in AudioControl.MMAudio.RenderSessions)
                        {
                            if (!string.IsNullOrEmpty(audioControl.DisplayName))
                            {
                                AudioSessionIdentifier audioSessionIdentifier = new AudioSessionIdentifier(audioControl.Id);
                                if (!string.IsNullOrEmpty(audioSessionIdentifier.ExePath) && !this.KeyValuePairs.Keys.Any(x => x.Contains(audioSessionIdentifier.ExePath)))
                                {
                                    e.AddItem(audioSessionIdentifier.ToString(), audioControl.DisplayName, "");
                                    this.KeyValuePairs.TryAdd(audioSessionIdentifier.ToString(), audioControl.DisplayName);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnControlValueChanged(object sender, ActionEditorControlValueChangedEventArgs e)
        {
            string displayName = string.Empty;
            string endpoint = e.ActionEditorState.GetControlValue(ActionEditorControl.Endpoint.ToLower());

            if (e.ControlName.Equals(ActionEditorControl.Channel.ToLower()))
            {
            }
            else if (e.ControlName.Equals(ActionEditorControl.Type.ToLower()))
            {
                this.Parent.ActionEditor.ListboxItemsChanged(ActionEditorControl.Endpoint.ToLower());
                e.ActionEditorState.SetEnabled(ActionEditorControl.Endpoint.ToLower(), true);
                e.ActionEditorState.SetValue(ActionEditorControl.Endpoint.ToLower(), string.Empty);
                endpoint = string.Empty;
            }
            else if (e.ControlName.Equals(ActionEditorControl.Endpoint.ToLower()))
            {
            }

            if (this.KeyValuePairs.TryGetValue(endpoint, out string endpointName))
            {
                if (this.Parent is ActionEditorCommand)
                {
                    displayName += "Touch - ";
                }
                else if (this.Parent is ActionEditorAdjustment)
                {
                    displayName += "Dial - ";
                }
                string channel = Enum.Parse(typeof(ActionChannel), e.ActionEditorState.GetControlValue(ActionEditorControl.Channel.ToLower()), true).ToString();
                if (channel != ActionChannel.None.ToString())
                {
                    displayName += $"{channel} - ";
                }
                displayName += $"{endpointName}";
                if (endpoint.Equals(COMMUNICATIONS_NAME) || endpoint.Equals(MULTIMEDIA_NAME))
                {
                    string type = Enum.Parse(typeof(EndpointType), e.ActionEditorState.GetControlValue(ActionEditorControl.Type.ToLower()), true).ToString();
                    displayName += $" {type.ToLower()}";
                }
                if (this.Parent is ActionEditorAdjustment)
                {
                    this.Parent.ResetDisplayName = $"{displayName} - Adjustment reset";
                    displayName += " - Adjustment";
                }
            }

            e.ActionEditorState.SetDisplayName(displayName);
        }

        private string StringifyActionParameters(ActionEditorActionParameters actionParameters)
        {
            string channel = actionParameters.Parameters[ActionEditorControl.Channel.ToLower()];
            string type = actionParameters.Parameters[ActionEditorControl.Type.ToLower()];
            string endpoint = actionParameters.Parameters[ActionEditorControl.Endpoint.ToLower()];
            return $"{channel}+{type}+{endpoint}";
        }

        private bool TryDecodeActionParametersString(string actionParameters, out ActionChannel channel, out EndpointType type, out string endpointId)
        {
            try
            {
                int index = actionParameters.IndexOf('+');
                string channelString = actionParameters.Substring(0, index);
                actionParameters = actionParameters.Substring(index + 1);
                index = actionParameters.IndexOf('+');
                string typeString = actionParameters.Substring(0, index);
                actionParameters = actionParameters.Substring(index + 1);
                
                channel = (ActionChannel)Enum.Parse(typeof(ActionChannel), channelString, true);
                type = (EndpointType)Enum.Parse(typeof(EndpointType), typeString, true);
                endpointId = actionParameters;

                if (type == EndpointType.Capture)
                {
                    if (endpointId.Equals(COMMUNICATIONS_NAME))
                    {
                        endpointId = AudioControl.MMAudio.DefaultCommunicationsCapture.Id;
                    }
                    else if (endpointId.Equals(MULTIMEDIA_NAME))
                    {
                        endpointId = AudioControl.MMAudio.DefaultMultimediaCapture.Id;
                    }
                }
                else if (type == EndpointType.Render)
                {
                    if (endpointId.Equals(COMMUNICATIONS_NAME))
                    {
                        endpointId = AudioControl.MMAudio.DefaultCommunicationsRender.Id;
                    }
                    else if (endpointId.Equals(MULTIMEDIA_NAME))
                    {
                        endpointId = AudioControl.MMAudio.DefaultMultimediaRender.Id;
                    }
                }
                else if (type == EndpointType.Application)
                {
                    if (endpointId.Equals(FOREGROUND_NAME))
                    {
                        if (WindowsHelper.TryGetForegroundProcessInfo(out int processId, out string fileName))
                        {
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                AudioSessionInstanceIdentifier asii = new AudioSessionInstanceIdentifier($"{{0.0.0.00000000}}.{{{Guid.Empty}}}", DevicePathMapper.FromDriveLetter(fileName), $"{{{Guid.Empty}}}", processId);
                                endpointId = asii.ToString();
                                return true;
                            }
                        }
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to decode action parameters string.|{ex.Message}");

                channel = ActionChannel.None;
                type = EndpointType.Capture;
                endpointId = null;

                return false;
            }
        }

        private bool TryDecodeActionParameters(ActionEditorActionParameters actionParameters, out ActionChannel channel, out EndpointType type, out string endpointId)
        {
            try
            {
                string channelString = actionParameters.Parameters[ActionEditorControl.Channel.ToLower()];
                channel = (ActionChannel)Enum.Parse(typeof(ActionChannel), channelString, true);

                string typeString = actionParameters.Parameters[ActionEditorControl.Type.ToLower()];
                type = (EndpointType)Enum.Parse(typeof(EndpointType), typeString, true);

                endpointId = actionParameters.Parameters[ActionEditorControl.Endpoint.ToLower()];

                return true;
            }
            catch
            {
                PluginLog.Error("Failed to decode action parameters.");

                channel = ActionChannel.None;
                type = EndpointType.Capture;
                endpointId = null;

                return false;
            }
        }

        private void RefreshActionImage(string actionParametersString)
        {
            if (this.TryDecodeActionParametersString(actionParametersString, out ActionChannel channel, out EndpointType type, out string endpointId))
            {
                AudioImageData audioImageData = null;
                bool highlighted = AudioControlAction.IsHighlighted(actionParametersString, channel);
                if (AudioControl.TryGetAudioControl(endpointId, out IAudioControl audioControl))
                {
                    audioImageData = AudioControl.CreateAudioData(audioControl, highlighted);
                }
                else
                {
                    if (type == EndpointType.Application)
                    {
                        AudioSessionInstanceIdentifier asii = new AudioSessionInstanceIdentifier(endpointId);
                        string displayName = string.Empty;
                        string exePath = DevicePathMapper.FromDevicePath(asii.ExePath);
                        if (File.Exists(exePath))
                        {
                            displayName = FileVersionInfo.GetVersionInfo(exePath).ProductName;
                            if (string.IsNullOrEmpty(displayName))
                            {
                                displayName = Path.GetFileNameWithoutExtension(exePath);
                            }
                        }
                        audioImageData = new AudioImageData();
                        audioImageData.Id = actionParametersString;
                        audioImageData.DisplayName = displayName;
                        audioImageData.UnmutedIconPath = exePath;
                        audioImageData.IsActive = false;
                        audioImageData.Highlighted = highlighted;
                    }
                    else
                    {
                        audioImageData = new AudioImageData();
                        audioImageData.Id = actionParametersString;
                        audioImageData.DataFlow = type == EndpointType.Capture ? DataFlow.Capture : DataFlow.Render;
                        audioImageData.NotFound = true;
                        audioImageData.Highlighted = highlighted;
                    }
                }
                if (this.ActionImageStore.UpdateImage(actionParametersString, audioImageData))
                {
                    this.Parent.ActionImageChanged();
                }
            }
        }

        private void Plugin_OnElapsed(object sender, ElapsedEventArgs e)
        {
            foreach (string imageId in this.ActionImageStore.ActionImageIds)
            {
                this.RefreshActionImage(imageId);
            }
        }

        public bool OnLoad()
        {
            AudioControlPlugin.RefreshTimer.Elapsed += this.Plugin_OnElapsed;
            this.Parent.ActionImageChanged();
            return true;
        }

        public bool OnUnload()
        {
            AudioControlPlugin.RefreshTimer.Elapsed -= this.Plugin_OnElapsed;
            return true;
        }

        public string GetDisplayName(ActionEditorActionParameters actionParameters)
        {
            return this.Parent.DisplayName;
        }

        public BitmapImage GetImage(ActionEditorActionParameters actionParameters, int imageWidth, int imageHeight)
        {
            if (this.TryDecodeActionParameters(actionParameters, out ActionChannel channel, out EndpointType type, out string endpointId))
            {
                string actionParametersString = this.StringifyActionParameters(actionParameters);
                if (this.ActionImageStore.TryGetImage(actionParametersString, PluginImage.GetImageSize(imageWidth, imageHeight), out BitmapImage bitmapImage))
                {
                    return bitmapImage;
                }
                if (this.Parent is ActionEditorAdjustment)
                {
                    if (channel == ActionChannel.A && string.IsNullOrEmpty(ChannelA))
                    {
                        ChannelA = actionParametersString;
                    }
                    else if (channel == ActionChannel.B && string.IsNullOrEmpty(ChannelB))
                    {
                        ChannelB = actionParametersString;
                    }
                    else if (channel == ActionChannel.C && string.IsNullOrEmpty(ChannelC))
                    {
                        ChannelC = actionParametersString;
                    }
                }
                this.RefreshActionImage(actionParametersString);
            }
            return PluginImage.DrawBlackImage(PluginImage.GetImageSize(imageWidth, imageHeight));
        }

        public bool ProcessButtonEvent2(ActionEditorActionParameters actionParameters, DeviceButtonEvent2 buttonEvent)
        {
            string actionParametersString = this.StringifyActionParameters(actionParameters);
            if (this.TryDecodeActionParametersString(actionParametersString, out ActionChannel channel, out EndpointType type, out string endpointId))
            {
                if (buttonEvent.EventType == DeviceButtonEventType.Press)
                {
                    if (channel == ActionChannel.None)
                    {
                        if (AudioControl.TryGetAudioControl(endpointId, out IAudioControl audioControl))
                        {
                            AudioControl.ToggleMute(audioControl);
                        }
                    }
                    else if (channel == ActionChannel.A)
                    {
                        if (this.TryDecodeActionParametersString(ChannelA, out ActionChannel channelA, out EndpointType typeA, out string endpointAId))
                        {
                            if (AudioControl.TryGetAudioControl(endpointAId, out IAudioControl audioControlA))
                            {
                                AudioControl.ToggleMute(audioControlA);
                            }
                        }
                    }
                    else if (channel == ActionChannel.B)
                    {
                        if (this.TryDecodeActionParametersString(ChannelB, out ActionChannel channelB, out EndpointType typeB, out string endpointBId))
                        {
                            if (AudioControl.TryGetAudioControl(endpointBId, out IAudioControl audioControlB))
                            {
                                AudioControl.ToggleMute(audioControlB);
                            }
                        }
                    }
                    else if (channel == ActionChannel.C)
                    {
                        if (this.TryDecodeActionParametersString(ChannelC, out ActionChannel channelC, out EndpointType typeC, out string endpointCId))
                        {
                            if (AudioControl.TryGetAudioControl(endpointCId, out IAudioControl audioControlC))
                            {
                                AudioControl.ToggleMute(audioControlC);
                            }
                        }
                    }
                }
            }
            return true;
        }

        public bool ProcessTouchEvent(ActionEditorActionParameters actionParameters, DeviceTouchEvent touchEvent)
        {
            string actionParametersString = this.StringifyActionParameters(actionParameters);
            if (this.TryDecodeActionParametersString(actionParametersString, out ActionChannel channel, out EndpointType type, out string endpointId))
            {
                if (touchEvent.EventType == DeviceTouchEventType.Tap)
                {
                    if (channel == ActionChannel.A)
                    {
                        ChannelA = actionParametersString;
                    }
                    else if (channel == ActionChannel.B)
                    {
                        ChannelB = actionParametersString;
                    }
                    else if (channel == ActionChannel.C)
                    {
                        ChannelC = actionParametersString;
                    }
                }
                else if (touchEvent.EventType == DeviceTouchEventType.DoubleTap)
                {
                    if (AudioControl.TryGetAudioControl(endpointId, out IAudioControl audioControl))
                    {
                        AudioControl.ToggleMute(audioControl);
                    }
                }
                else if (touchEvent.EventType == DeviceTouchEventType.LongPress)
                {
                    if (AudioControl.TryGetAudioControl(endpointId, out IAudioControl audioControl))
                    {
                        if (audioControl is IAudioControlDevice audioControlDevice)
                        {
                            bool muted = !audioControlDevice.Muted;
                            foreach (IAudioControlDevice device in AudioControl.MMAudio.Devices.Where(x => x.State == DeviceState.Active && x.DataFlow == audioControlDevice.DataFlow))
                            {
                                device.Muted = muted;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public bool ProcessEncoderEvent(ActionEditorActionParameters actionParameters, DeviceEncoderEvent encoderEvent)
        {
            string actionParametersString = this.StringifyActionParameters(actionParameters);
            if (this.TryDecodeActionParametersString(actionParametersString, out ActionChannel channel, out EndpointType type, out string endpointId))
            {
                if (channel == ActionChannel.None)
                {
                    if (AudioControl.TryGetAudioControl(endpointId, out IAudioControl audioControl))
                    {
                        AudioControl.SetVolume(audioControl, encoderEvent.Clicks);
                    }
                }
                else if (channel == ActionChannel.A)
                {
                    if (this.TryDecodeActionParametersString(ChannelA, out ActionChannel channelA, out EndpointType typeA, out string endpointAId))
                    {
                        if (AudioControl.TryGetAudioControl(endpointAId, out IAudioControl audioControlA))
                        {
                            AudioControl.SetVolume(audioControlA, encoderEvent.Clicks);
                        }
                    }
                }
                else if (channel == ActionChannel.B)
                {
                    if (this.TryDecodeActionParametersString(ChannelB, out ActionChannel channelB, out EndpointType typeB, out string endpointBId))
                    {
                        if (AudioControl.TryGetAudioControl(endpointBId, out IAudioControl audioControlB))
                        {
                            AudioControl.SetVolume(audioControlB, encoderEvent.Clicks);
                        }
                    }
                }
                else if (channel == ActionChannel.C)
                {
                    if (this.TryDecodeActionParametersString(ChannelC, out ActionChannel channelC, out EndpointType typeC, out string endpointCId))
                    {
                        if (AudioControl.TryGetAudioControl(endpointCId, out IAudioControl audioControlC))
                        {
                            AudioControl.SetVolume(audioControlC, encoderEvent.Clicks);
                        }
                    }
                }
            }
            return true;
        }
    }
}
