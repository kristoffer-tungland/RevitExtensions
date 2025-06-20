using System;
using Autodesk.Revit.DB;
using RevitExtensions;
using Xunit;

namespace RevitExtensions.Tests
{
    public class ParameterIdentifierTests
    {
        [Fact]
        public void Parse_GuidString_SetsGuid()
        {
            var guid = Guid.NewGuid().ToString();

            var id = ParameterIdentifier.Parse(guid);

            Assert.Equal(Guid.Parse(guid), id.Guid);
            Assert.Null(id.BuiltInParameter);
            Assert.Null(id.Name);
            Assert.Null(id.Id);
            Assert.Equal(guid, id.ToStableRepresentation());
        }

        [Fact]
        public void Parse_NegativeInt_SetsBuiltInParameter()
        {
            var id = ParameterIdentifier.Parse("-42");

            Assert.Equal((BuiltInParameter)(-42), id.BuiltInParameter);
            Assert.Null(id.Guid);
            Assert.Null(id.Name);
            Assert.Null(id.Id);
            Assert.Equal("-42", id.ToStableRepresentation());
        }

        [Fact]
        public void Parse_PositiveInt_SetsId()
        {
            var id = ParameterIdentifier.Parse("15");

            Assert.Equal(15L, id.Id);
            Assert.Null(id.Guid);
            Assert.Null(id.BuiltInParameter);
            Assert.Null(id.Name);
            Assert.Equal("15", id.ToStableRepresentation());
        }

        [Fact]
        public void Parse_Name_SetsName()
        {
            var id = ParameterIdentifier.Parse("Foo");

            Assert.Equal("Foo", id.Name);
            Assert.Null(id.Guid);
            Assert.Null(id.BuiltInParameter);
            Assert.Null(id.Id);
            Assert.Equal("Foo", id.ToStableRepresentation());
        }
    }
}
