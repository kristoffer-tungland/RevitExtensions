using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for the <see cref="BuiltInParameter"/> enumeration.
    /// </summary>
    public static class BuiltInParameterExtensions
    {
        extension (BuiltInParameter parameter)
        {
            /// <summary>
            /// Converts a built-in parameter to an <see cref="ElementId"/>.
            /// </summary>
            /// <param name="parameter">The built-in parameter.</param>
            /// <returns>The corresponding element id.</returns>
            public ElementId ToElementId()
            {
                return new ElementId((int)parameter);
            }
        }
    }
}
