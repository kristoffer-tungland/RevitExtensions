using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for numeric values.
    /// </summary>
    public static class NumericExtensions
    {
        extension (int value)
        {
            /// <summary>
            /// Converts an <see cref="int"/> value to an <see cref="ElementId"/>.
            /// </summary>
            /// <param name="value">The id value.</param>
            /// <returns>An <see cref="ElementId"/> created from the value.</returns>
            public ElementId ToElementId()
            {
                return new ElementId(value);
            }

            /// <summary>
            /// Converts the integer value to Revit internal units using the specified unit identifier.
            /// </summary>
            public double ToInternalUnits(ForgeTypeId unitId)
            {
#if REVIT2021_OR_LESS
                return UnitUtils.ConvertToInternalUnits(value, unitId.ToDisplayUnitType());
#else
                return UnitUtils.ConvertToInternalUnits(value, unitId);
#endif
            }

            /// <summary>
            /// Converts an integer value between two unit identifiers.
            /// </summary>
            public double Convert(ForgeTypeId currentUnitTypeId, ForgeTypeId desiredUnitTypeId)
            {
                return ((double)value).Convert(currentUnitTypeId, desiredUnitTypeId);
            }

            /// <summary>
            /// Converts an integer value from internal units to the specified unit identifier.
            /// </summary>
            public double ConvertFromInternalUnits(ForgeTypeId unitTypeId)
            {
                return ((double)value).ConvertFromInternalUnits(unitTypeId);
            }
        }

        extension (long value)
        {
            /// <summary>
            /// Converts a <see cref="long"/> value to an <see cref="ElementId"/>.
            /// </summary>
            /// <param name="value">The id value.</param>
            /// <returns>An <see cref="ElementId"/> created from the value.</returns>
            public ElementId ToElementId()
            {
#if REVIT2024_OR_ABOVE
                return new ElementId(value);
#else
                return new ElementId((int)value);
#endif
            }
        }

        extension (double value)
        {
            /// <summary>
            /// Converts the value to Revit internal units using the specified unit identifier.
            /// </summary>
            public double ToInternalUnits(ForgeTypeId unitId)
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
            public double Convert(ForgeTypeId currentUnitTypeId, ForgeTypeId desiredUnitTypeId)
            {
#if REVIT2021_OR_LESS
                return UnitUtils.Convert(value, currentUnitTypeId.ToDisplayUnitType(), desiredUnitTypeId.ToDisplayUnitType());
#else
                return UnitUtils.Convert(value, currentUnitTypeId, desiredUnitTypeId);
#endif
            }

            /// <summary>
            /// Converts a value from internal units to the specified unit identifier.
            /// </summary>
            public double ConvertFromInternalUnits(ForgeTypeId unitTypeId)
            {
#if REVIT2021_OR_LESS
                return UnitUtils.ConvertFromInternalUnits(value, unitTypeId.ToDisplayUnitType());
#else
                return UnitUtils.ConvertFromInternalUnits(value, unitTypeId);
#endif
            }
        }
    }
}
