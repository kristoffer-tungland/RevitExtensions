using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitExtensions.Models
{
    /// <summary>
    /// Information about a parameter available in the document.
    /// </summary>
    public class ParameterMetadata
    {
        /// <summary>
        /// Gets or sets the identifier for the parameter.
        /// </summary>
        public ParameterIdentifier Identifier { get; set; } = new ParameterIdentifier();

        /// <summary>
        /// Gets or sets a value indicating whether the parameter comes from an instance
        /// (<c>true</c>) or from the element type (<c>false</c>).
        /// </summary>
        public bool IsInstance { get; set; }

        /// <summary>
        /// Gets the categories the parameter applies to.
        /// </summary>
        public HashSet<BuiltInCategory> Categories { get; } = new HashSet<BuiltInCategory>();
    }
}
