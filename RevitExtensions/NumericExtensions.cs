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
    }
}
