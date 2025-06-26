using System;
using Autodesk.Revit.DB;

namespace RevitExtensions.Models
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
        public BuiltInParameter? BuiltInParameter { get; internal set; }

        /// <summary>
        /// Gets the shared parameter guid if the identifier represents one.
        /// </summary>
        public Guid? Guid { get; internal set; }

        /// <summary>
        /// Gets the parameter name if the identifier represents one.
        /// </summary>
        public string? Name { get; internal set; }

        /// <summary>
        /// Gets the element id value if the identifier represents one.
        /// </summary>
        public long? Id { get; internal set; }

        public ParameterIdentifier() { }

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

            var parts = value.Split(new[] { ';' }, 2);
            var token = parts[0];

            if (System.Guid.TryParse(token, out var guid))
            {
                identifier.Guid = guid;
            }
            else if (int.TryParse(token, out var intValue))
            {
                if (intValue < 0)
                {
                    identifier.BuiltInParameter = (BuiltInParameter)intValue;
                }
                else
                {
                    identifier.Id = intValue;
                }
            }
            else
            {
                identifier.Name = token;
            }

            if (parts.Length > 1)
                identifier.Name = parts[1];

            return identifier;
        }


        /// <summary>
        /// Returns the most stable string representation.
        /// </summary>
        /// <returns>The stable representation string.</returns>
        public string ToStableRepresentation()
        {
            if (this.Guid.HasValue)
            {
                return $"{this.Guid.Value};{this.Name}";
            }

            if (this.BuiltInParameter.HasValue)
            {
                return $"{(int)this.BuiltInParameter.Value};{this.Name}";
            }

            if (!string.IsNullOrEmpty(this.Name))
                return this.Name;

            if (this.Id.HasValue)
                return this.Id.Value.ToString();

            return string.Empty;
        }

    }
}
