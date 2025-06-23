using System;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for Autodesk Revit elements.
    /// </summary>
    public static class ElementExtensions
    {
        /// <summary>
        /// Gets the numeric id of the element as a <see cref="long"/> regardless of Revit version.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The element id as a long.</returns>
        public static long GetElementIdValue(this Element element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            return element.Id.GetElementIdValue();
        }

        /// <summary>
        /// Gets the numeric value of the element id as a <see cref="long"/>.
        /// Handles Revit versions prior to 2024 where the value was stored as an <see cref="int"/>.
        /// </summary>
        /// <param name="id">The element id.</param>
        /// <returns>The id value as a long.</returns>
        public static long GetElementIdValue(this ElementId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
#if REVIT2024_OR_ABOVE
            return id.Value;
#else
            return id.IntegerValue;
#endif
        }

        /// <summary>
        /// Gets the element with this id from the given document.
        /// </summary>
        /// <param name="id">The element id.</param>
        /// <param name="document">The owning document.</param>
        /// <returns>The element or <c>null</c> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="id"/> or <paramref name="document"/> is null.
        /// </exception>
        public static Element? ToElement(this ElementId id, Document document)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (document == null) throw new ArgumentNullException(nameof(document));

            return document.GetElement(id);
        }

        /// <summary>
        /// Gets the element with this id from the given document and casts it to the specified type.
        /// </summary>
        /// <typeparam name="T">The expected element type.</typeparam>
        /// <param name="id">The element id.</param>
        /// <param name="document">The owning document.</param>
        /// <returns>The element cast to <typeparamref name="T"/> if found; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="id"/> or <paramref name="document"/> is null.
        /// </exception>
        public static T? ToElement<T>(this ElementId id, Document document) where T : Element
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (document == null) throw new ArgumentNullException(nameof(document));

            return document.GetElement(id) as T;
        }

        /// <summary>
        /// Determines if the element can be edited.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <returns>True if the element can be edited.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
        public static bool CanEdit(this Element element)
        {
            return element.CanEdit(out _);
        }

        /// <summary>
        /// Determines if the element can be edited.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <param name="status">Outputs the edit status.</param>
        /// <returns>True if the element can be edited.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
        public static bool CanEdit(this Element element, out EditStatus status)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            var document = element.Document;
            if (document == null || !document.IsWorkshared)
            {
                status = EditStatus.NotWorkshared;
                return true;
            }

            var checkout = WorksharingUtils.GetCheckoutStatus(document, element.Id);

            switch (checkout)
            {
                case CheckoutStatus.OwnedByCurrentUser:
                    status = EditStatus.OwnedByCurrentUser;
                    return true;
                case CheckoutStatus.NotOwned:
                    status = EditStatus.Editable;
                    return true;
                default:
                    status = EditStatus.OwnedByOtherUser;
                    return false;
            }
        }

        /// <summary>
        /// Retrieves the element type for the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The element type or null if unavailable.</returns>
        public static Element? GetElementType(this Element element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            var document = element.Document;
            if (document == null) return null;

            var typeId = element.GetTypeId();
            if (typeId == null) return null;

            return document.GetElement(typeId);
        }
    }
}
