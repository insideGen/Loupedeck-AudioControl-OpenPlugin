namespace WindowsInterop.CoreAudio
{
    using System;
    using System.Runtime.InteropServices;

    [Guid("2A59116D-6C4F-45E0-A74F-707E3FEF9258")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioPolicyConfig1
    {
        int __incomplete__add_CtxVolumeChange();
        int __incomplete__remove_CtxVolumeChanged();
        int __incomplete__add_RingerVibrateStateChanged();
        int __incomplete__remove_RingerVibrateStateChange();
        int __incomplete__SetVolumeGroupGainForId();
        int __incomplete__GetVolumeGroupGainForId();
        int __incomplete__GetActiveVolumeGroupForEndpointId();
        int __incomplete__GetVolumeGroupsForEndpoint();
        int __incomplete__GetCurrentVolumeContext();
        int __incomplete__SetVolumeGroupMuteForId();
        int __incomplete__GetVolumeGroupMuteForId();
        int __incomplete__SetRingerVibrateState();
        int __incomplete__GetRingerVibrateState();
        int __incomplete__SetPreferredChatApplication();
        int __incomplete__ResetPreferredChatApplication();
        int __incomplete__GetPreferredChatApplication();
        int __incomplete__GetCurrentChatApplications();
        int __incomplete__add_ChatContextChanged();
        int __incomplete__remove_ChatContextChanged();
        [PreserveSig]
        HRESULT SetPersistedDefaultAudioEndpoint(int processId, DataFlow flow, Role role, IntPtr deviceId);
        [PreserveSig]
        HRESULT GetPersistedDefaultAudioEndpoint(int processId, DataFlow flow, Role role, [Out, MarshalAs(UnmanagedType.HString)] out string deviceId);
        [PreserveSig]
        HRESULT ClearAllPersistedApplicationDefaultEndpoints();
    }
}
