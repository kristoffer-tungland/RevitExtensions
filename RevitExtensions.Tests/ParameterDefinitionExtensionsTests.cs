#if REVIT2022_OR_LESS
using Autodesk.Revit.DB;
using RevitExtensions;
using Xunit;

namespace RevitExtensions.Tests
{
    public class ParameterDefinitionExtensionsTests
    {
        [Fact]
        public void GetDataType_ReturnsMappedForgeTypeId()
        {
            var def = new Definition { ParameterType = ParameterType.Text };

            var id = def.GetDataType();

            Assert.Equal(SpecTypeId.String.Text.TypeId, id.TypeId);
        }
    }
}
#endif
