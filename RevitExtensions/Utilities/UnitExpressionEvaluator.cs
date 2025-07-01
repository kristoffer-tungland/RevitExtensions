using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;
using RevitExtensions;

namespace RevitExtensions.Utilities
{
    /// <summary>
    /// Evaluates arithmetic expressions containing length units.
    /// Supported units include meters (m), centimeters (cm), millimeters (mm),
    /// feet (ft), feet and inches (ft-in) and inches (in). Other symbols are
    /// ignored if no conversion is known.
    /// </summary>
    internal static class UnitExpressionEvaluator
    {
        private static readonly Regex TokenRegex =
            new Regex(@"(-?\d+(?:\.\d+)?)(?:\s*([a-zA-Z0-9°²³/\-]+))?",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool TryEvaluate(string expression, Parameter parameter, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(expression))
                return false;

            var expr = expression.Trim();
            if (expr.StartsWith("="))
                expr = expr.Substring(1);

            var doc = parameter?.Element?.Document
                ?? throw new InvalidOperationException("Parameter must belong to a document.");
#if REVIT2021_OR_LESS
            var spec = parameter.Definition.GetDataType();
            if (spec == null || spec.Empty())
                spec = SpecTypeId.Number.Length;
            var fo = doc.GetUnits().GetFormatOptions(spec.ToUnitType());
            var displayUnitId = fo.DisplayUnits.ToForgeTypeId();
#else
            var spec = parameter.Definition.GetDataType();
            if (spec == null || spec.Empty())
                spec = SpecTypeId.Length;
            var fo = doc.GetUnits().GetFormatOptions(spec);
            var displayUnitId = fo.GetUnitTypeId();
#endif

            expr = TokenRegex.Replace(expr, m =>
            {
                var number = double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                var unit = m.Groups[2].Value.ToLowerInvariant();
                if (UnitIdUtils.TryParseUnitSymbol(unit, out var id))
                {
                    number = number.Convert(id, displayUnitId);
                }
                return number.ToString(CultureInfo.InvariantCulture);
            });

            try
            {
                using var table = new DataTable { Locale = CultureInfo.InvariantCulture };
                var resultObj = table.Compute(expr, null);
                var displayResult = Convert.ToDouble(resultObj, CultureInfo.InvariantCulture);
                value = displayResult.ToInternalUnits(displayUnitId);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
