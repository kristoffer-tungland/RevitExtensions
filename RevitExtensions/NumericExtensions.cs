using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for numeric values.
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// Converts an <see cref="int"/> value to an <see cref="ElementId"/>.
        /// </summary>
        /// <param name="value">The id value.</param>
        /// <returns>An <see cref="ElementId"/> created from the value.</returns>
        public static ElementId ToElementId(this int value)
        {
            return new ElementId(value);
        }

        /// <summary>
        /// Converts a <see cref="long"/> value to an <see cref="ElementId"/>.
        /// </summary>
        /// <param name="value">The id value.</param>
        /// <returns>An <see cref="ElementId"/> created from the value.</returns>
        public static ElementId ToElementId(this long value)
        {
#if REVIT2024_OR_ABOVE
            return new ElementId(value);
#else
            return new ElementId((int)value);
#endif
        }

        /// <summary>
        /// Converts the value to Revit internal units using the specified unit identifier.
        /// </summary>
        public static double ToInternalUnits(this double value, ForgeTypeId unitId)
        {
#if REVIT2021_OR_LESS
            return UnitUtils.ConvertToInternalUnits(value, unitId.ToDisplayUnitType());
#else
            return UnitUtils.ConvertToInternalUnits(value, unitId);
#endif
        }

        /// <summary>
        /// Converts the integer value to Revit internal units using the specified unit identifier.
        /// </summary>
        public static double ToInternalUnits(this int value, ForgeTypeId unitId)
        {
#if REVIT2021_OR_LESS
            return UnitUtils.ConvertToInternalUnits(value, unitId.ToDisplayUnitType());
#else
            return UnitUtils.ConvertToInternalUnits(value, unitId);
#endif
        }

        /// <summary>
        /// Converts a value between two unit identifiers.
        /// </summary>
        public static double Convert(this double value, ForgeTypeId currentUnitTypeId, ForgeTypeId desiredUnitTypeId)
        {
#if REVIT2021_OR_LESS
            return UnitUtils.Convert(value, currentUnitTypeId.ToDisplayUnitType(), desiredUnitTypeId.ToDisplayUnitType());
#else
            return UnitUtils.Convert(value, currentUnitTypeId, desiredUnitTypeId);
#endif
        }

        /// <summary>
        /// Converts an integer value between two unit identifiers.
        /// </summary>
        public static double Convert(this int value, ForgeTypeId currentUnitTypeId, ForgeTypeId desiredUnitTypeId)
        {
            return ((double)value).Convert(currentUnitTypeId, desiredUnitTypeId);
        }

        /// <summary>
        /// Converts a value from internal units to the specified unit identifier.
        /// </summary>
        public static double ConvertFromInternalUnits(this double value, ForgeTypeId unitTypeId)
        {
#if REVIT2021_OR_LESS
            return UnitUtils.ConvertFromInternalUnits(value, unitTypeId.ToDisplayUnitType());
#else
            return UnitUtils.ConvertFromInternalUnits(value, unitTypeId);
#endif
        }

        /// <summary>
        /// Converts an integer value from internal units to the specified unit identifier.
        /// </summary>
        public static double ConvertFromInternalUnits(this int value, ForgeTypeId unitTypeId)
        {
            return ((double)value).ConvertFromInternalUnits(unitTypeId);
        }
    }
}
