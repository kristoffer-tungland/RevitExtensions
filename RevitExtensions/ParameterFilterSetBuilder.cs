using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Builder for <see cref="ParameterFilterSet"/> instances using the same
    /// fluent style as the collector extension methods.
    /// </summary>
    public sealed class ParameterFilterSetBuilder
    {
        private readonly ParameterFilterSet _set;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterFilterSetBuilder"/> class.
        /// </summary>
        /// <param name="operator">Specifies how the resulting set combines its contents. Defaults to <see cref="ParameterFilterSetOperator.And"/>.</param>
        public ParameterFilterSetBuilder(ParameterFilterSetOperator @operator = ParameterFilterSetOperator.And)
        {
            _set = new ParameterFilterSet(@operator);
        }

        /// <summary>
        /// Adds a rule to the builder.
        /// </summary>
        public ParameterFilterSetBuilder Rule(ElementId parameterId, StringComparison comparison, string value)
        {
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            if (value == null) throw new ArgumentNullException(nameof(value));
            foreach (var rule in ParameterFilterRuleBuilder.CreateRules(parameterId, comparison, value))
            {
                _set.Rule(rule);
            }
            return this;
        }

        /// <summary>
        /// Adds a rule to the builder.
        /// </summary>
        public ParameterFilterSetBuilder Rule(ElementId parameterId, Comparison comparison, int value)
        {
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            var rule = ParameterFilterRuleBuilder.CreateRule(parameterId, comparison, value);
            if (rule != null)
                _set.Rule(rule);
            return this;
        }

        /// <summary>
        /// Adds a rule to the builder.
        /// </summary>
        public ParameterFilterSetBuilder Rule(ElementId parameterId, Comparison comparison, double value)
        {
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            var rule = ParameterFilterRuleBuilder.CreateRule(parameterId, comparison, value);
            if (rule != null)
                _set.Rule(rule);
            return this;
        }

        /// <summary>
        /// Adds a rule to the builder.
        /// </summary>
        public ParameterFilterSetBuilder Rule(ElementId parameterId, Comparison comparison, ElementId value)
        {
            if (parameterId == null) throw new ArgumentNullException(nameof(parameterId));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var rule = ParameterFilterRuleBuilder.CreateRule(parameterId, comparison, value);
            if (rule != null)
                _set.Rule(rule);
            return this;
        }

        /// <summary>
        /// Adds a nested OR set using the provided conditions.
        /// </summary>
        public ParameterFilterSetBuilder OrSet(params (ElementId parameterId, StringComparison comparison, string value)[] conditions)
        {
            if (conditions == null) throw new ArgumentNullException(nameof(conditions));
            var nested = new ParameterFilterSetBuilder(ParameterFilterSetOperator.Or);
            foreach (var (parameterId, comparison, value) in conditions)
            {
                nested.Rule(parameterId, comparison, value);
            }
            return AddSet(nested);
        }

        /// <summary>
        /// Adds a nested OR set configured via a callback.
        /// </summary>
        public ParameterFilterSetBuilder OrSet(Func<ParameterFilterSetBuilder, ParameterFilterSetBuilder> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var nested = new ParameterFilterSetBuilder(ParameterFilterSetOperator.Or);
            var result = configure(nested) ?? nested;
            return AddSet(result);
        }

        /// <summary>
        /// Adds a nested OR set for a single parameter with multiple values.
        /// </summary>
        public ParameterFilterSetBuilder OrSet(ElementId parameterId, StringComparison comparison, IEnumerable<string> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var nested = new ParameterFilterSetBuilder(ParameterFilterSetOperator.Or);
            foreach (var value in values)
            {
                nested.Rule(parameterId, comparison, value);
            }
            return AddSet(nested);
        }

        /// <summary>
        /// Adds a nested OR set for integer values.
        /// </summary>
        public ParameterFilterSetBuilder OrSet(ElementId parameterId, Comparison comparison, IEnumerable<int> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var nested = new ParameterFilterSetBuilder(ParameterFilterSetOperator.Or);
            foreach (var value in values)
            {
                nested.Rule(parameterId, comparison, value);
            }
            return AddSet(nested);
        }

        /// <summary>
        /// Adds a nested OR set for double values.
        /// </summary>
        public ParameterFilterSetBuilder OrSet(ElementId parameterId, Comparison comparison, IEnumerable<double> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var nested = new ParameterFilterSetBuilder(ParameterFilterSetOperator.Or);
            foreach (var value in values)
            {
                nested.Rule(parameterId, comparison, value);
            }
            return AddSet(nested);
        }

        /// <summary>
        /// Adds a nested OR set for element id values.
        /// </summary>
        public ParameterFilterSetBuilder OrSet(ElementId parameterId, Comparison comparison, IEnumerable<ElementId> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var nested = new ParameterFilterSetBuilder(ParameterFilterSetOperator.Or);
            foreach (var value in values)
            {
                nested.Rule(parameterId, comparison, value);
            }
            return AddSet(nested);
        }

        /// <summary>
        /// Adds a nested AND set using the provided conditions.
        /// </summary>
        public ParameterFilterSetBuilder AndSet(params (ElementId parameterId, StringComparison comparison, string value)[] conditions)
        {
            if (conditions == null) throw new ArgumentNullException(nameof(conditions));
            var nested = new ParameterFilterSetBuilder(ParameterFilterSetOperator.And);
            foreach (var (parameterId, comparison, value) in conditions)
            {
                nested.Rule(parameterId, comparison, value);
            }
            return AddSet(nested);
        }

        /// <summary>
        /// Adds a nested AND set configured via a callback.
        /// </summary>
        public ParameterFilterSetBuilder AndSet(Func<ParameterFilterSetBuilder, ParameterFilterSetBuilder> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var nested = new ParameterFilterSetBuilder(ParameterFilterSetOperator.And);
            var result = configure(nested) ?? nested;
            return AddSet(result);
        }

        /// <summary>
        /// Adds a nested AND set for a single parameter with multiple values.
        /// </summary>
        public ParameterFilterSetBuilder AndSet(ElementId parameterId, StringComparison comparison, IEnumerable<string> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var nested = new ParameterFilterSetBuilder(ParameterFilterSetOperator.And);
            foreach (var value in values)
            {
                nested.Rule(parameterId, comparison, value);
            }
            return AddSet(nested);
        }

        /// <summary>
        /// Adds a nested AND set for integer values.
        /// </summary>
        public ParameterFilterSetBuilder AndSet(ElementId parameterId, Comparison comparison, IEnumerable<int> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var nested = new ParameterFilterSetBuilder(ParameterFilterSetOperator.And);
            foreach (var value in values)
            {
                nested.Rule(parameterId, comparison, value);
            }
            return AddSet(nested);
        }

        /// <summary>
        /// Adds a nested AND set for double values.
        /// </summary>
        public ParameterFilterSetBuilder AndSet(ElementId parameterId, Comparison comparison, IEnumerable<double> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var nested = new ParameterFilterSetBuilder(ParameterFilterSetOperator.And);
            foreach (var value in values)
            {
                nested.Rule(parameterId, comparison, value);
            }
            return AddSet(nested);
        }

        /// <summary>
        /// Adds a nested AND set for element id values.
        /// </summary>
        public ParameterFilterSetBuilder AndSet(ElementId parameterId, Comparison comparison, IEnumerable<ElementId> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var nested = new ParameterFilterSetBuilder(ParameterFilterSetOperator.And);
            foreach (var value in values)
            {
                nested.Rule(parameterId, comparison, value);
            }
            return AddSet(nested);
        }

        /// <summary>
        /// Adds a pre-built nested set to this builder.
        /// </summary>
        public ParameterFilterSetBuilder AddSet(ParameterFilterSetBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            _set.AddSet(builder.Build());
            return this;
        }

        /// <summary>
        /// Finalizes the builder and returns the constructed <see cref="ParameterFilterSet"/>.
        /// </summary>
        public ParameterFilterSet Build() => _set;
    }
}
