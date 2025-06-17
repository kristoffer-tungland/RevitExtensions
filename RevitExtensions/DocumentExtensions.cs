using System;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for <see cref="Document"/>.
    /// </summary>
    public static class DocumentExtensions
    {
        /// <summary>
        /// Creates a collector for elements of the specified type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="document">The document to search.</param>
        /// <returns>A filtered element collector for instances of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static FilteredElementCollector InstancesOf<T>(this Document document) where T : Element
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return new FilteredElementCollector(document)
                .InstancesOf<T>();
        }

        /// <summary>
        /// Creates a collector for elements in the specified built-in category.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="category">The built-in category to filter by.</param>
        /// <returns>A collector filtered by the given category.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static FilteredElementCollector InstancesOf(this Document document, BuiltInCategory category)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return new FilteredElementCollector(document)
                .InstancesOf(category);
        }

        /// <summary>
        /// Creates a collector for elements in the specified built-in categories.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="categories">The built-in categories to filter by.</param>
        /// <returns>A collector filtered by the given categories.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> or <paramref name="categories"/> is null.</exception>
        public static FilteredElementCollector InstancesOf(this Document document, System.Collections.Generic.IEnumerable<BuiltInCategory> categories)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (categories == null) throw new ArgumentNullException(nameof(categories));

            return new FilteredElementCollector(document)
                .InstancesOf(categories);
        }
    }
}
