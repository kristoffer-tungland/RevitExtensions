using System;
using System.Collections.Generic;
using System.Globalization;
using Autodesk.Revit.DB;
using RevitExtensions.Utilities;

namespace RevitExtensions
{
    /// <summary>
    /// Provides helper conversions for parameter values using registered converters.
    /// </summary>
    internal static class CustomConverter
    {
        private static readonly Dictionary<(Type, Type), object> _converters =
            new Dictionary<(Type, Type), object>();

        static CustomConverter()
        {
            Register(new ObjectToDoubleConverter());
            Register(new ObjectToIntConverter());
            Register(new ObjectToElementIdConverter());
            Register(new ObjectToStringConverter());
        }

        public static void Register<TFrom, TTo>(IParameterConverter<TFrom, TTo> converter)
        {
            _converters[(typeof(TFrom), typeof(TTo))] = converter;
        }

        public static bool TryConvert<TFrom, TTo>(TFrom value, Parameter parameter, out TTo result)
        {
            if (_converters.TryGetValue((typeof(TFrom), typeof(TTo)), out var convObj))
            {
                return ((IParameterConverter<TFrom, TTo>)convObj).TryConvert(value, parameter, out result);
            }

            result = default!;
            return false;
        }

        public static bool TryConvert<TTo>(object value, Parameter parameter, out TTo result)
        {
            if (value is TTo direct)
            {
                result = direct;
                return true;
            }

            if (_converters.TryGetValue((typeof(object), typeof(TTo)), out var convObj))
            {
                return ((IParameterConverter<object, TTo>)convObj).TryConvert(value, parameter, out result);
            }

            try
            {
                result = (TTo)Convert.ChangeType(value, typeof(TTo), CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                result = default!;
                return false;
            }
        }

        public static object? ChangeType(object value, Type targetType)
        {
            if (value == null) return null;
            if (targetType.IsInstanceOfType(value)) return value;

            if (targetType == typeof(bool) || targetType == typeof(bool?))
                return ToBoolean(value);

            if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                return ToDateTime(value);

            return Convert.ChangeType(value, Nullable.GetUnderlyingType(targetType) ?? targetType, CultureInfo.InvariantCulture);
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
                _ => (DateTime)Convert.ChangeType(value, typeof(DateTime), CultureInfo.InvariantCulture)
            };
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

        // Converter implementations
        private class ObjectToDoubleConverter : IParameterConverter<object, double>
        {
            public bool TryConvert(object value, Parameter parameter, out double result)
            {
                double scale = parameter.Element?.Document?.GetLengthUnitScale() ?? 1.0;

                if (value is string sv && sv.StartsWith("="))
                    return UnitExpressionEvaluator.TryEvaluate(sv, scale, out result);

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
                        break;
                    default:
                        if (value is IConvertible conv)
                        {
                            try { result = Convert.ToDouble(conv, CultureInfo.InvariantCulture); return true; } catch { }
                        }
                        break;
                }
                result = default;
                return false;
            }
        }

        private class ObjectToIntConverter : IParameterConverter<object, int>
        {
            public bool TryConvert(object value, Parameter parameter, out int result)
            {
                double scale = parameter.Element?.Document?.GetLengthUnitScale() ?? 1.0;

                if (value is string sv && sv.StartsWith("="))
                {
                    if (UnitExpressionEvaluator.TryEvaluate(sv, scale, out var d))
                    {
                        result = (int)Math.Round(d);
                        return true;
                    }
                    result = default;
                    return false;
                }

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
                        break;
                    default:
                        if (value is IConvertible conv)
                        {
                            try { result = Convert.ToInt32(conv, CultureInfo.InvariantCulture); return true; } catch { }
                        }
                        break;
                }
                result = default;
                return false;
            }
        }

        private class ObjectToElementIdConverter : IParameterConverter<object, ElementId>
        {
            public bool TryConvert(object value, Parameter parameter, out ElementId result)
            {
                switch (value)
                {
                    case ElementId e:
                        result = e; return true;
                    case int i:
                        result = new ElementId(i); return true;
                    case long l:
                        result = new ElementId((int)l); return true;
                    case string s when int.TryParse(s, out var vi):
                        result = new ElementId(vi); return true;
                    default:
                        result = null;
                        return false;
                }
            }
        }

        private class ObjectToStringConverter : IParameterConverter<object, string>
        {
            public bool TryConvert(object value, Parameter parameter, out string result)
            {
                result = CustomConverter.ToString(value);
                return result != null;
            }
        }
    }
}
