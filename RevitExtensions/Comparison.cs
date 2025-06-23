using System;

namespace RevitExtensions
{
    /// <summary>
    /// Comparison options for numeric and element id parameters.
    /// </summary>
    public enum Comparison
    {
        Equals = 0,
        NotEquals = 1,
        Greater = 2,
        GreaterOrEqual = 3,
        Less = 4,
        LessOrEqual = 5,
    }

    /// <summary>
    /// Comparison options for string parameters.
    /// </summary>
    public enum StringComparison
    {
        Equals = Comparison.Equals,
        NotEquals = Comparison.NotEquals,
        Contains = 100,
        NotContains = 101,
        BeginsWith = 102,
        NotBeginsWith = 103,
        EndsWith = 104,
        NotEndsWith = 105,
        /// <summary>
        /// Matches a value using '*' wildcard segments.
        /// </summary>
        Wildcard = 200,
        Greater = Comparison.Greater,
        GreaterOrEqual = Comparison.GreaterOrEqual,
        Less = Comparison.Less,
        LessOrEqual = Comparison.LessOrEqual,
    }
}
