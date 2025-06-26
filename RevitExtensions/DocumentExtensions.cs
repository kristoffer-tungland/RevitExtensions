using System;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using RevitExtensions.Collectors;
using RevitExtensions.Models;
using RevitExtensions.Utilities;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for <see cref="Document"/>.
    /// </summary>
    public static class DocumentExtensions
    {
        /// <summary>
        /// Creates a collector for all element instances in the document.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <returns>A collector that excludes element types.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static FilteredElementCollector Instances(this Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return new FilteredElementCollector(document)
                .WhereElementIsNotElementType();
        }

        /// <summary>
        /// Creates a collector for all element types in the document.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <returns>A collector that only includes element types.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static FilteredElementCollector Types(this Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return new FilteredElementCollector(document)
                .WhereElementIsElementType();
        }

        /// <summary>
        /// Creates a collector for elements of the specified type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="document">The document to search.</param>
        /// <returns>A filtered element collector for instances of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static FilteredElementCollector InstancesOf<T>(this Document document) where T : Element
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return new FilteredElementCollector(document)
                .InstancesOf<T>();
        }

        /// <summary>
        /// Creates a collector for element types of the specified type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="document">The document to search.</param>
        /// <returns>A filtered element collector for types of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static FilteredElementCollector TypesOf<T>(this Document document) where T : Element
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return new FilteredElementCollector(document)
                .TypesOf<T>();
        }

        /// <summary>
        /// Creates a collector for elements in the specified built-in category.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="category">The built-in category to filter by.</param>
        /// <returns>A collector filtered by the given category.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static FilteredElementCollector InstancesOf(this Document document, BuiltInCategory category)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return new FilteredElementCollector(document)
                .InstancesOf(category);
        }

        /// <summary>
        /// Creates a collector for element types in the specified built-in category.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="category">The built-in category to filter by.</param>
        /// <returns>A collector filtered by the given category.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static FilteredElementCollector TypesOf(this Document document, BuiltInCategory category)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return new FilteredElementCollector(document)
                .TypesOf(category);
        }

        /// <summary>
        /// Creates a collector for elements in the specified built-in categories.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="categories">The built-in categories to filter by.</param>
        /// <returns>A collector filtered by the given categories.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> or <paramref name="categories"/> is null.</exception>
        public static FilteredElementCollector InstancesOf(this Document document, System.Collections.Generic.IEnumerable<BuiltInCategory> categories)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (categories == null) throw new ArgumentNullException(nameof(categories));

            return new FilteredElementCollector(document)
                .InstancesOf(categories);
        }

        /// <summary>
        /// Creates a collector for element types in the specified built-in categories.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="categories">The built-in categories to filter by.</param>
        /// <returns>A collector filtered by the given categories.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> or <paramref name="categories"/> is null.</exception>
        public static FilteredElementCollector TypesOf(this Document document, System.Collections.Generic.IEnumerable<BuiltInCategory> categories)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (categories == null) throw new ArgumentNullException(nameof(categories));

            return new FilteredElementCollector(document)
                .TypesOf(categories);
        }

        /// <summary>
        /// Gets an element by id.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="id">The element id.</param>
        /// <returns>The element with the given id, or <c>null</c> if none exists.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> or <paramref name="id"/> is null.</exception>
        public static Element? GetElement(this Document document, ElementId id)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (id == null) throw new ArgumentNullException(nameof(id));

            return id.ToElement(document);
        }

        /// <summary>
        /// Gets an element by integer id.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="id">The element id.</param>
        /// <returns>The element with the given id, or <c>null</c> if none exists.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static Element? GetElement(this Document document, int id)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return id.ToElementId().ToElement(document);
        }

        /// <summary>
        /// Gets an element by long id.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="id">The element id.</param>
        /// <returns>The element with the given id, or <c>null</c> if none exists.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static Element? GetElement(this Document document, long id)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return id.ToElementId().ToElement(document);
        }

        /// <summary>
        /// Gets an element of the specified type by id.
        /// </summary>
        /// <typeparam name="T">The expected element type.</typeparam>
        /// <param name="document">The document to search.</param>
        /// <param name="id">The element id.</param>
        /// <returns>The element cast to <typeparamref name="T"/> if found; otherwise null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> or <paramref name="id"/> is null.</exception>
        public static T? GetElement<T>(this Document document, ElementId id) where T : Element
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (id == null) throw new ArgumentNullException(nameof(id));

            return id.ToElement<T>(document);
        }

        /// <summary>
        /// Gets an element of the specified type by integer id.
        /// </summary>
        /// <typeparam name="T">The expected element type.</typeparam>
        /// <param name="document">The document to search.</param>
        /// <param name="id">The element id.</param>
        /// <returns>The element cast to <typeparamref name="T"/> if found; otherwise null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static T? GetElement<T>(this Document document, int id) where T : Element
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return id.ToElementId().ToElement<T>(document);
        }

        /// <summary>
        /// Gets an element of the specified type by long id.
        /// </summary>
        /// <typeparam name="T">The expected element type.</typeparam>
        /// <param name="document">The document to search.</param>
        /// <param name="id">The element id.</param>
        /// <returns>The element cast to <typeparamref name="T"/> if found; otherwise null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static T? GetElement<T>(this Document document, long id) where T : Element
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return id.ToElementId().ToElement<T>(document);
        }

        /// <summary>
        /// Creates and starts a transaction.
        /// </summary>
        /// <param name="document">The owning document.</param>
        /// <param name="name">The transaction name.</param>
        /// <returns>The started transaction.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="document"/> or <paramref name="name"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">Thrown when the transaction fails to start.</exception>
        public static Transaction StartTransaction(this Document document, string name)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var transaction = new Transaction(document, name);
            var status = transaction.Start();
            if (status != TransactionStatus.Started)
            {
                transaction.Dispose();
                throw new InvalidOperationException("Failed to start transaction.");
            }

            return transaction;
        }

        /// <summary>
        /// Creates and starts a transaction group.
        /// </summary>
        /// <param name="document">The owning document.</param>
        /// <param name="name">The group name.</param>
        /// <returns>The started transaction group.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="document"/> or <paramref name="name"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">Thrown when the transaction group fails to start.</exception>
        public static TransactionGroup StartTransactionGroup(this Document document, string name)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var group = new TransactionGroup(document, name);
            var status = group.Start();
            if (status != TransactionStatus.Started)
            {
                group.Dispose();
                throw new InvalidOperationException("Failed to start transaction group.");
            }

            return group;
        }

        /// <summary>
        /// Creates and starts a subtransaction.
        /// </summary>
        /// <param name="document">The owning document.</param>
        /// <returns>The started subtransaction.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the subtransaction fails to start.</exception>
        public static SubTransaction StartSubTransaction(this Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            var sub = new SubTransaction(document);
            var status = sub.Start();
            if (status != TransactionStatus.Started)
            {
                sub.Dispose();
                throw new InvalidOperationException("Failed to start subtransaction.");
            }

            return sub;
        }

        /// <summary>
        /// Gets the available parameters for all categories in the document. This includes
        /// built-in, project, and shared parameters. Only built-in parameters are cached.
        /// </summary>
        /// <param name="document">The document to inspect.</param>
        /// <returns>A dictionary of parameter metadata keyed by identifier.</returns>
        public static System.Collections.Generic.IReadOnlyDictionary<ParameterIdentifier, ParameterMetadata> GetAvailableParameters(this Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            var result = new System.Collections.Generic.Dictionary<ParameterIdentifier, ParameterMetadata>(new ParameterIdentifierComparer());

            foreach (var kvp in BuiltInParameterCollector.GetParameters(document))
                result[kvp.Key] = kvp.Value;

            foreach (var kvp in ProjectParameterCollector.GetParameters(document))
                result[kvp.Key] = kvp.Value;

            return result;
        }

        /// <summary>
        /// Gets the available built-in parameters for the specified category.
        /// </summary>
        /// <param name="document">The document to inspect.</param>
        /// <param name="category">The built-in category.</param>
        /// <returns>A dictionary of parameter information.</returns>
        public static System.Collections.Generic.IReadOnlyDictionary<ParameterIdentifier, ParameterMetadata> GetAvailableParameters(this Document document, BuiltInCategory category)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            var all = document.GetAvailableParameters();

            var result = new System.Collections.Generic.Dictionary<ParameterIdentifier, ParameterMetadata>(new ParameterIdentifierComparer());
            foreach (var kvp in all)
            {
                if (kvp.Value.Categories.Contains(category))
                    result[kvp.Key] = kvp.Value;
            }
            return result;
        }

        /// <summary>
        /// Gets the available built-in parameters for the specified categories.
        /// </summary>
        /// <param name="document">The document to inspect.</param>
        /// <param name="categories">The built-in categories.</param>
        /// <returns>A dictionary of parameter information.</returns>
        public static System.Collections.Generic.IReadOnlyDictionary<ParameterIdentifier, ParameterMetadata> GetAvailableParameters(this Document document, System.Collections.Generic.IEnumerable<BuiltInCategory> categories)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (categories == null) throw new ArgumentNullException(nameof(categories));

            var set = new System.Collections.Generic.HashSet<BuiltInCategory>(categories);
            var all = document.GetAvailableParameters();

            var result = new System.Collections.Generic.Dictionary<ParameterIdentifier, ParameterMetadata>(new ParameterIdentifierComparer());
            foreach (var kvp in all)
            {
                if (kvp.Value.Categories.Overlaps(set))
                    result[kvp.Key] = kvp.Value;
            }
            return result;
        }

        private static bool Matches(ParameterIdentifier candidate, ParameterIdentifier search)
        {
            if (search.BuiltInParameter.HasValue)
                return candidate.BuiltInParameter == search.BuiltInParameter;
            if (search.Guid.HasValue)
                return candidate.Guid == search.Guid;
            if (search.Id.HasValue)
                return candidate.Id == search.Id;
            if (!string.IsNullOrEmpty(search.Name))
                return candidate.Name == search.Name;
            return false;
        }

        /// <summary>
        /// Looks up a parameter identifier by name, built-in parameter, guid or id.
        /// Returns the first matching identifier or <c>null</c> if none are found.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="identifier">A string representation of the parameter.</param>
        /// <returns>The first matching identifier or null.</returns>
        public static ParameterIdentifier? LookupParameterId(this Document document, string identifier)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            var search = ParameterIdentifier.Parse(identifier);
            return document.LookupParameterId(search);
        }

        /// <summary>
        /// Looks up a parameter identifier by name, built-in parameter, guid or id.
        /// Returns the first matching identifier or <c>null</c> if none are found.
        /// Falls back to a name-based search when no matching identifier is found.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="identifier">The identifier to resolve.</param>
        /// <returns>The first matching identifier or null.</returns>
        public static ParameterIdentifier? LookupParameterId(this Document document, ParameterIdentifier identifier)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            foreach (var kvp in document.GetAvailableParameters())
            {
                if (Matches(kvp.Key, identifier))
                    return kvp.Key;
            }

            if (!string.IsNullOrEmpty(identifier.Name))
            {
                var all = document.GetParametersByName(identifier.Name);
                if (all.Count > 0)
                    return all[0].Identifier;
            }

            return null;
        }

        /// <summary>
        /// Looks up a parameter identifier within the specified category.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="identifier">A string representation of the parameter.</param>
        /// <param name="category">The built-in category to restrict the search to.</param>
        /// <returns>The first matching identifier or null.</returns>
        public static ParameterIdentifier? LookupParameterId(this Document document, string identifier, BuiltInCategory category)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            var search = ParameterIdentifier.Parse(identifier);
            return document.LookupParameterId(search, category);
        }

        /// <summary>
        /// Looks up a parameter identifier within the specified category.
        /// Falls back to a name-based search when no matching identifier is found.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="identifier">The identifier to resolve.</param>
        /// <param name="category">The built-in category to restrict the search to.</param>
        /// <returns>The first matching identifier or null.</returns>
        public static ParameterIdentifier? LookupParameterId(this Document document, ParameterIdentifier identifier, BuiltInCategory category)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            var all = document.GetAvailableParameters(category);

            foreach (var kvp in all)
            {
                if (Matches(kvp.Key, identifier))
                    return kvp.Key;
            }

            if (!string.IsNullOrEmpty(identifier.Name))
            {
                foreach (var kvp in all)
                {
                    if (kvp.Key.Name == identifier.Name)
                        return kvp.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all available parameters with the specified name.
        /// </summary>
        /// <param name="document">The document to search.</param>
        /// <param name="name">The parameter name.</param>
        /// <returns>A list of matching parameter metadata.</returns>
        public static System.Collections.Generic.List<ParameterMetadata> GetParametersByName(this Document document, string name)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var all = document.GetAvailableParameters();
            var result = new System.Collections.Generic.List<ParameterMetadata>();

            foreach (var kvp in all)
            {
                if (kvp.Key.Name == name)
                    result.Add(kvp.Value);
            }

            return result;
        }


    }
}

