#if REVIT2021_OR_LESS
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extensions for <see cref="DisplayUnitType"/> mapping to <see cref="ForgeTypeId"/>.
    /// </summary>
    internal static class DisplayUnitTypeExtensions
    {
        /// <summary>
        /// Maps the display unit to the corresponding forge type identifier.
        /// </summary>
        public static ForgeTypeId ToForgeTypeId(this DisplayUnitType unit)
        {
            return unit switch
            {
                DisplayUnitType.DUT_METERS => new ForgeTypeId("unit:meter"),
                DisplayUnitType.DUT_CENTIMETERS => new ForgeTypeId("unit:centimeter"),
                DisplayUnitType.DUT_MILLIMETERS => new ForgeTypeId("unit:millimeter"),
                DisplayUnitType.DUT_DECIMAL_INCHES => new ForgeTypeId("unit:inch"),
                DisplayUnitType.DUT_DECIMAL_FEET => new ForgeTypeId("unit:feet"),
                _ => new ForgeTypeId("unit:feet")
            };
        }
    }
}
#endif
