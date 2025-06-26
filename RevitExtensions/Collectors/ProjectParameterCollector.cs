using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitExtensions.Models;
using RevitExtensions.Utilities;

namespace RevitExtensions.Collectors
{
    /// <summary>
    /// Collects project and shared parameters available in a document.
    /// </summary>
    internal static class ProjectParameterCollector
    {
        /// <summary>
        /// Gets all project and shared parameters in the given document.
        /// </summary>
        /// <param name="document">The document to inspect.</param>
        /// <returns>A dictionary keyed by parameter identifier.</returns>
        public static IReadOnlyDictionary<ParameterIdentifier, ParameterMetadata> GetParameters(Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            var collector = new FilteredElementCollector(document).OfClass(typeof(ParameterElement));
            var dict = new Dictionary<ParameterIdentifier, ParameterMetadata>(new ParameterIdentifierComparer());

            foreach (ParameterElement pe in collector)
            {
                var def = pe.GetDefinitionSafe();
                var name = def?.Name;
                var guid = pe.GetGuidSafe();

                var identifier = new ParameterIdentifier
                {
                    Guid = guid,
                    Name = name,
                    Id = guid == null ? pe.Id.GetElementIdValue() : (long?)null
                };

                if (!dict.TryGetValue(identifier, out var info))
                {
                    info = new ParameterMetadata
                    {
                        Identifier = identifier,
                        IsInstance = pe.GetIsInstanceSafe(document)
                    };
                    dict[identifier] = info;
                }

                foreach (var bic in pe.GetCategoriesSafe(document))
                    info.Categories.Add(bic);
            }

            return dict;
        }
    }
}
