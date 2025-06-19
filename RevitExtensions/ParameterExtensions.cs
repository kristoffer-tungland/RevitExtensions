using System;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods related to Revit parameters.
    /// </summary>
    public static class ParameterExtensions
    {
        /// <summary>
        /// Gets a parameter from the element or its type using a flexible identifier.
        /// </summary>
        /// <param name="element">The element to search.</param>
        /// <param name="identifier">The parameter identifier string.</param>
        /// <returns>The found parameter or null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
        public static Parameter GetParameter(this Element element, string identifier)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            Parameter parameter = null;

            if (int.TryParse(identifier, out var intValue))
            {
                if (intValue < 0)
                {
                    var bip = (BuiltInParameter)intValue;
                    parameter = element.get_Parameter(bip);
                    if (parameter == null)
                    {
                        using var type = element.GetElementType();
                        parameter = type?.get_Parameter(bip);
                    }
                    return parameter;
                }
                else
                {
                    long idValue = intValue;
                    foreach (Parameter p in element.Parameters)
                    {
                        if (p.Id != null && p.Id.GetElementIdValue() == idValue)
                            return p;
                    }
                    using var typeElem = element.GetElementType();
                    if (typeElem != null)
                    {
                        foreach (Parameter p in typeElem.Parameters)
                        {
                            if (p.Id != null && p.Id.GetElementIdValue() == idValue)
                                return p;
                        }
                    }
                    return null;
                }
            }

            if (Guid.TryParse(identifier, out var guid))
            {
                parameter = element.get_Parameter(guid);
                if (parameter == null)
                {
                    using var type = element.GetElementType();
                    parameter = type?.get_Parameter(guid);
                }
                return parameter;
            }

            parameter = element.LookupParameter(identifier);
            if (parameter != null) return parameter;
            using var typeElement = element.GetElementType();
            return typeElement?.LookupParameter(identifier);
        }
    }
}
