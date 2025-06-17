using System;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for Autodesk Revit elements.
    /// </summary>
    public static class ElementExtensions
    {
        /// <summary>
        /// Gets the numeric id of the element as a <see cref="long"/> regardless of Revit version.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The element id as a long.</returns>
        public static long GetElementIdValue(this Element element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            return element.Id.GetElementIdValue();
        }

        /// <summary>
        /// Gets the numeric value of the element id as a <see cref="long"/>.
        /// Handles Revit versions prior to 2024 where the value was stored as an <see cref="int"/>.
        /// </summary>
        /// <param name="id">The element id.</param>
        /// <returns>The id value as a long.</returns>
        public static long GetElementIdValue(this ElementId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
#if REVIT2024_OR_ABOVE
            return id.Value;
#else
            return id.IntegerValue;
#endif
        }
    }
}
