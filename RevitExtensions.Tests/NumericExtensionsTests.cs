using Autodesk.Revit.DB;
using RevitExtensions;
using Xunit;

namespace RevitExtensions.Tests
{
    public class NumericExtensionsTests
    {
        [Fact]
        public void ToElementId_Int_ReturnsElementId()
        {
            int value = 5;
            var id = value.ToElementId();

            Assert.Equal(5L, id.GetElementIdValue());
        }

        [Fact]
        public void ToElementId_Long_ReturnsElementId()
        {
            long value = 7;
            var id = value.ToElementId();

            Assert.Equal(7L, id.GetElementIdValue());
        }
    }
}
