using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Provides helpers for working with document units in tests.
    /// </summary>
    public static class DocumentUnitExtensions
    {
        /// <summary>
        /// Sets the length unit used when evaluating expressions in tests.
        /// The <paramref name="scale"/> represents the conversion factor from
        /// the unit to feet (Revit's internal length unit).
        /// </summary>
        /// <param name="document">The document to configure.</param>
        /// <param name="scale">The scale factor for converting the document unit to feet.</param>
        public static void SetTestLengthUnitScale(this Document document, double scale)
        {
            var units = document.GetUnits();
#if REVIT2021_OR_LESS
            var fo = units.GetFormatOptions(UnitType.UT_Length) ?? new FormatOptions();
            fo.DisplayUnits = scale switch
            {
                1.0 => DisplayUnitType.DUT_DECIMAL_FEET,
                0.00328083989501312 => DisplayUnitType.DUT_MILLIMETERS,
                0.0328083989501312 => DisplayUnitType.DUT_CENTIMETERS,
                3.28083989501312 => DisplayUnitType.DUT_METERS,
                _ => DisplayUnitType.DUT_DECIMAL_FEET
            };
            units.SetFormatOptions(UnitType.UT_Length, fo);
#else
            var fo = units.GetFormatOptions(SpecTypeId.Length) ?? new FormatOptions();
            var unitId = scale switch
            {
                1.0 => UnitTypeId.Feet,
                0.00328083989501312 => UnitTypeId.Millimeters,
                0.0328083989501312 => UnitTypeId.Centimeters,
                3.28083989501312 => UnitTypeId.Meters,
                _ => UnitTypeId.Feet
            };
            fo.SetUnitTypeId(unitId);
            units.SetFormatOptions(SpecTypeId.Length, fo);
#endif
        }
    }
}
