using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Provides helpers for working with document units in tests.
    /// </summary>
    public static class DocumentUnitExtensions
    {
        private class Data { public double LengthScale = 1.0; }
        private static readonly ConditionalWeakTable<Document, Data> Cache = new ConditionalWeakTable<Document, Data>();

        internal static double GetLengthUnitScale(this Document document)
        {
            return Cache.TryGetValue(document, out var data) ? data.LengthScale : 1.0;
        }

        /// <summary>
        /// Sets the length unit scale used when evaluating expressions in tests.
        /// A value of 1 represents feet, 0.00328084 for millimeters, etc.
        /// </summary>
        /// <param name="document">The document to configure.</param>
        /// <param name="scale">The scale factor to convert document units to feet.</param>
        public static void SetTestLengthUnitScale(this Document document, double scale)
        {
            var data = Cache.GetOrCreateValue(document);
            data.LengthScale = scale;
        }
    }
}
