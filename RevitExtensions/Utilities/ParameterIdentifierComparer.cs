using System.Collections.Generic;

using RevitExtensions.Models;

namespace RevitExtensions.Utilities
{
    /// <summary>
    /// Equality comparer for <see cref="ParameterIdentifier"/> based on its stable representation.
    /// </summary>
    internal sealed class ParameterIdentifierComparer : IEqualityComparer<ParameterIdentifier>
    {
        public bool Equals(ParameterIdentifier? x, ParameterIdentifier? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.ToStableRepresentation() == y.ToStableRepresentation();
        }

        public int GetHashCode(ParameterIdentifier obj)
        {
            return obj.ToStableRepresentation().GetHashCode();
        }
    }
}
