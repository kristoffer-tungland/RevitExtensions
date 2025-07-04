namespace RevitExtensions.Models
{
    /// <summary>
    /// Specifies the type of a parameter value.
    /// </summary>
    public enum ParameterValueType
    {
        /// <summary>Textual value.</summary>
        Text,
        /// <summary>Integer value.</summary>
        Integer,
        /// <summary>Numeric value.</summary>
        Number,
        /// <summary>Element identifier.</summary>
        Element,
        /// <summary>Workset identifier.</summary>
        Workset,
    }

    /// <summary>
    /// Represents a value assigned to a parameter.
    /// </summary>
    public class ParameterValueDetailed
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Gets or sets the string representation of the value.
        /// </summary>
        public string? ValueString { get; set; }

        /// <summary>
        /// Gets or sets the value type.
        /// </summary>
        public ParameterValueType ValueType { get; set; }
    }
}
