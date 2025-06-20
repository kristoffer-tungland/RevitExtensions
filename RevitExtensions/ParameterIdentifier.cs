using System;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Represents a parameter identifier that can be expressed as a built-in parameter,
    /// a shared parameter guid, a name, or an element id.
    /// </summary>
    public class ParameterIdentifier
    {
        /// <summary>
        /// Gets the built-in parameter value if the identifier represents one.
        /// </summary>
        public BuiltInParameter? BuiltInParameter { get; private set; }

        /// <summary>
        /// Gets the shared parameter guid if the identifier represents one.
        /// </summary>
        public Guid? Guid { get; private set; }

        /// <summary>
        /// Gets the parameter name if the identifier represents one.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the element id value if the identifier represents one.
        /// </summary>
        public long? Id { get; private set; }

        private ParameterIdentifier() { }

        /// <summary>
        /// Parses a parameter identifier from a string.
        /// </summary>
        /// <param name="value">The identifier string.</param>
        /// <returns>A <see cref="ParameterIdentifier"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        public static ParameterIdentifier Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var identifier = new ParameterIdentifier();

            if (System.Guid.TryParse(value, out var guid))
            {
                identifier.Guid = guid;
                return identifier;
            }

            if (int.TryParse(value, out var intValue))
            {
                if (intValue < 0)
                {
                    identifier.BuiltInParameter = (BuiltInParameter)intValue;
                }
                else
                {
                    identifier.Id = intValue;
                }

                return identifier;
            }

            identifier.Name = value;
            return identifier;
        }

        /// <summary>
        /// Creates a <see cref="ParameterIdentifier"/> from a Revit <see cref="Parameter"/>.
        /// </summary>
        /// <param name="parameter">The parameter to convert.</param>
        /// <returns>A new <see cref="ParameterIdentifier"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
        public static ParameterIdentifier FromParameter(Parameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            var identifier = new ParameterIdentifier();

            var guid = TryGetGuid(parameter);
            if (guid.HasValue)
            {
                identifier.Guid = guid.Value;
                return identifier;
            }

            var bipProp = parameter.GetType().GetProperty("BuiltInParameter");
            if (bipProp != null)
            {
                var bipValue = bipProp.GetValue(parameter);
                if (bipValue is BuiltInParameter bip)
                {
                    identifier.BuiltInParameter = bip;
                    return identifier;
                }
            }

            if (parameter.Id != null)
            {
                var intValue = (int)parameter.Id.GetElementIdValue();
                if (intValue < 0)
                {
                    identifier.BuiltInParameter = (BuiltInParameter)intValue;
                    return identifier;
                }

                identifier.Id = parameter.Id.GetElementIdValue();
            }

            if (!string.IsNullOrEmpty(parameter.Definition?.Name))
            {
                identifier.Name = parameter.Definition.Name;
                return identifier;
            }

            return identifier;
        }

        /// <summary>
        /// Returns the most stable string representation.
        /// </summary>
        /// <returns>The stable representation string.</returns>
        public string ToStableRepresentation()
        {
            if (this.Guid.HasValue)
                return this.Guid.Value.ToString();

            if (this.BuiltInParameter.HasValue)
                return ((int)this.BuiltInParameter.Value).ToString();

            if (!string.IsNullOrEmpty(this.Name))
                return this.Name;

            if (this.Id.HasValue)
                return this.Id.Value.ToString();

            return string.Empty;
        }

        private static Guid? TryGetGuid(Parameter parameter)
        {
            var prop = parameter.GetType().GetProperty("GUID") ?? parameter.GetType().GetProperty("Guid");
            if (prop == null) return null;
            var value = prop.GetValue(parameter);
            if (value == null) return null;
            if (value is System.Guid g)
            {
                if (g == System.Guid.Empty) return null;
                return g;
            }
            var type = value.GetType();
            if (type.FullName == "System.Nullable`1[System.Guid]")
            {
                var ng = (System.Nullable<System.Guid>)value;
                if (ng.HasValue && ng.Value != System.Guid.Empty) return ng.Value;
                return null;
            }
            return null;
        }
    }
}
