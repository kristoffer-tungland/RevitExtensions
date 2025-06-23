using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for the <see cref="BuiltInParameter"/> enumeration.
    /// </summary>
    public static class BuiltInParameterExtensions
    {
        /// <summary>
        /// Converts a built-in parameter to an <see cref="ElementId"/>.
        /// </summary>
        /// <param name="parameter">The built-in parameter.</param>
        /// <returns>The corresponding element id.</returns>
        public static ElementId ToElementId(this BuiltInParameter parameter)
        {
            return new ElementId((int)parameter);
        }
    }
}
