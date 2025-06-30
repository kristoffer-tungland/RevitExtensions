using System;

namespace Autodesk.Revit.DB
{
    /// <summary>
    /// Backport of the ForgeTypeId class for Revit 2021 and earlier.
    /// </summary>
    public class ForgeTypeId : IEquatable<ForgeTypeId>
    {
        private string _typeId;

        public ForgeTypeId()
        {
            _typeId = string.Empty;
        }

        public ForgeTypeId(string typeId)
        {
            _typeId = typeId ?? string.Empty;
        }

        public void Clear()
        {
            _typeId = string.Empty;
        }

        public bool Empty()
        {
            return string.IsNullOrEmpty(_typeId);
        }

        public override bool Equals(object? obj)
        {
            return obj is ForgeTypeId other && NameEquals(other);
        }

        public bool Equals(ForgeTypeId? other)
        {
            return other != null && NameEquals(other);
        }

        public override int GetHashCode()
        {
            return _typeId?.ToLowerInvariant().GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return _typeId;
        }

        public bool NameEquals(ForgeTypeId other)
        {
            if (other == null)
                return false;

            string thisBase = GetBaseTypeId(_typeId);
            string otherBase = GetBaseTypeId(other._typeId);

            return string.Equals(thisBase, otherBase, StringComparison.OrdinalIgnoreCase);
        }

        public bool StrictlyEquals(ForgeTypeId other)
        {
            return other != null &&
                   string.Equals(_typeId, other._typeId, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetBaseTypeId(string typeId)
        {
            if (string.IsNullOrEmpty(typeId))
                return string.Empty;

            int dashIndex = typeId.IndexOf('-');
            return dashIndex > 0 ? typeId.Substring(0, dashIndex) : typeId;
        }

        public static bool operator ==(ForgeTypeId? a, ForgeTypeId? b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a is null || b is null)
                return false;
            return a.NameEquals(b);
        }

        public static bool operator !=(ForgeTypeId? a, ForgeTypeId? b)
        {
            return !(a == b);
        }

        public static bool operator <(ForgeTypeId? a, ForgeTypeId? b)
        {
            if (a is null || b is null)
                return false;
            return string.Compare(a._typeId, b._typeId, StringComparison.OrdinalIgnoreCase) < 0;
        }

        public static bool operator >(ForgeTypeId? a, ForgeTypeId? b)
        {
            if (a is null || b is null)
                return false;
            return string.Compare(a._typeId, b._typeId, StringComparison.OrdinalIgnoreCase) > 0;
        }

        public bool IsValidObject => !string.IsNullOrEmpty(_typeId);

        public string TypeId => _typeId;
    }
}
