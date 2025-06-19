using System;
using Autodesk.Revit.DB;
using RevitExtensions;
using Xunit;

namespace RevitExtensions.Tests
{
    public class ParameterExtensionsTests
    {
        [Fact]
        public void GetParameter_NegativeInt_UsesBuiltInParameter()
        {
            var element = new Element(new ElementId(1));
            var param = element.GetParameter("-5");

            Assert.NotNull(param);
            Assert.Equal((BuiltInParameter)(-5), param.BuiltInParameter);
        }

        [Fact]
        public void GetParameter_PositiveInt_UsesId()
        {
            var element = new Element(new ElementId(1));
            element.Parameters.Add(new Parameter(new ElementId(7)));
            var param = element.GetParameter("7");

            Assert.NotNull(param);
            Assert.Equal(7L, param.Id.GetElementIdValue());
        }

        [Fact]
        public void GetParameter_Guid_UsesGuid()
        {
            var element = new Element(new ElementId(1));
            var guid = Guid.NewGuid();
            var param = element.GetParameter(guid.ToString());

            Assert.NotNull(param);
            Assert.Equal(guid, param.Guid);
        }

        [Fact]
        public void GetParameter_Name_UsesLookup()
        {
            var element = new Element(new ElementId(1));
            var param = element.GetParameter("Foo");

            Assert.NotNull(param);
            Assert.Equal("Foo", param.Name);
        }

        [Fact]
        public void GetParameter_FallsBackToElementType()
        {
            var doc = new Document();
            var type = new Element(doc, new ElementId(20));
            type.Parameters.Add(new Parameter(new ElementId(9)));
            doc.AddElement(type);

            var element = new Element(doc, new ElementId(2)) { TypeId = new ElementId(20) };

            var param = element.GetParameter("9");

            Assert.NotNull(param);
            Assert.Equal(9L, param.Id.GetElementIdValue());
            Assert.Same(type.Parameters[0], param);
        }
    }
}
