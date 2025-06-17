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
    }
}
