using System;
using Autodesk.Revit.DB;
using RevitExtensions.Models;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for <see cref="FilteredElementCollector"/>.
    /// </summary>
    public static class FilteredElementCollectorExtensions
    {
        /// <summary>
        /// Filters the collector to include only element instances.
        /// </summary>
        /// <param name="collector">The collector to filter.</param>
        /// <returns>The filtered collector.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is null.</exception>
        public static FilteredElementCollector Instances(this FilteredElementCollector collector)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));

            return collector.WhereElementIsNotElementType();
        }

        /// <summary>
        /// Filters the collector to include only element types.
        /// </summary>
        /// <param name="collector">The collector to filter.</param>
        /// <returns>The filtered collector.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is null.</exception>
        public static FilteredElementCollector Types(this FilteredElementCollector collector)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));

            return collector.WhereElementIsElementType();
        }

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

        /// <summary>
        /// Invokes an action for each element of type <typeparamref name="T"/> in the collector.
        /// Elements not assignable to <typeparamref name="T"/> are disposed and skipped.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="collector">The collector to enumerate.</param>
        /// <param name="action">The action to invoke for each element.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="collector"/> or <paramref name="action"/> is null.
        /// </exception>
        public static void ForEach<T>(this FilteredElementCollector collector, Action<T> action) where T : Element
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var enumerator = collector.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var element = enumerator.Current;
                    if (element is T typed)
                    {
                        try
                        {
                            action(typed);
                        }
                        finally
                        {
                            typed.Dispose();
                        }
                    }
                    else
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

        /// <summary>
        /// Filters the collector by comparing a parameter value.
        /// </summary>
        /// <param name="collector">The collector to filter.</param>
        /// <param name="parameterId">The element id of the parameter.</param>
        /// <param name="comparison">The comparison type.</param>
        /// <param name="value">The comparison value.</param>
        /// <returns>The same collector instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="collector"/> or <paramref name="parameterId"/> or <paramref name="value"/> is null.
        /// </exception>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            ElementId parameterId,
            StringComparison comparison,
            string value)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var filter = CreateFilter(parameterId, comparison, value);
            if (filter != null)
            {
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector by comparing a parameter value using a built-in parameter.
        /// </summary>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            BuiltInParameter parameter,
            StringComparison comparison,
            string value)
        {
            return collector.Where(parameter.ToElementId(), comparison, value);
        }

        /// <summary>
        /// Filters the collector by comparing an integer parameter value.
        /// </summary>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            ElementId parameterId,
            Comparison comparison,
            int value)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));

            var filter = CreateFilter(parameterId, comparison, value);
            if (filter != null)
            {
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector by comparing an integer parameter value using a built-in parameter.
        /// </summary>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            BuiltInParameter parameter,
            Comparison comparison,
            int value)
        {
            return collector.Where(parameter.ToElementId(), comparison, value);
        }

        /// <summary>
        /// Filters the collector by comparing a double parameter value.
        /// </summary>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            ElementId parameterId,
            Comparison comparison,
            double value)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));

            var filter = CreateFilter(parameterId, comparison, value);
            if (filter != null)
            {
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector by comparing a double parameter value using a built-in parameter.
        /// </summary>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            BuiltInParameter parameter,
            Comparison comparison,
            double value)
        {
            return collector.Where(parameter.ToElementId(), comparison, value);
        }

        /// <summary>
        /// Filters the collector by comparing an element id parameter value.
        /// </summary>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            ElementId parameterId,
            Comparison comparison,
            ElementId value)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var filter = CreateFilter(parameterId, comparison, value);
            if (filter != null)
            {
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector by comparing an element id parameter value using a built-in parameter.
        /// </summary>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            BuiltInParameter parameter,
            Comparison comparison,
            ElementId value)
        {
            return collector.Where(parameter.ToElementId(), comparison, value);
        }

        /// <summary>
        /// Filters the collector by comparing a parameter value using the parameter name.
        /// All parameters with the matching name are combined using a logical OR.
        /// </summary>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            Document document,
            string parameterName,
            StringComparison comparison,
            string value)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            if (value == null) throw new ArgumentNullException(nameof(value));

            return collector.WhereByName(document, parameterName, id => CreateFilter(id, comparison, value));
        }

        /// <summary>
        /// Filters the collector by comparing an integer parameter value using the parameter name.
        /// </summary>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            Document document,
            string parameterName,
            Comparison comparison,
            int value)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            return collector.WhereByName(document, parameterName, id => CreateFilter(id, comparison, value));
        }

        /// <summary>
        /// Filters the collector by comparing a double parameter value using the parameter name.
        /// </summary>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            Document document,
            string parameterName,
            Comparison comparison,
            double value)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            return collector.WhereByName(document, parameterName, id => CreateFilter(id, comparison, value));
        }

        /// <summary>
        /// Filters the collector by comparing an element id parameter value using the parameter name.
        /// </summary>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            Document document,
            string parameterName,
            Comparison comparison,
            ElementId value)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            if (value == null) throw new ArgumentNullException(nameof(value));

            return collector.WhereByName(document, parameterName, id => CreateFilter(id, comparison, value));
        }

        private static FilteredElementCollector WhereByName(
            this FilteredElementCollector collector,
            Document document,
            string parameterName,
            Func<ElementId, ElementParameterFilter?> create)
        {
            var infos = document.GetParametersByName(parameterName);
            var filters = new System.Collections.Generic.List<ElementFilter>();

            foreach (var info in infos)
            {
                var id = info.Identifier.ToElementId();
                if (id == null) continue;
                var filter = create(id);
                if (filter != null)
                    filters.Add(filter);
            }

            if (filters.Count == 0)
                return collector;

            ElementFilter finalFilter = filters.Count == 1
                ? filters[0]
                : new LogicalOrFilter(filters);

            collector.WherePasses(finalFilter);
            return collector;
        }

        // Removed overloads taking ParameterIdentifier or string to simplify API

        /// <summary>
        /// Filters the collector using a logical OR of several parameter comparisons.
        /// </summary>
        public static FilteredElementCollector WhereOr(
            this FilteredElementCollector collector,
            params (ElementId parameterId, StringComparison comparison, string value)[] conditions)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (conditions == null) throw new ArgumentNullException(nameof(conditions));

            var filters = new System.Collections.Generic.List<ElementFilter>();
            foreach (var (parameterId, comparison, value) in conditions)
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                var filter = CreateFilter(parameterId, comparison, value);
                if (filter != null)
                    filters.Add(filter);
            }

            if (filters.Count > 0)
            {
                var filter = new LogicalOrFilter(filters);
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector using a logical OR of several values for a single parameter.
        /// </summary>
        public static FilteredElementCollector WhereOr(
            this FilteredElementCollector collector,
            ElementId parameterId,
            StringComparison comparison,
            System.Collections.Generic.IEnumerable<string> values)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            if (values == null) throw new ArgumentNullException(nameof(values));

            var filters = new System.Collections.Generic.List<ElementFilter>();
            foreach (var value in values)
            {
                if (value == null) throw new ArgumentNullException(nameof(values));
                var filter = CreateFilter(parameterId, comparison, value);
                if (filter != null)
                    filters.Add(filter);
            }

            if (filters.Count > 0)
            {
                var filter = new LogicalOrFilter(filters);
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector using a logical AND of several parameter comparisons.
        /// </summary>
        public static FilteredElementCollector WhereAnd(
            this FilteredElementCollector collector,
            params (ElementId parameterId, StringComparison comparison, string value)[] conditions)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (conditions == null) throw new ArgumentNullException(nameof(conditions));

            var filters = new System.Collections.Generic.List<ElementFilter>();
            foreach (var (parameterId, comparison, value) in conditions)
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                var filter = CreateFilter(parameterId, comparison, value);
                if (filter != null)
                    filters.Add(filter);
            }

            if (filters.Count > 0)
            {
                var filter = new LogicalAndFilter(filters);
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector using a logical AND of several values for a single parameter.
        /// </summary>
        public static FilteredElementCollector WhereAnd(
            this FilteredElementCollector collector,
            ElementId parameterId,
            StringComparison comparison,
            System.Collections.Generic.IEnumerable<string> values)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            if (values == null) throw new ArgumentNullException(nameof(values));

            var filters = new System.Collections.Generic.List<ElementFilter>();
            foreach (var value in values)
            {
                if (value == null) throw new ArgumentNullException(nameof(values));
                var filter = CreateFilter(parameterId, comparison, value);
                if (filter != null)
                    filters.Add(filter);
            }

            if (filters.Count > 0)
            {
                var filter = new LogicalAndFilter(filters);
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector using a logical OR of several integer values for a single parameter.
        /// </summary>
        public static FilteredElementCollector WhereOr(
            this FilteredElementCollector collector,
            ElementId parameterId,
            Comparison comparison,
            System.Collections.Generic.IEnumerable<int> values)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            if (values == null) throw new ArgumentNullException(nameof(values));

            var filters = new System.Collections.Generic.List<ElementFilter>();
            foreach (var value in values)
            {
                var filter = CreateFilter(parameterId, comparison, value);
                if (filter != null)
                    filters.Add(filter);
            }

            if (filters.Count > 0)
            {
                var filter = new LogicalOrFilter(filters);
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector using a logical AND of several integer values for a single parameter.
        /// </summary>
        public static FilteredElementCollector WhereAnd(
            this FilteredElementCollector collector,
            ElementId parameterId,
            Comparison comparison,
            System.Collections.Generic.IEnumerable<int> values)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            if (values == null) throw new ArgumentNullException(nameof(values));

            var filters = new System.Collections.Generic.List<ElementFilter>();
            foreach (var value in values)
            {
                var filter = CreateFilter(parameterId, comparison, value);
                if (filter != null)
                    filters.Add(filter);
            }

            if (filters.Count > 0)
            {
                var filter = new LogicalAndFilter(filters);
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector using a logical OR of several double values for a single parameter.
        /// </summary>
        public static FilteredElementCollector WhereOr(
            this FilteredElementCollector collector,
            ElementId parameterId,
            Comparison comparison,
            System.Collections.Generic.IEnumerable<double> values)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            if (values == null) throw new ArgumentNullException(nameof(values));

            var filters = new System.Collections.Generic.List<ElementFilter>();
            foreach (var value in values)
            {
                var filter = CreateFilter(parameterId, comparison, value);
                if (filter != null)
                    filters.Add(filter);
            }

            if (filters.Count > 0)
            {
                var filter = new LogicalOrFilter(filters);
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector using a logical AND of several double values for a single parameter.
        /// </summary>
        public static FilteredElementCollector WhereAnd(
            this FilteredElementCollector collector,
            ElementId parameterId,
            Comparison comparison,
            System.Collections.Generic.IEnumerable<double> values)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            if (values == null) throw new ArgumentNullException(nameof(values));

            var filters = new System.Collections.Generic.List<ElementFilter>();
            foreach (var value in values)
            {
                var filter = CreateFilter(parameterId, comparison, value);
                if (filter != null)
                    filters.Add(filter);
            }

            if (filters.Count > 0)
            {
                var filter = new LogicalAndFilter(filters);
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector using a logical OR of several element id values for a single parameter.
        /// </summary>
        public static FilteredElementCollector WhereOr(
            this FilteredElementCollector collector,
            ElementId parameterId,
            Comparison comparison,
            System.Collections.Generic.IEnumerable<ElementId> values)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            if (values == null) throw new ArgumentNullException(nameof(values));

            var filters = new System.Collections.Generic.List<ElementFilter>();
            foreach (var value in values)
            {
                if (value == null) throw new ArgumentNullException(nameof(values));
                var filter = CreateFilter(parameterId, comparison, value);
                if (filter != null)
                    filters.Add(filter);
            }

            if (filters.Count > 0)
            {
                var filter = new LogicalOrFilter(filters);
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Filters the collector using a logical AND of several element id values for a single parameter.
        /// </summary>
        public static FilteredElementCollector WhereAnd(
            this FilteredElementCollector collector,
            ElementId parameterId,
            Comparison comparison,
            System.Collections.Generic.IEnumerable<ElementId> values)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            if (values == null) throw new ArgumentNullException(nameof(values));

            var filters = new System.Collections.Generic.List<ElementFilter>();
            foreach (var value in values)
            {
                if (value == null) throw new ArgumentNullException(nameof(values));
                var filter = CreateFilter(parameterId, comparison, value);
                if (filter != null)
                    filters.Add(filter);
            }

            if (filters.Count > 0)
            {
                var filter = new LogicalAndFilter(filters);
                collector.WherePasses(filter);
            }

            return collector;
        }

        private static ElementParameterFilter? CreateFilter(ElementId parameterId, StringComparison comparison, string value)
            => ParameterFilterRuleBuilder.CreateFilter(parameterId, comparison, value);

        private static ElementParameterFilter? CreateFilter(ElementId parameterId, Comparison comparison, int value)
            => ParameterFilterRuleBuilder.CreateFilter(parameterId, comparison, value);

        private static ElementParameterFilter? CreateFilter(ElementId parameterId, Comparison comparison, double value)
            => ParameterFilterRuleBuilder.CreateFilter(parameterId, comparison, value);

        private static ElementParameterFilter? CreateFilter(ElementId parameterId, Comparison comparison, ElementId value)
            => ParameterFilterRuleBuilder.CreateFilter(parameterId, comparison, value);

        /// <summary>
        /// Applies a complex set of parameter filters built from nested
        /// <see cref="ParameterFilterSet"/> instances.
        /// </summary>
        /// <param name="collector">The collector to filter.</param>
        /// <param name="filterSet">The filter set to apply.</param>
        /// <returns>The same collector instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="collector"/> or <paramref name="filterSet"/> is null.
        /// </exception>
        public static FilteredElementCollector WherePasses(
            this FilteredElementCollector collector,
            ParameterFilterSet filterSet)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (filterSet == null) throw new ArgumentNullException(nameof(filterSet));

            var filter = filterSet.ToFilter();
            if (filter != null)
            {
                collector.WherePasses(filter);
            }

            return collector;
        }

        /// <summary>
        /// Builds a <see cref="ParameterFilterSet"/> using a builder callback
        /// and applies it to the collector.
        /// </summary>
        /// <param name="collector">The collector to filter.</param>
        /// <param name="configure">Callback used to add rules to a builder.</param>
        /// <returns>The same collector instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="collector"/> or <paramref name="configure"/> is null.
        /// </exception>
        public static FilteredElementCollector Where(
            this FilteredElementCollector collector,
            System.Func<ParameterFilterSetBuilder, ParameterFilterSetBuilder> configure)
        {
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var builder = new ParameterFilterSetBuilder();
            var result = configure(builder) ?? builder;
            var set = result.Build();
            return collector.WherePasses(set);
        }

        // parameter lookup helpers removed for simplicity



    }
}

