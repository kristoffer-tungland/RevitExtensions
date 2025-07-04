using System;
using System.Collections.Generic;
using System.Globalization;
using Autodesk.Revit.DB;
using RevitExtensions.Utilities;
using RevitExtensions.Models;

namespace RevitExtensions
{
    /// <summary>
    /// Provides parameter-aware conversions between specific types.
    /// Custom converters can be registered by library consumers.
    /// </summary>
    public static class CustomConverter
    {
        private delegate (bool success, object? result) ConverterDelegate(object value, Parameter parameter);

        private static readonly Dictionary<(Type, Type), List<ConverterDelegate>> _converters =
            new Dictionary<(Type, Type), List<ConverterDelegate>>();

        static CustomConverter()
        {
            Register(new StringToDoubleConverter());
            Register(new StringToIntConverter());
            Register(new BoolToIntConverter());
            Register(new IntToBoolConverter());
            Register(new DateTimeToStringConverter());
            Register(new IntToDateTimeConverter());
            Register(new IntToElementIdConverter());
            Register(new LongToElementIdConverter());
            Register(new StringToElementIdConverter());
            Register(new ElementIdToLongConverter());
            Register(new WorksetToIntConverter());
            Register(new WorksetIdToIntConverter());
            Register(new StringToParameterValueConverter());
            Register(new IntToParameterValueConverter());
            Register(new DoubleToParameterValueConverter());
            Register(new ElementIdToParameterValueConverter());
        }

        /// <summary>
        /// Registers a new converter used when changing parameter values.
        /// </summary>
        public static void Register<TFrom, TTo>(IParameterConverter<TFrom, TTo> converter)
        {
            ConverterDelegate del = (obj, param) =>
            {
                if (obj is TFrom from && converter.TryConvert(from, param, out var res))
                    return (true, (object?)res);
                return (false, null);
            };
            var key = (typeof(TFrom), typeof(TTo));
            if (!_converters.TryGetValue(key, out var list))
            {
                list = new List<ConverterDelegate>();
                _converters[key] = list;
            }
            list.Add(del);
        }

