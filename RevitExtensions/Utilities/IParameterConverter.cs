using Autodesk.Revit.DB;

namespace RevitExtensions.Utilities
{
    /// <summary>
    /// Converts a value from <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>
    /// in the context of a parameter.
    /// </summary>
    public interface IParameterConverter<TFrom, TTo>
    {
        /// <summary>
        /// Attempts to convert the specified value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="parameter">The parameter providing context.</param>
        /// <param name="result">The converted result.</param>
        /// <returns>True if the conversion succeeded.</returns>
        bool TryConvert(TFrom value, Parameter parameter, out TTo result);
    }
}
