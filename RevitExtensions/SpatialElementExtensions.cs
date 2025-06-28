using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using System.Collections.Generic;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for associating elements with rooms and spaces.
    /// </summary>
    public static class SpatialElementExtensions
    {
        private class StoredData
        {
            public ElementId? Id { get; set; }
            public XYZ? Point { get; set; }
            public ElementId? PhaseId { get; set; }
            public bool IsRoom { get; set; }
        }

        private static readonly Dictionary<Element, List<StoredData>> _storage = new Dictionary<Element, List<StoredData>>();

        private static StoredData? Find(Element element, bool isRoom, Phase? phase)
        {
            if (_storage.TryGetValue(element, out var list))
            {
                var phaseId = phase?.Id;
                foreach (var d in list)
                {
                    if (d.IsRoom == isRoom &&
                        ((phaseId == null && d.PhaseId == null) ||
                         (phaseId != null && phaseId.Equals(d.PhaseId))))
                        return d;
                }
            }
            return null;
        }

        private static IEnumerable<StoredData> FindAll(Element element, bool isRoom)
        {
            if (_storage.TryGetValue(element, out var list))
            {
                foreach (var d in list)
                {
                    if (d.IsRoom == isRoom)
                        yield return d;
                }
            }
        }

        private static void Set(Element element, bool isRoom, Phase? phase, ElementId? id, XYZ? point)
        {
            if (!_storage.TryGetValue(element, out var list))
            {
                list = new List<StoredData>();
                _storage[element] = list;
            }

            var data = Find(element, isRoom, phase);
            if (data != null)
            {
                data.Id = id;
                data.Point = point;
                data.PhaseId = phase?.Id;
            }
            else
            {
                list.Add(new StoredData { Id = id, Point = point, PhaseId = phase?.Id, IsRoom = isRoom });
            }
        }

        /// <summary>
        /// Associates the element with the given room in the specified phase.
        /// </summary>
        /// <param name="element">The element to tag.</param>
        /// <param name="room">The room to store.</param>
        /// <param name="phase">Optional phase the room belongs to.</param>
        public static void SetRoom(this Element element, Room room, Phase? phase = null)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (room == null) throw new ArgumentNullException(nameof(room));

            Set(element, true, phase, room.Id, null);
        }

        /// <summary>
        /// Stores a point used to locate the room for the element.
        /// </summary>
        /// <param name="element">The element to tag.</param>
        /// <param name="point">Point used to resolve the room.</param>
        /// <param name="phase">Optional phase to use for room lookup.</param>
        public static void SetRoom(this Element element, XYZ point, Phase? phase = null)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (point == null) throw new ArgumentNullException(nameof(point));

            Set(element, true, phase, null, point);
        }

        /// <summary>
        /// Associates the element with the given space in the specified phase.
        /// </summary>
        /// <param name="element">The element to tag.</param>
        /// <param name="space">The space to store.</param>
        /// <param name="phase">Optional phase the space belongs to.</param>
        public static void SetSpace(this Element element, Space space, Phase? phase = null)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (space == null) throw new ArgumentNullException(nameof(space));

            Set(element, false, phase, space.Id, null);
        }

        /// <summary>
        /// Stores a point used to locate the space for the element.
        /// </summary>
        /// <param name="element">The element to tag.</param>
        /// <param name="point">Point used to resolve the space.</param>
        /// <param name="phase">Optional phase to use for space lookup.</param>
        public static void SetSpace(this Element element, XYZ point, Phase? phase = null)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (point == null) throw new ArgumentNullException(nameof(point));

            Set(element, false, phase, null, point);
        }

        /// <summary>
        /// Retrieves the room associated with the element for the given phase.
        /// </summary>
        /// <param name="element">The element to inspect.</param>
        /// <param name="phase">Optional phase for which to get the room.</param>
        /// <returns>The resolved room or <c>null</c> if none is found.</returns>
        public static Room? GetRoom(this Element element, Phase? phase = null)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            var data = Find(element, true, phase);
            var doc = element.Document;
            if (data != null && doc != null)
            {
                if (data.Id != null)
                    return doc.GetElement(data.Id) as Room;
                if (data.Point != null)
                    return doc.GetRoomAtPoint(data.Point);
            }
            return null;
        }

        /// <summary>
        /// Retrieves the space associated with the element for the given phase.
        /// </summary>
        /// <param name="element">The element to inspect.</param>
        /// <param name="phase">Optional phase for which to get the space.</param>
        /// <returns>The resolved space or <c>null</c> if none is found.</returns>
        public static Space? GetSpace(this Element element, Phase? phase = null)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            var data = Find(element, false, phase);
            var doc = element.Document;
            if (data != null && doc != null)
            {
                if (data.Id != null)
                    return doc.GetElement(data.Id) as Space;
                if (data.Point != null)
                    return doc.GetSpaceAtPoint(data.Point);
            }
            return null;
        }

        /// <summary>
        /// Retrieves the room or space associated with the element for the given phase.
        /// </summary>
        /// <param name="element">The element to inspect.</param>
        /// <param name="phase">Optional phase used for lookup.</param>
        /// <returns>The room or space if available, otherwise <c>null</c>.</returns>
        public static SpatialElement? GetRoomOrSpace(this Element element, Phase? phase = null)
        {
            return element.GetRoom(phase) ?? (SpatialElement?)element.GetSpace(phase);
        }

        /// <summary>
        /// Retrieves all stored room ids for the element keyed by phase.
        /// </summary>
        /// <param name="element">Element to inspect.</param>
        /// <returns>Mapping of phase to room id.</returns>
        public static IReadOnlyDictionary<Phase, ElementId> GetStoredRoomIds(this Element element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            var result = new Dictionary<Phase, ElementId>();
            var doc = element.Document;

            foreach (var d in FindAll(element, true))
            {
                if (d.Id != null)
                {
                    Phase? phase = d.PhaseId != null && doc != null ? doc.GetElement(d.PhaseId) as Phase : null;
                    if (phase != null)
                        result[phase] = d.Id;
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves all stored space ids for the element keyed by phase.
        /// </summary>
        /// <param name="element">Element to inspect.</param>
        /// <returns>Mapping of phase to space id.</returns>
        public static IReadOnlyDictionary<Phase, ElementId> GetStoredSpaceIds(this Element element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            var result = new Dictionary<Phase, ElementId>();
            var doc = element.Document;

            foreach (var d in FindAll(element, false))
            {
                if (d.Id != null)
                {
                    Phase? phase = d.PhaseId != null && doc != null ? doc.GetElement(d.PhaseId) as Phase : null;
                    if (phase != null)
                        result[phase] = d.Id;
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves all stored room points for the element keyed by phase.
        /// </summary>
        /// <param name="element">Element to inspect.</param>
        /// <returns>Mapping of phase to point.</returns>
        public static IReadOnlyDictionary<Phase, XYZ> GetStoredRoomPoints(this Element element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            var result = new Dictionary<Phase, XYZ>();
            var doc = element.Document;

            foreach (var d in FindAll(element, true))
            {
                if (d.Point != null)
                {
                    Phase? phase = d.PhaseId != null && doc != null ? doc.GetElement(d.PhaseId) as Phase : null;
                    if (phase != null)
                        result[phase] = d.Point;
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves all stored space points for the element keyed by phase.
        /// </summary>
        /// <param name="element">Element to inspect.</param>
        /// <returns>Mapping of phase to point.</returns>
        public static IReadOnlyDictionary<Phase, XYZ> GetStoredSpacePoints(this Element element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            var result = new Dictionary<Phase, XYZ>();
            var doc = element.Document;

            foreach (var d in FindAll(element, false))
            {
                if (d.Point != null)
                {
                    Phase? phase = d.PhaseId != null && doc != null ? doc.GetElement(d.PhaseId) as Phase : null;
                    if (phase != null)
                        result[phase] = d.Point;
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether any room or space data is stored on the element.
        /// </summary>
        /// <param name="element">Element to inspect.</param>
        /// <returns><c>true</c> if any room or space reference or point is stored; otherwise <c>false</c>.</returns>
        public static bool HasStoredRoomOrSpace(this Element element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            return _storage.TryGetValue(element, out var list) && list.Count > 0;
        }
    }
}
