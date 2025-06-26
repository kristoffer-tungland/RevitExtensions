using System;
using Autodesk.Revit.DB;
using RevitExtensions.Models;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for <see cref="ParameterIdentifier"/>.
    /// </summary>
    internal static class ParameterIdentifierExtensions
    {
        /// <summary>
        /// Converts a <see cref="ParameterIdentifier"/> to an <see cref="ElementId"/> if possible.
        /// </summary>
        /// <param name="identifier">The identifier to convert.</param>
        /// <returns>The element id if represented by the identifier; otherwise <c>null</c>.</returns>
        public static ElementId? ToElementId(this ParameterIdentifier identifier)
        {
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            if (identifier.BuiltInParameter.HasValue)
                return identifier.BuiltInParameter.Value.ToElementId();
            if (identifier.Id.HasValue)
                return new ElementId((int)identifier.Id.Value);
            return null;
        }
    }
}
