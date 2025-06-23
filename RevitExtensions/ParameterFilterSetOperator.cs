namespace RevitExtensions
{
    /// <summary>
    /// Specifies how filter rules and nested sets are logically combined.
    /// </summary>
    public enum ParameterFilterSetOperator
    {
        /// <summary>
        /// Combine using logical AND.
        /// </summary>
        And,

        /// <summary>
        /// Combine using logical OR.
        /// </summary>
        Or,
    }
}

