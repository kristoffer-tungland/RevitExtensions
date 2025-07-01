using Autodesk.Revit.DB;

namespace RevitExtensions.Utilities
{
    /// <summary>
    /// Helpers for working with unit identifiers.
    /// </summary>
    internal static class UnitIdUtils
    {
        /// <summary>
        /// Attempts to parse a short unit string into a <see cref="ForgeTypeId"/>.
        /// Supports common length symbols like m, cm, mm, ft, in and variants
        /// such as ft-in and m-cm.
        /// </summary>
        public static bool TryParseUnitSymbol(string unit, out ForgeTypeId id)
        {
            id = default!;
            if (string.IsNullOrEmpty(unit))
                return false;

            switch (unit.ToLowerInvariant())
            {
                case "m":
#if REVIT2021_OR_LESS
                    id = new ForgeTypeId("unit:meter");
#else
                    id = UnitTypeId.Meters;
#endif
                    return true;
                case "cm":
#if REVIT2021_OR_LESS
                    id = new ForgeTypeId("unit:centimeter");
#else
                    id = UnitTypeId.Centimeters;
#endif
                    return true;
                case "mm":
#if REVIT2021_OR_LESS
                    id = new ForgeTypeId("unit:millimeter");
#else
                    id = UnitTypeId.Millimeters;
#endif
                    return true;
                case "ft":
                case "ft-in":
#if REVIT2021_OR_LESS
                    id = new ForgeTypeId("unit:feet");
#else
                    id = UnitTypeId.Feet;
#endif
                    return true;
                case "in":
#if REVIT2021_OR_LESS
                    id = new ForgeTypeId("unit:inch");
#else
                    id = UnitTypeId.Inches;
#endif
                    return true;
                case "m-cm":
#if REVIT2021_OR_LESS
                    id = new ForgeTypeId("unit:meter");
#else
                    id = UnitTypeId.Meters;
#endif
                    return true;
                case "yd":
                    id = new ForgeTypeId("unit:yard");
                    return true;
                case "ac":
                    id = new ForgeTypeId("unit:acre");
                    return true;
                case "ha":
                    id = new ForgeTypeId("unit:hectare");
                    return true;
                case "ft\u00B2": // ft²
                case "ft2":
                    id = new ForgeTypeId("unit:square-foot");
                    return true;
                case "m\u00B2": // m²
                case "m2":
                    id = new ForgeTypeId("unit:square-meter");
                    return true;
                case "cm\u00B2":
                case "cm2":
                    id = new ForgeTypeId("unit:square-centimeter");
                    return true;
                case "mm\u00B2":
                case "mm2":
                    id = new ForgeTypeId("unit:square-millimeter");
                    return true;
                case "in\u00B2":
                case "in2":
                    id = new ForgeTypeId("unit:square-inch");
                    return true;
                case "ft\u00B3":
                case "ft3":
                    id = new ForgeTypeId("unit:cubic-foot");
                    return true;
                case "m\u00B3":
                case "m3":
                    id = new ForgeTypeId("unit:cubic-meter");
                    return true;
                case "cm\u00B3":
                case "cm3":
                    id = new ForgeTypeId("unit:cubic-centimeter");
                    return true;
                case "mm\u00B3":
                case "mm3":
                    id = new ForgeTypeId("unit:cubic-millimeter");
                    return true;
                case "in\u00B3":
                case "in3":
                    id = new ForgeTypeId("unit:cubic-inch");
                    return true;
                case "l":
                    id = new ForgeTypeId("unit:liter");
                    return true;
                case "gal":
                    id = new ForgeTypeId("unit:gallon");
                    return true;
                default:
                    return false;
            }
        }
    }
}

