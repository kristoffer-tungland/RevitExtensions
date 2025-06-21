using System;
using System.Globalization;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Provides helper conversions for parameter values.
    /// </summary>
    internal static class CustomConvert
    {
        public static object ChangeType(object value, Type targetType)
        {
            if (value == null) return null;
            if (targetType.IsInstanceOfType(value)) return value;

            if (targetType == typeof(bool) || targetType == typeof(bool?))
                return ToBoolean(value);

            if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                return ToDateTime(value);

            return Convert.ChangeType(value, Nullable.GetUnderlyingType(targetType) ?? targetType);
        }

        public static bool ToBoolean(object value)
        {
            return value switch
            {
                bool b => b,
                int i => i != 0,
                long l => l != 0,
                string s => bool.Parse(s),
                _ => Convert.ToBoolean(value)
            };
        }

        public static DateTime ToDateTime(object value)
        {
            return value switch
            {
                DateTime dt => dt,
                int i => DateTimeOffset.FromUnixTimeSeconds(i).UtcDateTime,
                long l => DateTimeOffset.FromUnixTimeSeconds(l).UtcDateTime,
                double d => DateTime.FromOADate(d),
                string s => DateTime.Parse(s, null, DateTimeStyles.RoundtripKind),
                _ => (DateTime)Convert.ChangeType(value, typeof(DateTime))
            };
        }

        public static bool TryToDouble(object value, out double result)
        {
            result = default;
            switch (value)
            {
                case double d:
                    result = d; return true;
                case bool b:
                    result = b ? 1 : 0; return true;
                case DateTime dt:
                    result = dt.ToOADate(); return true;
                case string s:
                    if (double.TryParse(s, out result)) return true;
                    if (bool.TryParse(s, out var sb)) { result = sb ? 1 : 0; return true; }
                    if (DateTime.TryParse(s, null, DateTimeStyles.RoundtripKind, out var sd)) { result = sd.ToOADate(); return true; }
                    return false;
                default:
                    if (value is IConvertible conv)
                    {
                        try { result = Convert.ToDouble(conv); return true; }
                        catch { }
                    }
                    return false;
            }
        }

        public static bool TryToInt32(object value, out int result)
        {
            result = default;
            switch (value)
            {
                case int i:
                    result = i; return true;
                case bool b:
                    result = b ? 1 : 0; return true;
                case DateTime dt:
                    result = (int)new DateTimeOffset(dt).ToUnixTimeSeconds(); return true;
                case string s:
                    if (int.TryParse(s, out result)) return true;
                    if (bool.TryParse(s, out var sb)) { result = sb ? 1 : 0; return true; }
                    if (DateTime.TryParse(s, null, DateTimeStyles.RoundtripKind, out var sd)) { result = (int)new DateTimeOffset(sd).ToUnixTimeSeconds(); return true; }
                    return false;
                default:
                    if (value is IConvertible conv)
                    {
                        try { result = Convert.ToInt32(conv); return true; }
                        catch { }
                    }
                    return false;
            }
        }

        public static bool TryToElementId(object value, out ElementId? id)
        {
            id = null;
            switch (value)
            {
                case ElementId e:
                    id = e; return true;
                case int i:
                    id = new ElementId(i); return true;
                case long l:
                    id = new ElementId((int)l); return true;
                case string s when int.TryParse(s, out var vi):
                    id = new ElementId(vi); return true;
                default:
                    return false;
            }
        }

        public static string? ToString(object value)
        {
            return value switch
            {
                DateTime dt => dt.ToString("o"),
                ElementId eid => eid.GetElementIdValue().ToString(),
                _ => value?.ToString()
            };
        }
    }
}
