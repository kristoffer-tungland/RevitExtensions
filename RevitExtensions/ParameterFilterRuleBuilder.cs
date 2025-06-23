using Autodesk.Revit.DB;

namespace RevitExtensions
{
    internal static class ParameterFilterRuleBuilder
    {
        public static FilterRule? CreateRule(ElementId parameterId, StringComparison comparison, string value)
        {
#if REVIT2022_OR_LESS
            return comparison switch
            {
                StringComparison.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameterId, value, false),
                StringComparison.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameterId, value, false),
                StringComparison.Contains => ParameterFilterRuleFactory.CreateContainsRule(parameterId, value, false),
                StringComparison.NotContains => ParameterFilterRuleFactory.CreateNotContainsRule(parameterId, value, false),
                StringComparison.BeginsWith => ParameterFilterRuleFactory.CreateBeginsWithRule(parameterId, value, false),
                StringComparison.NotBeginsWith => ParameterFilterRuleFactory.CreateNotBeginsWithRule(parameterId, value, false),
                StringComparison.EndsWith => ParameterFilterRuleFactory.CreateEndsWithRule(parameterId, value, false),
                StringComparison.NotEndsWith => ParameterFilterRuleFactory.CreateNotEndsWithRule(parameterId, value, false),
                StringComparison.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameterId, value, false),
                StringComparison.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameterId, value, false),
                StringComparison.Less => ParameterFilterRuleFactory.CreateLessRule(parameterId, value, false),
                StringComparison.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameterId, value, false),
                _ => null,
            };
#else
            return comparison switch
            {
                StringComparison.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameterId, value),
                StringComparison.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameterId, value),
                StringComparison.Contains => ParameterFilterRuleFactory.CreateContainsRule(parameterId, value),
                StringComparison.NotContains => ParameterFilterRuleFactory.CreateNotContainsRule(parameterId, value),
                StringComparison.BeginsWith => ParameterFilterRuleFactory.CreateBeginsWithRule(parameterId, value),
                StringComparison.NotBeginsWith => ParameterFilterRuleFactory.CreateNotBeginsWithRule(parameterId, value),
                StringComparison.EndsWith => ParameterFilterRuleFactory.CreateEndsWithRule(parameterId, value),
                StringComparison.NotEndsWith => ParameterFilterRuleFactory.CreateNotEndsWithRule(parameterId, value),
                StringComparison.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameterId, value),
                StringComparison.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameterId, value),
                StringComparison.Less => ParameterFilterRuleFactory.CreateLessRule(parameterId, value),
                StringComparison.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameterId, value),
                _ => null,
            };
#endif
        }

        public static System.Collections.Generic.IEnumerable<FilterRule> CreateRules(ElementId parameterId, StringComparison comparison, string value)
        {
            if (comparison == StringComparison.Wildcard)
                return CreateWildcardRules(parameterId, value);
            if (comparison == StringComparison.Equals && value.IndexOf('*') >= 0)
                return CreateWildcardRules(parameterId, value);

            var rule = CreateRule(parameterId, comparison, value);
            return rule == null
                ? System.Array.Empty<FilterRule>()
                : new[] { rule };
        }

        private static System.Collections.Generic.IEnumerable<FilterRule> CreateWildcardRules(ElementId parameterId, string value)
        {
            var parts = value.Split('*');
            var rules = new System.Collections.Generic.List<FilterRule>();

#if REVIT2022_OR_LESS
            if (parts.Length > 0 && parts[0].Length > 0)
                rules.Add(ParameterFilterRuleFactory.CreateBeginsWithRule(parameterId, parts[0], false));
            for (int i = 1; i < parts.Length - 1; i++)
                if (parts[i].Length > 0)
                    rules.Add(ParameterFilterRuleFactory.CreateContainsRule(parameterId, parts[i], false));
            if (parts.Length > 1 && parts[parts.Length - 1].Length > 0)
                rules.Add(ParameterFilterRuleFactory.CreateEndsWithRule(parameterId, parts[parts.Length - 1], false));
#else
            if (parts.Length > 0 && parts[0].Length > 0)
                rules.Add(ParameterFilterRuleFactory.CreateBeginsWithRule(parameterId, parts[0]));
            for (int i = 1; i < parts.Length - 1; i++)
                if (parts[i].Length > 0)
                    rules.Add(ParameterFilterRuleFactory.CreateContainsRule(parameterId, parts[i]));
            if (parts.Length > 1 && parts[parts.Length - 1].Length > 0)
                rules.Add(ParameterFilterRuleFactory.CreateEndsWithRule(parameterId, parts[parts.Length - 1]));
#endif

            return rules;
        }

        public static FilterRule? CreateRule(ElementId parameterId, Comparison comparison, int value)
        {
#if REVIT2022_OR_LESS
            return comparison switch
            {
                Comparison.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameterId, value),
                Comparison.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameterId, value),
                Comparison.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameterId, value),
                Comparison.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameterId, value),
                Comparison.Less => ParameterFilterRuleFactory.CreateLessRule(parameterId, value),
                Comparison.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameterId, value),
                _ => null,
            };
