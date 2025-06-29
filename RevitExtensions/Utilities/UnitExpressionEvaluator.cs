using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;

namespace RevitExtensions.Utilities
{
    /// <summary>
    /// Evaluates arithmetic expressions containing length units.
    /// Supported units are meters (m), centimeters (cm), millimeters (mm),
    /// feet (ft) and inches (in).
    /// </summary>
    internal static class UnitExpressionEvaluator
    {
        private const double FeetPerMeter = 3.28083989501312;
        private const double FeetPerCentimeter = 0.0328083989501312;
        private const double FeetPerMillimeter = 0.00328083989501312;

        private static readonly Regex TokenRegex =
            new Regex(@"(-?\d+(?:\.\d+)?)(?:\s*(m|cm|mm|ft|in))?",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool TryEvaluate(string expression, Parameter parameter, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(expression))
                return false;

            var expr = expression.Trim();
            if (expr.StartsWith("="))
                expr = expr.Substring(1);

            double defaultScale = 1.0;
            var doc = parameter?.Element?.Document;
#if REVIT2022_OR_LESS && !REVIT2023_OR_ABOVE
            if (doc != null)
            {
                var ut = parameter.Definition.ParameterType == ParameterType.Length
                    ? UnitType.UT_Length
                    : UnitType.UT_Length;
                var fo = doc.GetUnits().GetFormatOptions(ut);
                defaultScale = UnitUtils.ConvertToInternalUnits(1, fo.DisplayUnits);
            }
#else
            if (doc != null)
            {
                var spec = parameter.Definition.GetDataType();
                if (spec == null || spec.Empty())
                    spec = SpecTypeId.Number.Length;
                var fo = doc.GetUnits().GetFormatOptions(spec);
                defaultScale = UnitUtils.ConvertToInternalUnits(1, fo.GetUnitTypeId());
            }
#endif

            expr = TokenRegex.Replace(expr, m =>
            {
                var number = double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                var unit = m.Groups[2].Value.ToLowerInvariant();
#if REVIT2022_OR_LESS && !REVIT2023_OR_ABOVE
                double scaled = unit switch
                {
                    "m" => UnitUtils.ConvertToInternalUnits(number, DisplayUnitType.DUT_METERS),
                    "cm" => UnitUtils.ConvertToInternalUnits(number, DisplayUnitType.DUT_CENTIMETERS),
                    "mm" => UnitUtils.ConvertToInternalUnits(number, DisplayUnitType.DUT_MILLIMETERS),
                    "ft" => UnitUtils.ConvertToInternalUnits(number, DisplayUnitType.DUT_DECIMAL_FEET),
                    "in" => UnitUtils.ConvertToInternalUnits(number, DisplayUnitType.DUT_DECIMAL_INCHES),
                    _ => number * defaultScale
                };
#else
                double scaled = unit switch
                {
                    "m" => UnitUtils.ConvertToInternalUnits(number, UnitTypeId.Meters),
                    "cm" => UnitUtils.ConvertToInternalUnits(number, UnitTypeId.Centimeters),
                    "mm" => UnitUtils.ConvertToInternalUnits(number, UnitTypeId.Millimeters),
                    "ft" => UnitUtils.ConvertToInternalUnits(number, UnitTypeId.Feet),
                    "in" => UnitUtils.ConvertToInternalUnits(number, UnitTypeId.Inches),
                    _ => number * defaultScale
                };
#endif
                return scaled.ToString(CultureInfo.InvariantCulture);
            });

            try
            {
                using var table = new DataTable { Locale = CultureInfo.InvariantCulture };
                var resultObj = table.Compute(expr, null);
                value = Convert.ToDouble(resultObj, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
