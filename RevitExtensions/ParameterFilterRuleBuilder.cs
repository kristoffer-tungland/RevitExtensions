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
            var rule = CreateRule(parameterId, comparison, value);
            return rule == null ? null : new ElementParameterFilter(rule);
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
