using System;
using Autodesk.Revit.DB;
using RevitExtensions.Models;
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

        [Fact]
        public void ToIdentifier_Guid_ReturnsIdentifierWithGuid()
        {
            var guid = Guid.NewGuid();
            var parameter = new Parameter(guid);

            var id = parameter.ToIdentifier();

            Assert.Equal(guid, id.Guid);
            Assert.Equal(guid.ToString(), parameter.ToIdentifier().ToStableRepresentation());
        }

        [Fact]
        public void ToIdentifier_BuiltInParameter_ReturnsIdentifier()
        {
            var parameter = new Parameter((BuiltInParameter)(-10));

            var id = parameter.ToIdentifier();

            Assert.Equal((BuiltInParameter)(-10), id.BuiltInParameter);
            Assert.Equal(parameter.Definition.Name, id.Name);
            Assert.Equal("-10;" + parameter.Definition.Name, parameter.ToIdentifier().ToStableRepresentation());
        }

        [Fact]
        public void ToIdentifier_Name_ReturnsIdentifier()
        {
            var parameter = new Parameter("Foo");

            var id = parameter.ToIdentifier();

            Assert.Equal("Foo", id.Name);
            Assert.Equal("Foo", parameter.ToIdentifier().ToStableRepresentation());
        }

        [Fact]
        public void ToIdentifier_Id_ReturnsIdentifier()
        {
            var parameter = new Parameter(new ElementId(5));

            var id = parameter.ToIdentifier();

            Assert.Equal(5L, id.Id);
            Assert.Equal("5", parameter.ToIdentifier().ToStableRepresentation());
        }

        [Fact]
        public void ToIdentifier_GuidWithName_IncludesName()
        {
            var guid = Guid.NewGuid();
            var parameter = new Parameter(guid);
            parameter.Definition.Name = "Foo";

            var id = parameter.ToIdentifier();

            Assert.Equal(guid, id.Guid);
            Assert.Equal("Foo", id.Name);
        }
    }
}
