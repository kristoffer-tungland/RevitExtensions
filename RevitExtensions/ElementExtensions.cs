using System;
using Autodesk.Revit.DB;
using RevitExtensions.Models;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for Autodesk Revit elements.
    /// </summary>
    public static class ElementExtensions
    {
        extension (Element element)
        {
            /// <summary>
            /// Gets the numeric id of the element as a <see cref="long"/> regardless of Revit version.
            /// </summary>
            /// <value>The element id as a long.</value>
            public long IdValue
            {
                get
                {
                    if (element == null) throw new ArgumentNullException(nameof(element));
                    return element.Id.Value;
                }
            }

            /// <summary>
            /// Determines if the element can be edited.
            /// </summary>
            /// <value><c>true</c> if the element can be edited.</value>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
            public bool IsEditable => element.CanEdit(out _);

            /// <summary>
            /// Determines if the element can be edited.
            /// </summary>
            /// <param name="status">Outputs the edit status.</param>
            /// <returns>True if the element can be edited.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
            public bool CanEdit(out EditStatus status)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));

                var document = element.Document;
                bool canEdit;

                if (document == null || !document.IsWorkshared)
                {
                    status = EditStatus.NotWorkshared;
                    canEdit = true;
                }
                else
                {
                    var checkout = WorksharingUtils.GetCheckoutStatus(document, element.Id);
                    switch (checkout)
                    {
                        case CheckoutStatus.OwnedByCurrentUser:
                            status = EditStatus.OwnedByCurrentUser;
                            canEdit = true;
                            break;
                        case CheckoutStatus.NotOwned:
                            status = EditStatus.Editable;
                            canEdit = true;
                            break;
                        default:
                            status = EditStatus.OwnedByOtherUser;
                            canEdit = false;
                            break;
                    }
                }

                if (!canEdit)
                    return false;

#if REVIT2020_OR_ABOVE
                if (document != null && document.IsLinked)
                {
                    status = EditStatus.LinkedModel;
                    return false;
                }
#endif

#if REVIT2024_OR_ABOVE
                if (!element.IsModifiable)
                {
                    status = EditStatus.ModelLocked;
                    return false;
                }
#endif

                return true;
            }

            /// <summary>
            /// Retrieves the element type for the given element.
            /// </summary>
            /// <value>The element type or null if unavailable.</value>
            public Element? ElementType
            {
                get
                {
                    if (element == null) throw new ArgumentNullException(nameof(element));

                    var document = element.Document;
                    if (document == null) return null;

                    var typeId = element.GetTypeId();
                    if (typeId == null) return null;

                    return document.GetElement(typeId);
                }
            }

            /// <summary>
            /// Assigns the element to the specified workset.
            /// </summary>
            /// <param name="workset">The workset to assign.</param>
            public void SetWorkset(Workset workset)
            {
                if (workset == null) throw new ArgumentNullException(nameof(workset));
                element.SetWorkset(workset.Id);
            }

            /// <summary>
            /// Assigns the element to the specified workset.
            /// </summary>
            /// <param name="worksetId">The workset id to assign.</param>
            public void SetWorkset(WorksetId worksetId)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                if (worksetId == null) throw new ArgumentNullException(nameof(worksetId));

                var identifier = new ParameterIdentifier
                {
                    BuiltInParameter = BuiltInParameter.ELEM_PARTITION_PARAM
                };
                element.SetParameterValue(identifier, worksetId);
            }
        }

        extension (ElementId id)
        {
#if !REVIT2025_OR_ABOVE
            /// <summary>
            /// Gets the numeric value of the element id as a <see cref="long"/>.
            /// Handles Revit versions prior to 2024 where the value was stored as an <see cref="int"/>.
            /// </summary>
            /// <value>The id value as a long.</value>
            public long Value
            {
                get
                {
                    if (id == null) throw new ArgumentNullException(nameof(id));
#if REVIT2024_OR_ABOVE
                    return id.Value;
#elif REVIT2024_OR_LESS
                    return id.IntegerValue;
#else
                    return id.Value;
#endif
                }
            }
#endif

            /// <summary>
            /// Gets the element with this id from the given document.
            /// </summary>
            /// <param name="document">The owning document.</param>
            /// <returns>The element or <c>null</c> if not found.</returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="id"/> or <paramref name="document"/> is null.
            /// </exception>
            public Element? ToElement(Document document)
            {
                if (id == null) throw new ArgumentNullException(nameof(id));
                if (document == null) throw new ArgumentNullException(nameof(document));

                return document.GetElement(id);
            }

            /// <summary>
            /// Gets the element with this id from the given document and casts it to the specified type.
            /// </summary>
            /// <typeparam name="T">The expected element type.</typeparam>
            /// <param name="document">The owning document.</param>
            /// <returns>The element cast to <typeparamref name="T"/> if found; otherwise <c>null</c>.</returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="id"/> or <paramref name="document"/> is null.
            /// </exception>
            public T? ToElement<T>(Document document) where T : Element
            {
                if (id == null) throw new ArgumentNullException(nameof(id));
                if (document == null) throw new ArgumentNullException(nameof(document));

                return document.GetElement(id) as T;
            }
        }
    }
}
