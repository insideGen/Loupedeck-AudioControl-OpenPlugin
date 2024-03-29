﻿namespace WindowsInterop.CoreAudio
{
    using System;

    /// <summary>
    /// Wave Header Flags enumeration
    /// </summary>
    /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/mmeapi/ns-mmeapi-wavehdr"></a></remarks>
    [Flags]
    public enum WaveHeaderFlags
    {
        /// <summary>
        /// This buffer is the first buffer in a loop. This flag is used only with output buffers.
        /// </summary>
        BeginLoop = 0x00000004,
        /// <summary>
        /// Set by the device driver to indicate that it is finished with the buffer and is returning it to the application.
        /// </summary>
        Done = 0x00000001,
        /// <summary>
        /// This buffer is the last buffer in a loop. This flag is used only with output buffers.
        /// </summary>
        EndLoop = 0x00000008,
        /// <summary>
        /// Set by Windows to indicate that the buffer is queued for playback.
        /// </summary>
        InQueue = 0x00000010,
        /// <summary>
        /// Set by Windows to indicate that the buffer has been prepared with
        /// the waveInPrepareHeader or waveOutPrepareHeader function.
        /// </summary>
        Prepared = 0x00000002
    }
}
