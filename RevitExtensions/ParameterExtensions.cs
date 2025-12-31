using System;
using Autodesk.Revit.DB;
using System.Globalization;
using RevitExtensions.Models;
using RevitExtensions.Utilities;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods related to Revit parameters.
    /// </summary>
    public static class ParameterExtensions
    {
        extension (Element element)
        {
            /// <summary>
            /// Gets a parameter from the element or its type using a flexible identifier.
            /// </summary>
            /// <param name="identifier">The parameter identifier string.</param>
            /// <returns>The found parameter or null.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public Parameter? GetParameter(string identifier)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                if (identifier == null) throw new ArgumentNullException(nameof(identifier));

                return element.GetParameter(ParameterIdentifier.Parse(identifier));
            }

            /// <summary>
            /// Gets a parameter from the element or its type using a <see cref="ParameterIdentifier"/>.
            /// </summary>
            /// <param name="identifier">The parameter identifier.</param>
            /// <returns>The found parameter or null.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public Parameter? GetParameter(ParameterIdentifier identifier)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                if (identifier == null) throw new ArgumentNullException(nameof(identifier));

                Parameter FindOn(Element source)
                {
                    Parameter p = null;
                    if (identifier.Guid.HasValue)
                    {
                        p = source.get_Parameter(identifier.Guid.Value);
                        if (p != null) return p;
                    }

                    if (identifier.BuiltInParameter.HasValue)
                    {
                        p = source.get_Parameter(identifier.BuiltInParameter.Value);
                        if (p != null) return p;
                    }

                    if (identifier.Id.HasValue)
                    {
                        var target = identifier.Id.Value;
                        foreach (Parameter ip in source.Parameters)
                        {
                            if (ip.Id != null && ip.Id.Value == target)
                                return ip;
                        }
                    }

                    if (!identifier.Guid.HasValue &&
                        !identifier.BuiltInParameter.HasValue &&
                        !identifier.Id.HasValue &&
                        !string.IsNullOrEmpty(identifier.Name))
                    {
                        return source.LookupParameter(identifier.Name);
                    }

                    return null;
                }

                var parameter = FindOn(element);
                if (parameter != null) return parameter;

                using var typeElement = element.ElementType;
                if (typeElement != null)
                {
                    parameter = FindOn(typeElement);
                }

                return parameter;
            }

            /// <summary>
            /// Looks up a parameter from the element or its type using a <see cref="ParameterIdentifier"/>.
            /// This method will fall back to the parameter name when the strict lookup fails.
            /// </summary>
            /// <param name="identifier">The parameter identifier.</param>
            /// <returns>The found parameter or null.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public Parameter? LookupParameter(ParameterIdentifier identifier)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                if (identifier == null) throw new ArgumentNullException(nameof(identifier));

                var parameter = element.GetParameter(identifier);
                if (parameter != null) return parameter;

                if (!string.IsNullOrEmpty(identifier.Name))
                {
                    parameter = element.LookupParameter(identifier.Name);
                    if (parameter != null) return parameter;

                    using var typeElement = element.ElementType;
                    return typeElement?.LookupParameter(identifier.Name);
                }

                return null;
            }

            public Parameter? LookupParameter(string identifier)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                if (identifier == null) throw new ArgumentNullException(nameof(identifier));

                return element.LookupParameter(ParameterIdentifier.Parse(identifier));
            }

            /// <summary>
            /// Retrieves the value of the parameter identified on the element.
            /// </summary>
            /// <param name="identifier">The parameter identifier string.</param>
            /// <returns>The parameter value or null.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public object? GetParameterValue(string identifier)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                if (identifier == null) throw new ArgumentNullException(nameof(identifier));

                return element.GetParameterValue(ParameterIdentifier.Parse(identifier));
            }

            /// <summary>
            /// Retrieves the value of the parameter identified on the element.
            /// </summary>
            /// <param name="identifier">The parameter identifier.</param>
            /// <returns>The parameter value or null.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public object? GetParameterValue(ParameterIdentifier identifier)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                if (identifier == null) throw new ArgumentNullException(nameof(identifier));

                using var parameter = element.GetParameter(identifier);
                return parameter?.Value;
            }

            /// <summary>
            /// Retrieves detailed information about the parameter identified on the element.
            /// </summary>
            /// <param name="identifier">The parameter identifier string.</param>
            /// <returns>The detailed parameter value or null.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public ParameterValueDetailed? GetValueDetailed(string identifier)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                if (identifier == null) throw new ArgumentNullException(nameof(identifier));

                return element.GetValueDetailed(ParameterIdentifier.Parse(identifier));
            }

            /// <summary>
            /// Retrieves detailed information about the parameter identified on the element.
            /// </summary>
            /// <param name="identifier">The parameter identifier.</param>
            /// <returns>The detailed parameter value or null.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public ParameterValueDetailed? GetValueDetailed(ParameterIdentifier identifier)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                if (identifier == null) throw new ArgumentNullException(nameof(identifier));

                using var parameter = element.GetParameter(identifier);
                return parameter?.ValueDetailed;
            }

            /// <summary>
            /// Retrieves the value of the parameter identified on the element and converts it to the specified type.
            /// </summary>
            /// <param name="identifier">The parameter identifier string.</param>
            /// <typeparam name="T">The desired return type.</typeparam>
            /// <returns>The converted value or default if the parameter is not found or has a null value.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public T GetParameterValue<T>(string identifier)
            {
                using var parameter = element.GetParameter(ParameterIdentifier.Parse(identifier));
                return parameter != null ? parameter.GetParameterValue<T>() : default;
            }

            /// <summary>
            /// Retrieves the value of the parameter identified on the element and converts it to the specified type.
            /// </summary>
            /// <param name="identifier">The parameter identifier.</param>
            /// <typeparam name="T">The desired return type.</typeparam>
            /// <returns>The converted value or default if the parameter is not found or has a null value.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public T GetParameterValue<T>(ParameterIdentifier identifier)
            {
                using var parameter = element.GetParameter(identifier);
                return parameter != null ? parameter.GetParameterValue<T>() : default;
            }

            /// <summary>
            /// Looks up the value of the parameter identified on the element.
            /// If the retrieved parameter has a null value, additional parameters with the same name are searched.
            /// </summary>
            /// <param name="identifier">The parameter identifier string.</param>
            /// <returns>The parameter value or null.</returns>
            public object? LookupParameterValue(string identifier)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                if (identifier == null) throw new ArgumentNullException(nameof(identifier));

                return element.LookupParameterValue(ParameterIdentifier.Parse(identifier));
            }

            /// <summary>
            /// Looks up the value of the parameter identified on the element.
            /// If the retrieved parameter has a null value, additional parameters with the same name are searched.
            /// </summary>
            /// <param name="identifier">The parameter identifier.</param>
            /// <returns>The parameter value or null.</returns>
            public object? LookupParameterValue(ParameterIdentifier identifier)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                if (identifier == null) throw new ArgumentNullException(nameof(identifier));

                using var parameter = element.LookupParameter(identifier);
                var value = parameter?.Value;

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
                                var next = p.Value;
                                if (next != null) return next;
                            }
                        }

                        using var typeElement = element.ElementType;
                        if (typeElement != null)
                        {
                            foreach (Parameter p in typeElement.Parameters)
                            {
                                if (p.Definition?.Name == name)
                                {
                                    var next = p.Value;
                                    if (next != null) return next;
                                }
                            }
                        }
                    }
                }

                return value;
            }

            public T LookupParameterValue<T>(string identifier)
            {
                using var parameter = element.LookupParameter(ParameterIdentifier.Parse(identifier));
                return parameter != null ? parameter.GetParameterValue<T>() : default;
            }

            public T LookupParameterValue<T>(ParameterIdentifier identifier)
            {
                using var parameter = element.LookupParameter(identifier);
                return parameter != null ? parameter.GetParameterValue<T>() : default;
            }

            /// <summary>
            /// Sets the value of the parameter identified on the element.
            /// </summary>
            /// <param name="identifier">The parameter identifier string.</param>
            /// <param name="value">The value to set.</param>
            /// <returns>True if successful.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public void SetParameterValue(string identifier, object value)
            {
                if (!element.TrySetParameterValue(identifier, value, out var reason))
                    throw new InvalidOperationException(reason);
            }

            /// <summary>
            /// Sets the value of the parameter identified on the element.
            /// </summary>
            /// <param name="identifier">The parameter identifier.</param>
            /// <param name="value">The value to set.</param>
            /// <returns>True if successful.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public void SetParameterValue(ParameterIdentifier identifier, object value)
            {
                if (!element.TrySetParameterValue(identifier, value, out var reason))
                    throw new InvalidOperationException(reason);
            }

            /// <summary>
            /// Sets the value of the parameter identified on the element.
            /// </summary>
            /// <param name="identifier">The parameter identifier string.</param>
            /// <param name="value">The value to set.</param>
            /// <param name="reason">Outputs the failure reason.</param>
            /// <returns>True if successful.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public bool TrySetParameterValue(string identifier, object value, out string? reason)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                if (identifier == null) throw new ArgumentNullException(nameof(identifier));

                return element.TrySetParameterValue(ParameterIdentifier.Parse(identifier), value, out reason);
            }

            /// <summary>
            /// Sets the value of the parameter identified on the element.
            /// </summary>
            /// <param name="identifier">The parameter identifier.</param>
            /// <param name="value">The value to set.</param>
            /// <param name="reason">Outputs the failure reason.</param>
            /// <returns>True if successful.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="identifier"/> is null.</exception>
            public bool TrySetParameterValue(ParameterIdentifier identifier, object value, out string? reason)
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
        }

        extension (Parameter parameter)
        {
            /// <summary>
            /// Retrieves the value stored in the parameter.
            /// </summary>
            /// <value>The parameter value or null.</value>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
            public object? Value
            {
                get
                {
                    if (parameter == null) throw new ArgumentNullException(nameof(parameter));

                    return parameter.StorageType switch
                    {
                        StorageType.Double => (object)parameter.AsDouble(),
                        StorageType.Integer => parameter.AsInteger(),
                        StorageType.String => parameter.AsString(),
                        StorageType.ElementId => parameter.AsElementId()?.Value,
                        _ => null,
                    };
                }
            }

            /// <summary>
            /// Retrieves detailed information about the parameter value.
            /// </summary>
            /// <value>A <see cref="ParameterValueDetailed"/> instance.</value>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
            public ParameterValueDetailed ValueDetailed
            {
                get
                {
                    if (parameter == null) throw new ArgumentNullException(nameof(parameter));

                    return BuildValueDetailed(parameter);
                }
            }

            /// <summary>
            /// Retrieves the value stored in the parameter and converts it to the specified type.
            /// </summary>
            /// <typeparam name="T">The desired return type.</typeparam>
            /// <returns>The converted value or default if the parameter value is null.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
            public T GetParameterValue<T>()
            {
                var value = parameter.Value;
                if (value == null)
                    return default;

                if (value is T t)
                    return t;

                if (CustomConverter.TryConvert(value, parameter, out T result))
                    return result;

                try
                {
                    return (T)System.Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                }
                catch
                {
                    return default!;
                }
            }

            /// <summary>
            /// Sets the value of the parameter.
            /// </summary>
            /// <param name="value">The value to set. Strings starting with '=' are
            /// evaluated as arithmetic expressions supporting m, cm, mm, ft and in
            /// units.</param>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
            /// <exception cref="InvalidOperationException">Thrown when setting the value fails.</exception>
            public void SetParameterValue(object value)
            {
                if (!parameter.TrySetParameterValue(value, out var reason))
                    throw new InvalidOperationException(reason);
            }

            /// <summary>
            /// Sets the value of the parameter.
            /// </summary>
            /// <param name="value">The value to set. Strings starting with '=' are
            /// evaluated as arithmetic expressions supporting m, cm, mm, ft and in
            /// units.</param>
            /// <param name="reason">Outputs the failure reason.</param>
            /// <returns>True if successful.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
            public bool TrySetParameterValue(object value, out string? reason)
            {
                if (parameter == null) throw new ArgumentNullException(nameof(parameter));

                reason = null;

                var owner = parameter.Element;
                if (owner != null && !owner.CanEdit(out var status))
                {
                    reason = status.FriendlyName;
                    return false;
                }

                if (parameter.IsReadOnly)
                {
                    reason = "Parameter is read-only.";
                    return false;
                }

                bool result;
                switch (parameter.StorageType)
                {
                    case StorageType.Double:
                        double d;
                        if (!CustomConverter.TryConvert(value, parameter, out d))
                        {
                            reason = "Value must be a number.";
                            return false;
                        }

                        if (parameter.AsDouble() == d) return true;
                        result = parameter.Set(d);
                        break;
                    case StorageType.Integer:
                        int i;
                        if (!CustomConverter.TryConvert(value, parameter, out i))
                        {
                            reason = "Value must be an integer.";
                            return false;
                        }

                        if (parameter.AsInteger() == i) return true;
                        result = parameter.Set(i);
                        break;
                    case StorageType.String:
                        string str;
                        if (!CustomConverter.TryConvert(value, parameter, out str))
                            str = CustomConverter.ToInvariantString(value);

                        if (string.Equals(parameter.AsString(), str)) return true;
                        result = parameter.Set(str);
                        break;
                    case StorageType.ElementId:
                        ElementId id;
                        if (!CustomConverter.TryConvert(value, parameter, out id))
                        {
                            reason = "Value must be an ElementId.";
                            return false;
                        }

                        var current = parameter.AsElementId();
                        if ((current == null && id == null) ||
                            (current != null && current.Value == id.Value))
                            return true;
                        result = parameter.Set(id);
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
            /// Creates a <see cref="ParameterIdentifier"/> for the parameter.
            /// </summary>
            /// <value>A <see cref="ParameterIdentifier"/> identifying the parameter.</value>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
            public ParameterIdentifier Identifier
            {
                get
                {
                    if (parameter == null) throw new ArgumentNullException(nameof(parameter));

                    var identifier = new ParameterIdentifier();

                    var guid = TryGetGuid(parameter);
                    if (guid.HasValue)
                    {
                        identifier.Guid = guid.Value;
                    }

                    if (parameter.Definition is InternalDefinition internalDef)
                    {
                        identifier.BuiltInParameter = internalDef.BuiltInParameter;
                    }

                    if (parameter.Id != null)
                    {
                        var intValue = (int)parameter.Id.Value;
                        if (intValue < 0)
                        {
                            if (!identifier.BuiltInParameter.HasValue)
                                identifier.BuiltInParameter = (BuiltInParameter)intValue;
                        }
                        else
                        {
                            identifier.Id = parameter.Id.Value;
                        }
                    }

                    if (!string.IsNullOrEmpty(parameter.Definition?.Name))
                    {
                        identifier.Name = parameter.Definition.Name;
                    }

                    return identifier;
                }
            }
        }

        private static ParameterValueType GetParameterValueType(Parameter parameter)
        {
            if (parameter == null)
                return ParameterValueType.Text;

            var defaultType = parameter.StorageType switch
            {
                StorageType.Integer => ParameterValueType.Integer,
                StorageType.Double => ParameterValueType.Number,
                StorageType.ElementId => ParameterValueType.Element,
                _ => ParameterValueType.Text,
            };

            BuiltInParameter? bip = null;

            if (parameter.Definition is InternalDefinition internalDef)
            {
                bip = internalDef.BuiltInParameter;
            }

            if (!bip.HasValue && parameter.Id != null)
            {
                var intValue = (int)parameter.Id.Value;
                if (intValue < 0)
                    bip = (BuiltInParameter)intValue;
            }

            if (bip == BuiltInParameter.ELEM_PARTITION_PARAM)
                return ParameterValueType.Workset;

            return defaultType;
        }

        private static ParameterValueDetailed BuildValueDetailed(Parameter parameter)
        {
            var value = parameter.Value;

            return new ParameterValueDetailed
            {
                Value = value,
                ValueString = parameter.AsValueString(),
                ValueType = GetParameterValueType(parameter),
            };
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
