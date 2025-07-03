using Autodesk.Revit.DB;
using RevitExtensions;
using Xunit;

namespace RevitExtensions.Tests
{
    public class ElementExtensionsTests
    {
        [Fact]
        public void GetElementIdValue_FromElementId_ReturnsValue()
        {
            var id = new ElementId(42);
            Assert.Equal(42L, id.GetElementIdValue());
        }

        [Fact]
        public void GetElementIdValue_FromElement_ReturnsValue()
        {
            var id = new ElementId(99);
            var element = new Element(id);
            Assert.Equal(99L, element.GetElementIdValue());
        }

        [Fact]
        public void CanEdit_NotWorkshared_ReturnsTrue()
        {
            var doc = new Document();
            var element = new Element(doc, new ElementId(1));

            var result = element.CanEdit(out var status);

            Assert.True(result);
            Assert.Equal(EditStatus.NotWorkshared, status);
        }

        [Fact]
        public void CanEdit_Unowned_ReturnsEditable()
        {
            var doc = new Document { IsWorkshared = true, CurrentUser = "A" };
            var element = new Element(doc, new ElementId(2));

            var result = element.CanEdit(out var status);

            Assert.True(result);
            Assert.Equal(EditStatus.Editable, status);
        }

        [Fact]
        public void CanEdit_OwnedBySelf_ReturnsOwnedByCurrentUser()
        {
            var doc = new Document { IsWorkshared = true, CurrentUser = "A" };
            var id = new ElementId(3);
            doc.SetElementOwner(id, "A");
            var element = new Element(doc, id);

            var result = element.CanEdit(out var status);

            Assert.True(result);
            Assert.Equal(EditStatus.OwnedByCurrentUser, status);
        }

        [Fact]
        public void CanEdit_OwnedByOther_ReturnsFalse()
        {
            var doc = new Document { IsWorkshared = true, CurrentUser = "A" };
            var id = new ElementId(4);
            doc.SetElementOwner(id, "B");
            var element = new Element(doc, id);

            var result = element.CanEdit(out var status);

            Assert.False(result);
            Assert.Equal(EditStatus.OwnedByOtherUser, status);
        }

        [Fact]
        public void GetElementType_ReturnsTypeElement()
        {
            var doc = new Document();
            var type = new Element(doc, new ElementId(10));
            doc.AddElement(type);

            var element = new Element(doc, new ElementId(1)) { TypeId = new ElementId(10) };

            using var result = element.GetElementType();
            Assert.Same(type, result);
        }

        [Fact]
        public void GetElementType_NoDocument_ReturnsNull()
        {
            var element = new Element(new ElementId(2)) { TypeId = new ElementId(99) };
            using var type = element.GetElementType();
            Assert.Null(type);
        }

        [Fact]
        public void ToElement_ReturnsElementFromDocument()
        {
            var doc = new Document();
            var id = new ElementId(11);
            var element = new Element(doc, id);
            doc.AddElement(element);

            var result = id.ToElement(doc);

            Assert.Same(element, result);
        }

        [Fact]
        public void ToElement_Generic_ReturnsTypedElement()
        {
            var doc = new Document();
            var id = new ElementId(12);
            var wall = new Wall(id);
            doc.AddElement(wall);

            var result = id.ToElement<Wall>(doc);

            Assert.Same(wall, result);
        }

        [Fact]
        public void ToElement_Generic_WrongType_ReturnsNull()
        {
            var doc = new Document();
            var id = new ElementId(13);
            var element = new Element(doc, id);
            doc.AddElement(element);

            var result = id.ToElement<Wall>(doc);

            Assert.Null(result);
        }

        [Fact]
        public void CanEdit_ModelLocked_ReturnsFalse()
        {
            var doc = new Document { IsWorkshared = true, CurrentUser = "A" };
            var element = new Element(doc, new ElementId(14)) { IsModifiable = false };

            var result = element.CanEdit(out var status);

            Assert.False(result);
            Assert.Equal(EditStatus.ModelLocked, status);
        }

        [Fact]
        public void CanEdit_LinkedModel_ReturnsFalse()
        {
            var doc = new Document { IsWorkshared = true, CurrentUser = "A", IsLinked = true };
            var element = new Element(doc, new ElementId(15));

            var result = element.CanEdit(out var status);

            Assert.False(result);
            Assert.Equal(EditStatus.LinkedModel, status);
        }

        [Fact]
        public void SetWorkset_ById_UpdatesParameter()
        {
            var element = new Element(new ElementId(20));
            var parameter = new Parameter(BuiltInParameter.ELEM_PARTITION_PARAM) { StorageType = StorageType.Integer };
            element.Parameters.Add(parameter);

            element.SetWorkset(new WorksetId(3));

            Assert.Equal(3, parameter.AsInteger());
        }

        [Fact]
        public void SetWorkset_ByWorkset_UpdatesParameter()
        {
            var element = new Element(new ElementId(21));
            var parameter = new Parameter(BuiltInParameter.ELEM_PARTITION_PARAM) { StorageType = StorageType.Integer };
            element.Parameters.Add(parameter);
            var ws = new Workset(new WorksetId(5));

            element.SetWorkset(ws);

            Assert.Equal(5, parameter.AsInteger());
        }
    }
}
