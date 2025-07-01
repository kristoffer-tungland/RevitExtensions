using Autodesk.Revit.DB;

namespace RevitExtensions
{
#if REVIT2021_OR_LESS
    /// <summary>
    /// Extensions for <see cref="ForgeTypeId"/> when running on older Revit versions.
    /// </summary>
    internal static class ForgeTypeIdExtensions
    {
        /// <summary>
        /// Maps a <see cref="ForgeTypeId"/> representing a unit to the corresponding <see cref="DisplayUnitType"/>.
        /// </summary>
        public static DisplayUnitType ToDisplayUnitType(this ForgeTypeId id)
        {
            var tid = id.TypeId.ToLowerInvariant();
            if (tid == "unit:meter") return DisplayUnitType.DUT_METERS;
            if (tid == "unit:centimeter") return DisplayUnitType.DUT_CENTIMETERS;
            if (tid == "unit:millimeter") return DisplayUnitType.DUT_MILLIMETERS;
            if (tid == "unit:inch") return DisplayUnitType.DUT_DECIMAL_INCHES;
            if (tid == "unit:feet") return DisplayUnitType.DUT_DECIMAL_FEET;
            return DisplayUnitType.DUT_DECIMAL_FEET;
        }

        /// <summary>
        /// Maps a specification identifier to the corresponding <see cref="UnitType"/>.
        /// Currently only length is supported in the API stubs.
        /// </summary>
        public static UnitType ToUnitType(this ForgeTypeId id)
        {
            var tid = id.TypeId.ToLowerInvariant();
            if (tid.Contains("length")) return UnitType.UT_Length;
            return UnitType.UT_Length;
        }
    }
#endif
}