        internal static bool TryConvert(object value, Type targetType, Parameter parameter, out object? result)
        {
            if (value == null)
            {
                result = null;
                return !targetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null;
            }

            var sourceType = value.GetType();

            if (targetType.IsAssignableFrom(sourceType))
            {
                result = value;
                return true;
            }

            if (_converters.TryGetValue((sourceType, targetType), out var list))
            {
                foreach (var del in list)
                {
                    var (success, res) = del(value, parameter);
                    if (success)
                    {
                        result = res;
                        return true;
                    }
                }
            }

            try
            {
                result = System.Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to convert the specified value.
        /// </summary>
        public static bool TryConvert<TFrom, TTo>(TFrom value, Parameter parameter, out TTo result)
        {
            if (TryConvert((object?)value!, typeof(TTo), parameter, out var obj) && obj is TTo t)
            {
                result = t;
                return true;
            }

            result = default!;
            return false;
        }

        /// <summary>
        /// Attempts to convert the specified value.
        /// </summary>
        public static bool TryConvert<TTo>(object value, Parameter parameter, out TTo result)
        {
            if (TryConvert(value, typeof(TTo), parameter, out var obj) && obj is TTo t)
            {
                result = t;
                return true;
            }

            result = default!;
            return false;
        }

        /// <summary>
        /// Converts a value to an invariant string representation.
        /// </summary>
        internal static string ToInvariantString(object value)
        {
            return value switch
            {
                DateTime dt => dt.ToString("o"),
                ElementId eid => eid.GetElementIdValue().ToString(CultureInfo.InvariantCulture),
                IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
                _ => value?.ToString() ?? string.Empty
            };
        }

        // Converter implementations
        private class StringToDoubleConverter : IParameterConverter<string, double>
        {
            public bool TryConvert(string value, Parameter parameter, out double result)
            {
                if (value.StartsWith("="))
                    return UnitExpressionEvaluator.TryEvaluate(value, parameter, out result);

                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                    return true;

                // Attempt unit conversion using the expression evaluator when the
                // string does not parse directly as a number.
                return UnitExpressionEvaluator.TryEvaluate(value, parameter, out result);
            }
        }

        private class StringToIntConverter : IParameterConverter<string, int>
        {
            public bool TryConvert(string value, Parameter parameter, out int result)
            {
                if (value.StartsWith("="))
                {
                    if (UnitExpressionEvaluator.TryEvaluate(value, parameter, out var d))
                    {
                        result = (int)Math.Round(d);
                        return true;
                    }
                    result = default;
                    return false;
                }

                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                    return true;

                // Attempt unit conversion if integer parse fails.
                if (UnitExpressionEvaluator.TryEvaluate(value, parameter, out var dd))
                {
                    result = (int)Math.Round(dd);
                    return true;
                }

                return false;
            }
        }

        private class BoolToIntConverter : IParameterConverter<bool, int>
        {
            public bool TryConvert(bool value, Parameter parameter, out int result)
            {
                result = value ? 1 : 0;
                return true;
            }
        }

        private class IntToBoolConverter : IParameterConverter<int, bool>
        {
            public bool TryConvert(int value, Parameter parameter, out bool result)
            {
                result = value != 0;
                return true;
            }
        }

        private class DateTimeToStringConverter : IParameterConverter<DateTime, string>
        {
            public bool TryConvert(DateTime value, Parameter parameter, out string result)
            {
                result = value.ToString("o");
                return true;
            }
        }

        private class IntToDateTimeConverter : IParameterConverter<int, DateTime>
        {
            public bool TryConvert(int value, Parameter parameter, out DateTime result)
            {
                result = DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime;
                return true;
            }
        }

        private class IntToElementIdConverter : IParameterConverter<int, ElementId>
        {
            public bool TryConvert(int value, Parameter parameter, out ElementId result)
            {
                result = new ElementId(value);
                return true;
            }
        }

        private class LongToElementIdConverter : IParameterConverter<long, ElementId>
        {
            public bool TryConvert(long value, Parameter parameter, out ElementId result)
            {
                result = new ElementId((int)value);
                return true;
            }
        }

        private class StringToElementIdConverter : IParameterConverter<string, ElementId>
        {
            public bool TryConvert(string value, Parameter parameter, out ElementId result)
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
                {
                    result = new ElementId(i);
                    return true;
                }
                result = null!;
                return false;
            }
        }

        private class ElementIdToLongConverter : IParameterConverter<ElementId, long>
        {
            public bool TryConvert(ElementId value, Parameter parameter, out long result)
            {
                result = value.GetElementIdValue();
                return true;
            }
        }

        private class WorksetToIntConverter : IParameterConverter<Workset, int>
        {
            public bool TryConvert(Workset value, Parameter parameter, out int result)
            {
                if (value == null)
                {
                    result = default;
                    return false;
                }

                result = value.Id.IntegerValue;
                return true;
            }
        }

        private class WorksetIdToIntConverter : IParameterConverter<WorksetId, int>
        {
            public bool TryConvert(WorksetId value, Parameter parameter, out int result)
            {
                result = value.IntegerValue;
                return true;
            }
        }

        private class StringToParameterValueConverter : IParameterConverter<string, ParameterValueDetailed>
        {
            public bool TryConvert(string value, Parameter parameter, out ParameterValueDetailed result)
            {
                result = new ParameterValueDetailed
                {
                    Value = value,
                    ValueString = parameter.AsValueString(),
                    ValueType = GetValueType(parameter, Models.ParameterValueType.Text)
                };
                return true;
            }
        }

        private class IntToParameterValueConverter : IParameterConverter<int, ParameterValueDetailed>
        {
            public bool TryConvert(int value, Parameter parameter, out ParameterValueDetailed result)
            {
                result = new ParameterValueDetailed
                {
                    Value = value,
                    ValueString = parameter.AsValueString(),
                    ValueType = GetValueType(parameter, Models.ParameterValueType.Integer)
                };
                return true;
            }
        }

        private class DoubleToParameterValueConverter : IParameterConverter<double, ParameterValueDetailed>
        {
            public bool TryConvert(double value, Parameter parameter, out ParameterValueDetailed result)
            {
                result = new ParameterValueDetailed
                {
                    Value = value,
                    ValueString = parameter.AsValueString(),
                    ValueType = GetValueType(parameter, Models.ParameterValueType.Number)
                };
                return true;
            }
        }

        private class ElementIdToParameterValueConverter : IParameterConverter<ElementId, ParameterValueDetailed>
        {
            public bool TryConvert(ElementId value, Parameter parameter, out ParameterValueDetailed result)
            {
                if (value == null)
                {
                    result = default!;
                    return false;
                }

                result = new ParameterValueDetailed
                {
                    Value = value,
                    ValueString = parameter.AsValueString(),
                    ValueType = GetValueType(parameter, Models.ParameterValueType.Element)
                };
                return true;
            }
        }

        private static Models.ParameterValueType GetValueType(Parameter parameter, Models.ParameterValueType defaultType)
        {
            if (parameter == null)
                return defaultType;

            BuiltInParameter? bip = null;

            if (parameter.Definition is InternalDefinition internalDef)
            {
                bip = internalDef.BuiltInParameter;
            }

            if (!bip.HasValue && parameter.Id != null)
            {
                var intValue = (int)parameter.Id.GetElementIdValue();
                if (intValue < 0)
                    bip = (BuiltInParameter)intValue;
            }

            if (bip == BuiltInParameter.ELEM_PARTITION_PARAM)
                return Models.ParameterValueType.Workset;

            return defaultType;
        }
    }
}
