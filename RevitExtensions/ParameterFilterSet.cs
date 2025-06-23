using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Represents a set of parameter rules combined with either OR or AND logic.
    /// Sets can contain nested sets for complex compositions.
    /// </summary>
    public sealed class ParameterFilterSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterFilterSet"/> class.
        /// </summary>
        /// <param name="operator">Specifies how rules and sets are combined. Defaults to <see cref="ParameterFilterSetOperator.And"/>.</param>
        public ParameterFilterSet(ParameterFilterSetOperator @operator = ParameterFilterSetOperator.And)
        {
            Operator = @operator;
        }

        /// <summary>
        /// Gets the logical operator used to combine the contents of this set.
        /// </summary>
        public ParameterFilterSetOperator Operator { get; }

        /// <summary>
        /// Gets the rules contained in this set.
        /// </summary>
        public List<FilterRule> Rules { get; } = new List<FilterRule>();

        /// <summary>
        /// Gets the nested sets contained in this set.
        /// </summary>
        public List<ParameterFilterSet> Sets { get; } = new List<ParameterFilterSet>();

        /// <summary>
        /// Adds a rule to this set.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        /// <returns>This set instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="rule"/> is null.</exception>
        public ParameterFilterSet AddRule(FilterRule rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            Rules.Add(rule);
            return this;
        }

        /// <summary>
        /// Adds a nested set to this set.
        /// </summary>
        /// <param name="set">The set to add.</param>
        /// <returns>This set instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="set"/> is null.</exception>
        public ParameterFilterSet AddSet(ParameterFilterSet set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));
            Sets.Add(set);
            return this;
        }

        internal ElementFilter? ToFilter()
        {
            var filters = new List<ElementFilter>();
            foreach (var rule in Rules)
            {
                filters.Add(new ElementParameterFilter(rule));
            }

            foreach (var set in Sets)
            {
                var filter = set.ToFilter();
                if (filter != null)
                    filters.Add(filter);
            }

            if (filters.Count == 0)
                return null;

            if (filters.Count == 1)
                return filters[0];

            return Operator == ParameterFilterSetOperator.Or ? (ElementFilter)new LogicalOrFilter(filters) : new LogicalAndFilter(filters);
        }
    }
}
