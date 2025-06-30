using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension helpers for <see cref="Definition"/> to retrieve the data type consistently.
    /// </summary>
    public static class ParameterDefinitionExtensions
    {
        /// <summary>
        /// Gets the parameter data type as a <see cref="ForgeTypeId"/> for all Revit versions.
        /// </summary>
        /// <param name="definition">The parameter definition.</param>
        /// <returns>The data type identifier.</returns>
        public static ForgeTypeId GetDataType(this Definition definition)
        {
#if REVIT2021_OR_LESS
            return definition.ParameterType.ToForgeTypeId();
#else
            // For newer APIs this instance method already exists.
            return definition.GetDataType();
#endif
        }
    }
}