#else
            return comparison switch
            {
                Comparison.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameterId, value),
                Comparison.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameterId, value),
                Comparison.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameterId, value),
                Comparison.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameterId, value),
                Comparison.Less => ParameterFilterRuleFactory.CreateLessRule(parameterId, value),
                Comparison.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameterId, value),
                _ => null,
            };
#endif
        }

        public static FilterRule? CreateRule(ElementId parameterId, Comparison comparison, double value)
        {
#if REVIT2022_OR_LESS
            return comparison switch
            {
                Comparison.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameterId, value, 0),
                Comparison.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameterId, value, 0),
                Comparison.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameterId, value, 0),
                Comparison.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameterId, value, 0),
                Comparison.Less => ParameterFilterRuleFactory.CreateLessRule(parameterId, value, 0),
                Comparison.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameterId, value, 0),
                _ => null,
            };
#else
            return comparison switch
            {
                Comparison.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameterId, value, 0),
                Comparison.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameterId, value, 0),
                Comparison.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameterId, value, 0),
                Comparison.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameterId, value, 0),
                Comparison.Less => ParameterFilterRuleFactory.CreateLessRule(parameterId, value, 0),
                Comparison.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameterId, value, 0),
                _ => null,
            };
#endif
        }

        public static FilterRule? CreateRule(ElementId parameterId, Comparison comparison, ElementId value)
        {
#if REVIT2022_OR_LESS
            return comparison switch
            {
                Comparison.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameterId, value),
                Comparison.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameterId, value),
                _ => null,
            };
#else
            return comparison switch
            {
                Comparison.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameterId, value),
                Comparison.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameterId, value),
                _ => null,
            };
#endif
        }

        public static ElementParameterFilter? CreateFilter(ElementId parameterId, StringComparison comparison, string value)
        {
            var rules = CreateRules(parameterId, comparison, value);
            var list = new System.Collections.Generic.List<FilterRule>(rules);
            if (list.Count == 0)
                return null;
            return new ElementParameterFilter(list);
        }

        public static ElementParameterFilter? CreateFilter(ElementId parameterId, Comparison comparison, int value)
        {
            var rule = CreateRule(parameterId, comparison, value);
            return rule == null ? null : new ElementParameterFilter(rule);
        }

        public static ElementParameterFilter? CreateFilter(ElementId parameterId, Comparison comparison, double value)
        {
            var rule = CreateRule(parameterId, comparison, value);
            return rule == null ? null : new ElementParameterFilter(rule);
        }

        public static ElementParameterFilter? CreateFilter(ElementId parameterId, Comparison comparison, ElementId value)
        {
            var rule = CreateRule(parameterId, comparison, value);
            return rule == null ? null : new ElementParameterFilter(rule);
        }
    }
}
