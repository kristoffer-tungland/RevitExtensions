using System;

namespace RevitExtensions
{
    /// <summary>
    /// Reasons describing whether an element is available for editing.
    /// </summary>
    public enum EditStatus
    {
        /// <summary>
        /// The element is editable.
        /// </summary>
        Editable,

        /// <summary>
        /// Worksharing is disabled for the document.
        /// </summary>
        NotWorkshared,

        /// <summary>
        /// The element is owned by the current user.
        /// </summary>
        OwnedByCurrentUser,

        /// <summary>
        /// The element is owned by another user.
        /// </summary>
        OwnedByOtherUser,
    }

    /// <summary>
    /// Extension methods for <see cref="EditStatus"/>.
    /// </summary>
    public static class EditStatusExtensions
    {
        /// <summary>
        /// Gets a human readable string for the given status.
        /// </summary>
        /// <param name="status">The status to describe.</param>
        /// <returns>A user friendly description.</returns>
        public static string ToFriendlyString(this EditStatus status)
        {
            switch (status)
            {
                case EditStatus.Editable:
                    return "Editable";
                case EditStatus.NotWorkshared:
                    return "Not workshared";
                case EditStatus.OwnedByCurrentUser:
                    return "Owned by current user";
                case EditStatus.OwnedByOtherUser:
                    return "Owned by another user";
                default:
                    return status.ToString();
            }
        }
    }
}
