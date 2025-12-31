using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension helpers for <see cref="Definition"/> to retrieve the data type consistently.
    /// </summary>
    public static class ParameterDefinitionExtensions
    {
        extension (Definition definition)
        {
            /// <summary>
            /// Gets the parameter data type as a <see cref="ForgeTypeId"/> for all Revit versions.
            /// </summary>
            /// <value>The data type identifier.</value>
            public ForgeTypeId DataType
            {
                get
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
    }
}
