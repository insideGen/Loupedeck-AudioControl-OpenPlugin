﻿namespace WindowsInterop.Win32
{
    /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellitemimagefactory-getimage"></a></remarks>
    public enum SIIGBF : int
    {
        SIIGBF_RESIZETOFIT = 0,
        SIIGBF_BIGGERSIZEOK = 0x1,
        SIIGBF_MEMORYONLY = 0x2,
        SIIGBF_ICONONLY = 0x4,
        SIIGBF_THUMBNAILONLY = 0x8,
        SIIGBF_INCACHEONLY = 0x10,
        SIIGBF_CROPTOSQUARE = 0x20,
        SIIGBF_WIDETHUMBNAILS = 0x40,
        SIIGBF_ICONBACKGROUND = 0x80,
        SIIGBF_SCALEUP = 0x100
    }
}
