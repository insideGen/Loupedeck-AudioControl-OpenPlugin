namespace Loupedeck.AudioControlPlugin
{
    using System;

    internal interface IActionImageData : IEquatable<IActionImageData>
    {
        string Id { get; }
    }
}
