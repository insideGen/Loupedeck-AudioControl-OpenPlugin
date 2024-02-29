namespace WindowsCoreAudio.API
{
    using System;

    /// <summary>
    /// Specifies the FMTID/PID identifier that programmatically identifies a property.
    /// </summary>
    /// <remarks><a href="https://learn.microsoft.com/en-us/windows/win32/api/wtypes/ns-wtypes-propertykey"></a></remarks>
    public struct PropertyKey : IEquatable<PropertyKey>
    {
        public Guid FormatId;
        public UIntPtr PropertyId;

        public PropertyKey(Guid formatId, UIntPtr propertyId)
        {
            this.FormatId = formatId;
            this.PropertyId = propertyId;
        }

        public bool Equals(PropertyKey other)
        {
            return this.FormatId.Equals(other.FormatId) && this.PropertyId.Equals(other.PropertyId);
        }

        public override int GetHashCode() => (this.FormatId, this.PropertyId).GetHashCode();

        public override bool Equals(object obj) => obj is PropertyKey other && this.Equals(other);

        public static bool operator ==(PropertyKey left, PropertyKey right) => left.Equals(right);

        public static bool operator !=(PropertyKey left, PropertyKey right) => !left.Equals(right);
    }
}
