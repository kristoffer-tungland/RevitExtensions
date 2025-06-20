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

            return element.GetParameter(ParameterIdentifier.Parse(identifier));
        }

        /// <summary>
        /// Gets a parameter from the element or its type using a <see cref="ParameterIdentifier"/>.
        /// </summary>
        /// <param name="element">The element to search.</param>
        /// <param name="identifier">The parameter identifier.</param>
        /// <returns>The found parameter or null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
        public static Parameter GetParameter(this Element element, ParameterIdentifier identifier)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            Parameter parameter = null;

            if (identifier.Guid.HasValue)
            {
                var guid = identifier.Guid.Value;
                foreach (Parameter p in element.Parameters)
                {
                    var pg = TryGetGuid(p);
                    if (pg.HasValue && pg.Value == guid)
                        return p;
                }

                using var type = element.GetElementType();
                if (type != null)
                {
                    foreach (Parameter p in type.Parameters)
                    {
                        var pg = TryGetGuid(p);
                        if (pg.HasValue && pg.Value == guid)
                            return p;
                    }
                }
            }

            if (identifier.BuiltInParameter.HasValue)
            {
                var bip = identifier.BuiltInParameter.Value;
                parameter = element.get_Parameter(bip);
                if (parameter == null)
                {
                    using var type = element.GetElementType();
                    parameter = type?.get_Parameter(bip);
                }
                return parameter;
            }

            if (identifier.Id.HasValue)
            {
                long idValue = identifier.Id.Value;
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
            }

            if (!string.IsNullOrEmpty(identifier.Name))
            {
                parameter = element.LookupParameter(identifier.Name);
                if (parameter != null) return parameter;
                using var typeElement = element.GetElementType();
                return typeElement?.LookupParameter(identifier.Name);
            }

            return null;
        }

        /// <summary>
        /// Retrieves the value stored in the parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <typeparam name="T">The desired return type.</typeparam>
        /// <returns>The parameter value or null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
        public static object GetParameterValue(this Parameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            return parameter.StorageType switch
            {
                StorageType.Double => (object)parameter.AsDouble(),
                StorageType.Integer => parameter.AsInteger(),
                StorageType.String => parameter.AsString(),
                StorageType.ElementId => parameter.AsElementId()?.GetElementIdValue(),
                _ => null,
            };
        }

        /// <summary>
        /// Retrieves the value stored in the parameter and converts it to the specified type.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <typeparam name="T">The desired return type.</typeparam>
        /// <returns>The converted value or default if the parameter value is null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
        public static T GetParameterValue<T>(this Parameter parameter)
        {
            var value = parameter.GetParameterValue();
            if (value == null) return default;

            if (value is T t) return t;

            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Retrieves the value of the parameter identified on the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="identifier">The parameter identifier string.</param>
        /// <returns>The parameter value or null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
        public static object GetParameterValue(this Element element, string identifier)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            return element.GetParameterValue(ParameterIdentifier.Parse(identifier));
        }

        /// <summary>
        /// Retrieves the value of the parameter identified on the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="identifier">The parameter identifier.</param>
        /// <returns>The parameter value or null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
        public static object GetParameterValue(this Element element, ParameterIdentifier identifier)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            using var parameter = element.GetParameter(identifier);
            var value = parameter?.GetParameterValue();

            if (value == null && parameter != null)
            {
                var name = identifier.Name ?? parameter.Definition?.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    foreach (Parameter p in element.Parameters)
                    {
                        if (p == parameter) continue;
                        if (p.Definition?.Name == name)
                        {
                            var next = p.GetParameterValue();
                            if (next != null) return next;
                        }
                    }

                    using var typeElement = element.GetElementType();
                    if (typeElement != null)
                    {
                        foreach (Parameter p in typeElement.Parameters)
                        {
                            if (p.Definition?.Name == name)
                            {
                                var next = p.GetParameterValue();
                                if (next != null) return next;
                            }
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Retrieves the value of the parameter identified on the element and converts it to the specified type.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="identifier">The parameter identifier string.</param>
        /// <typeparam name="T">The desired return type.</typeparam>
        /// <returns>The converted value or default if the parameter is not found or has a null value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
        public static T GetParameterValue<T>(this Element element, string identifier)
        {
            var value = element.GetParameterValue(identifier);
            if (value == null) return default;

            if (value is T t) return t;

            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Retrieves the value of the parameter identified on the element and converts it to the specified type.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="identifier">The parameter identifier.</param>
        /// <typeparam name="T">The desired return type.</typeparam>
        /// <returns>The converted value or default if the parameter is not found or has a null value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
        public static T GetParameterValue<T>(this Element element, ParameterIdentifier identifier)
        {
            var value = element.GetParameterValue(identifier);
            if (value == null) return default;

            if (value is T t) return t;

            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Sets the value of the parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when setting the value fails.</exception>
        public static void SetParameterValue(this Parameter parameter, object value)
        {
            if (!parameter.TrySetParameterValue(value, out var reason))
                throw new InvalidOperationException(reason);
        }

        /// <summary>
        /// Sets the value of the parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="reason">Outputs the failure reason.</param>
        /// <returns>True if successful.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
        public static bool TrySetParameterValue(this Parameter parameter, object value, out string reason)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            reason = null;

            if (parameter.IsReadOnly)
            {
                reason = "Parameter is read-only.";
                return false;
            }

            bool result;
            switch (parameter.StorageType)
            {
                case StorageType.Double:
                    if (value is double d)
                        result = parameter.Set(d);
                    else
                    {
                        reason = "Value must be a double.";
                        return false;
                    }
                    break;
                case StorageType.Integer:
                    if (value is int i)
                        result = parameter.Set(i);
                    else
                    {
                        reason = "Value must be an integer.";
                        return false;
                    }
                    break;
                case StorageType.String:
                    result = parameter.Set(value?.ToString());
                    break;
                case StorageType.ElementId:
                    if (value is ElementId id)
                    {
                        result = parameter.Set(id);
                    }
                    else
                    {
                        reason = "Value must be an ElementId.";
                        return false;
                    }
                    break;
                default:
                    reason = "Unsupported storage type.";
                    return false;
            }

            if (!result)
            {
                reason = "Parameter set failed.";
            }

            return result;
        }

        /// <summary>
        /// Sets the value of the parameter identified on the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="identifier">The parameter identifier string.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>True if successful.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
        public static void SetParameterValue(this Element element, string identifier, object value)
        {
            if (!element.TrySetParameterValue(identifier, value, out var reason))
                throw new InvalidOperationException(reason);
        }

        /// <summary>
        /// Sets the value of the parameter identified on the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="identifier">The parameter identifier.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>True if successful.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
        public static void SetParameterValue(this Element element, ParameterIdentifier identifier, object value)
        {
            if (!element.TrySetParameterValue(identifier, value, out var reason))
                throw new InvalidOperationException(reason);
        }

        /// <summary>
        /// Sets the value of the parameter identified on the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="identifier">The parameter identifier string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="reason">Outputs the failure reason.</param>
        /// <returns>True if successful.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
        public static bool TrySetParameterValue(this Element element, string identifier, object value, out string reason)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            return element.TrySetParameterValue(ParameterIdentifier.Parse(identifier), value, out reason);
        }

        /// <summary>
        /// Sets the value of the parameter identified on the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="identifier">The parameter identifier.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="reason">Outputs the failure reason.</param>
        /// <returns>True if successful.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
        public static bool TrySetParameterValue(this Element element, ParameterIdentifier identifier, object value, out string reason)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            using var parameter = element.GetParameter(identifier);
            if (parameter == null)
            {
                reason = "Parameter not found.";
                return false;
            }

            return parameter.TrySetParameterValue(value, out reason);
        }

        /// <summary>
        /// Creates a <see cref="ParameterIdentifier"/> for the parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>A <see cref="ParameterIdentifier"/> identifying the parameter.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
        public static ParameterIdentifier ToIdentifier(this Parameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            var identifier = new ParameterIdentifier();

            var guid = TryGetGuid(parameter);
            if (guid.HasValue)
            {
                identifier.Guid = guid.Value;
            }

            var bipProp = parameter.GetType().GetProperty("BuiltInParameter");
            if (bipProp != null)
            {
                var bipValue = bipProp.GetValue(parameter);
                if (bipValue is BuiltInParameter bip)
                {
                    identifier.BuiltInParameter = bip;
                }
            }

            if (parameter.Id != null)
            {
                var intValue = (int)parameter.Id.GetElementIdValue();
                if (intValue < 0)
                {
                    if (!identifier.BuiltInParameter.HasValue)
                        identifier.BuiltInParameter = (BuiltInParameter)intValue;
                }
                else
                {
                    identifier.Id = parameter.Id.GetElementIdValue();
                }
            }

            if (!string.IsNullOrEmpty(parameter.Definition?.Name))
            {
                identifier.Name = parameter.Definition.Name;
            }

            return identifier;
        }

        private static Guid? TryGetGuid(Parameter parameter)
        {
            var prop = parameter.GetType().GetProperty("GUID") ?? parameter.GetType().GetProperty("Guid");
            if (prop == null) return null;
            var value = prop.GetValue(parameter);
            if (value == null) return null;
            if (value is Guid g)
            {
                if (g == Guid.Empty) return null;
                return g;
            }
            var type = value.GetType();
            if (type.FullName == "System.Nullable`1[System.Guid]")
            {
                var ng = (Guid?)value;
                if (ng.HasValue && ng.Value != Guid.Empty) return ng.Value;
                return null;
            }
            return null;
        }
    }
}
