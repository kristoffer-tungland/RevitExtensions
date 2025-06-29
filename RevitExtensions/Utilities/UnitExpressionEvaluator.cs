using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

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

        public static bool TryEvaluate(string expression, double defaultScale, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(expression))
                return false;

            var expr = expression.Trim();
            if (expr.StartsWith("="))
                expr = expr.Substring(1);

            expr = TokenRegex.Replace(expr, m =>
            {
                var number = double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                var unit = m.Groups[2].Value.ToLowerInvariant();
                double scaled = unit switch
                {
                    "m" => number * FeetPerMeter,
                    "cm" => number * FeetPerCentimeter,
                    "mm" => number * FeetPerMillimeter,
                    "ft" => number,
                    "in" => number / 12.0,
                    _ => number * defaultScale
                };
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
