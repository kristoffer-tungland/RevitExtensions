using System;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for <see cref="FilteredElementCollector"/>.
    /// </summary>
    public static class FilteredElementCollectorExtensions
    {
        /// <summary>
        /// Filters the collector for instances of the specified type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="collector">The collector to filter.</param>
        /// <returns>The filtered collector.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is null.</exception>
        public static FilteredElementCollector InstancesOf<T>(this FilteredElementCollector collector) where T : Element
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));

            return collector
                .OfClass(typeof(T))
                .WhereElementIsNotElementType();
        }

        /// <summary>
        /// Filters the collector for element types of the specified type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="collector">The collector to filter.</param>
        /// <returns>The filtered collector.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is null.</exception>
        public static FilteredElementCollector TypesOf<T>(this FilteredElementCollector collector) where T : Element
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));

            return collector
                .OfClass(typeof(T))
                .WhereElementIsElementType();
        }

        /// <summary>
        /// Filters the collector for instances in the specified category.
        /// </summary>
        /// <param name="collector">The collector to filter.</param>
        /// <param name="category">The built-in category.</param>
        /// <returns>The filtered collector.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is null.</exception>
        public static FilteredElementCollector InstancesOf(this FilteredElementCollector collector, BuiltInCategory category)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));

            var filter = new ElementCategoryFilter(category);
            return collector
                .WherePasses(filter)
                .WhereElementIsNotElementType();
        }

        /// <summary>
        /// Filters the collector for element types in the specified category.
        /// </summary>
        /// <param name="collector">The collector to filter.</param>
        /// <param name="category">The built-in category.</param>
        /// <returns>The filtered collector.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is null.</exception>
        public static FilteredElementCollector TypesOf(this FilteredElementCollector collector, BuiltInCategory category)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));

            var filter = new ElementCategoryFilter(category);
            return collector
                .WherePasses(filter)
                .WhereElementIsElementType();
        }

        /// <summary>
        /// Filters the collector for instances in the specified categories.
        /// </summary>
        /// <param name="collector">The collector to filter.</param>
        /// <param name="categories">The built-in categories.</param>
        /// <returns>The filtered collector.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> or <paramref name="categories"/> is null.</exception>
        public static FilteredElementCollector InstancesOf(this FilteredElementCollector collector, System.Collections.Generic.IEnumerable<BuiltInCategory> categories)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (categories == null) throw new ArgumentNullException(nameof(categories));

            var list = new System.Collections.Generic.List<BuiltInCategory>(categories);
            var filter = new ElementMulticategoryFilter(list);
            return collector
                .WherePasses(filter)
                .WhereElementIsNotElementType();
        }

        /// <summary>
        /// Filters the collector for element types in the specified categories.
        /// </summary>
        /// <param name="collector">The collector to filter.</param>
        /// <param name="categories">The built-in categories.</param>
        /// <returns>The filtered collector.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> or <paramref name="categories"/> is null.</exception>
        public static FilteredElementCollector TypesOf(this FilteredElementCollector collector, System.Collections.Generic.IEnumerable<BuiltInCategory> categories)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (categories == null) throw new ArgumentNullException(nameof(categories));

            var list = new System.Collections.Generic.List<BuiltInCategory>(categories);
            var filter = new ElementMulticategoryFilter(list);
            return collector
                .WherePasses(filter)
                .WhereElementIsElementType();
        }

        /// <summary>
        /// Invokes an action for each element in the collector.
        /// </summary>
        /// <param name="collector">The collector to enumerate.</param>
        /// <param name="action">The action to invoke for each element.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="collector"/> or <paramref name="action"/> is null.
        /// </exception>
        public static void ForEach(this FilteredElementCollector collector, Action<Element> action)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var enumerator = collector.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var element = enumerator.Current;
                    try
                    {
                        action(element);
                    }
                    finally
                    {
                        if (element is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }
    }
}

