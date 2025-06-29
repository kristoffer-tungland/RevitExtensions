#if REVIT2021_OR_LESS
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension helpers for the deprecated <see cref="ParameterType"/> enum.
    /// </summary>
    public static class ParameterTypeExtensions
    {
        /// <summary>
        /// Maps a <see cref="ParameterType"/> to its <see cref="ForgeTypeId"/> equivalent.
        /// </summary>
        public static ForgeTypeId ToForgeTypeId(this ParameterType pt)
        {
            return pt switch
            {
                ParameterType.Text => SpecTypeId.String.Text,
                ParameterType.MultilineText => SpecTypeId.String.Multiline,
                ParameterType.URL => SpecTypeId.String.Url,
                ParameterType.Integer => SpecTypeId.Int.Integer,
                ParameterType.YesNo => SpecTypeId.Boolean.YesNo,
                ParameterType.Material => SpecTypeId.Reference.Material,
                ParameterType.Number => SpecTypeId.Number.General,
                ParameterType.FixtureUnit => SpecTypeId.Number.General,
                ParameterType.Length => SpecTypeId.Number.Length,
                ParameterType.Area => SpecTypeId.Number.Area,
                ParameterType.Volume => SpecTypeId.Number.Volume,
                ParameterType.Angle => SpecTypeId.Number.Angle,
                ParameterType.HVACDensity => SpecTypeId.Number.HVACDensity,
                ParameterType.HVACPower => SpecTypeId.Number.HVACPower,
                ParameterType.HVACTemperature => SpecTypeId.Number.HVACTemperature,
                ParameterType.HVACAirflow => SpecTypeId.Number.HVACAirflow,
                ParameterType.ElectricalCurrent => SpecTypeId.Number.ElectricalCurrent,
                ParameterType.ElectricalPower => SpecTypeId.Number.ElectricalPower,
                _ => SpecTypeId.String.Text,
            };
        }
    }
}
#endif
