using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Information about a parameter available in the document.
    /// </summary>
    public class ParameterMetadata
    {
        /// <summary>
        /// Gets or sets the identifier for the built-in parameter.
        /// The identifier stores both the enumeration value and the parameter name.
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

    /// <summary>
    /// Collects and caches built-in parameters available in a document.
    /// </summary>
    public static class BuiltInParameterCollector
    {
        private static IReadOnlyDictionary<ParameterIdentifier, ParameterMetadata>? _cache;

        private static string GetCachePath(Document doc)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));

            var version = 0;
            try
            {
                var number = doc.Application?.VersionNumber;
                if (!string.IsNullOrEmpty(number) && int.TryParse(number, out var v))
                    version = v;
            }
            catch
            {
                // ignore failure, fall back to zero
            }

            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Autodesk",
                $"RVT {version}",
                "BuiltInParameters.json");
        }

        /// <summary>
        /// Gets the cached built-in parameter data, collecting it from the
        /// document if necessary.
        /// </summary>
        /// <param name="doc">The document used to collect data if no cache exists.</param>
        /// <returns>The cached built-in parameters.</returns>
        public static IReadOnlyDictionary<ParameterIdentifier, ParameterMetadata> GetParameters(Document doc)
        {
            if (_cache != null)
                return _cache;

            var cachePath = GetCachePath(doc);

            if (File.Exists(cachePath))
            {
                try
                {
                    var json = File.ReadAllText(cachePath);
                    var list = JsonSerializer.Deserialize<List<SerializableInfo>>(json);
                    if (list != null)
                    {
                        var loaded = new Dictionary<ParameterIdentifier, ParameterMetadata>(new ParameterIdentifierComparer());
                        foreach (var i in list)
                        {
                            var identifier = new ParameterIdentifier
                            {
                                BuiltInParameter = i.BuiltInParameter,
                                Name = i.Name
                            };
                            var info = new ParameterMetadata
                            {
                                Identifier = identifier,
                                IsInstance = i.IsInstance
                            };
                            foreach (var bic in i.Categories)
                                info.Categories.Add(bic);
                            loaded[identifier] = info;
                        }
                        _cache = loaded;
                        return _cache;
                    }
                }
                catch
                {
                    // ignore corrupt cache
                }
            }

            var dict = Collect(doc);
            _cache = dict;

            try
            {
                var list = new List<SerializableInfo>();
                foreach (var v in dict.Values)
                {
                    list.Add(new SerializableInfo
                    {
                        BuiltInParameter = v.Identifier.BuiltInParameter.GetValueOrDefault(),
                        Name = v.Identifier.Name ?? string.Empty,
                        IsInstance = v.IsInstance,
                        Categories = v.Categories.ToArray()
                    });
                }
                Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
                File.WriteAllText(cachePath, JsonSerializer.Serialize(list));
            }
            catch
            {
                // ignore write failures
            }

            return _cache;
        }

        internal static Dictionary<ParameterIdentifier, ParameterMetadata> Collect(Document doc)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));

            var result = new Dictionary<ParameterIdentifier, ParameterMetadata>(new ParameterIdentifierComparer());
            var categories = doc.Settings.Categories;

            if (doc.IsModifiable)
            {
                using var sub = new SubTransaction(doc);
                sub.Start();
                CollectFromAllCategories(doc, categories, result);
                sub.RollBack();
            }
            else
            {
                using var tx = new Transaction(doc, "Collect BuiltInParameters");
                tx.Start();
                CollectFromAllCategories(doc, categories, result);
                tx.RollBack();
            }

            return result;
        }

        private static void CollectFromAllCategories(Document doc, Categories categories, Dictionary<ParameterIdentifier, ParameterMetadata> result)
        {
            foreach (Category cat in categories)
            {
                if (cat.Id.GetElementIdValue() < 0)
                    continue;

                var element = TryFindElement(doc, cat);
                if (element == null)
                    continue;

                CollectParameters(element.Parameters, cat, true, result);

                var typeElement = element.GetElementType();
                if (typeElement != null)
                    CollectParameters(typeElement.Parameters, cat, false, result);
            }
        }

        private static void CollectParameters(ParameterSet set, Category cat, bool isInstance,
            Dictionary<ParameterIdentifier, ParameterMetadata> result)
        {
            foreach (Parameter p in set)
            {
                var val = p.Id.GetElementIdValue();
                if (val < 0)
                {
                    var bip = (BuiltInParameter)val;
                    var identifier = new ParameterIdentifier
                    {
                        BuiltInParameter = bip,
                        Name = p.Definition?.Name
                    };
                    if (!result.TryGetValue(identifier, out var info))
                    {
                        info = new ParameterMetadata
                        {
                            Identifier = identifier,
                            IsInstance = isInstance
                        };
                        result[identifier] = info;
                    }

                    info.Categories.Add((BuiltInCategory)cat.Id.GetElementIdValue());
                }
            }
        }

        private static Element? TryFindElement(Document doc, Category cat)
        {
            return new FilteredElementCollector(doc)
                .WherePasses(new ElementCategoryFilter((BuiltInCategory)cat.Id.GetElementIdValue()))
                .WhereElementIsNotElementType()
                .FirstOrDefault();
        }

        private class SerializableInfo
        {
            public BuiltInParameter BuiltInParameter { get; set; }
            public string Name { get; set; } = string.Empty;
            public bool IsInstance { get; set; }
            public BuiltInCategory[] Categories { get; set; } = Array.Empty<BuiltInCategory>();
        }
    }
}
