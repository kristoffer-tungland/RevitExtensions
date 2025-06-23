using Autodesk.Revit.DB;
using RevitExtensions;
using Xunit;

namespace RevitExtensions.Tests
{
    public class BuiltInParameterExtensionsTests
    {
        [Fact]
        public void ToElementId_ReturnsElementIdWithSameValue()
        {
            BuiltInParameter bip = (BuiltInParameter)(-42);
            var id = bip.ToElementId();

            Assert.Equal(-42, id.GetElementIdValue());
        }
    }
}
