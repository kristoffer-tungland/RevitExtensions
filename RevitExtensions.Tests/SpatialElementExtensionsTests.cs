using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using RevitExtensions;
using Xunit;

namespace RevitExtensions.Tests
{
    public class SpatialElementExtensionsTests
    {
        [Fact]
        public void SetRoom_WithRoom_ReturnsSameRoom()
        {
            var doc = new Document();
            var room = new Room(new ElementId(1));
            doc.AddElement(room);

            var element = new Element(doc, new ElementId(2));
            element.SetRoom(room);

            using var result = element.GetRoom();
            Assert.Same(room, result);
        }

        [Fact]
        public void SetRoom_Point_UsesDocumentLookup()
        {
            var doc = new Document();
            var room = new Room(new ElementId(3));
            var point = new XYZ(1, 2, 3);
            doc.AddElement(room);
            doc.AddRoom(room, point);

            var element = new Element(doc, new ElementId(4));
            element.SetRoom(point);

            using var result = element.GetRoom();
            Assert.Same(room, result);
        }

        [Fact]
        public void SetSpace_WithSpace_ReturnsSameSpace()
        {
            var doc = new Document();
            var space = new Space(new ElementId(5));
            doc.AddElement(space);

            var element = new Element(doc, new ElementId(6));
            element.SetSpace(space);

            using var result = element.GetSpace();
            Assert.Same(space, result);
        }

        [Fact]
        public void StoredMappings_ReturnExpectedDictionaries()
        {
            var doc = new Document();
            var phase = new Phase(new ElementId(10));
            doc.AddElement(phase);

            var room = new Room(new ElementId(20));
            var space = new Space(new ElementId(30));
            doc.AddElement(room);
            doc.AddElement(space);

            var spacePoint = new XYZ(2, 2, 2);

            var element = new Element(doc, new ElementId(40));
            element.SetRoom(room, phase);
            element.SetSpace(spacePoint, phase);

            var roomIds = element.GetStoredRoomIds();
            var spacePoints = element.GetStoredSpacePoints();

            Assert.Single(roomIds);
            Assert.Equal(room.Id, roomIds[phase]);

            Assert.Single(spacePoints);
            Assert.Equal(spacePoint, spacePoints[phase]);

            Assert.True(element.HasStoredRoomOrSpace());
        }
    }
}
