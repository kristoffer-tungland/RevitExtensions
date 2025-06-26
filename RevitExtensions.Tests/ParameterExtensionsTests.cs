using System;
using Autodesk.Revit.DB;
using RevitExtensions;
using RevitExtensions.Models;
using Xunit;

namespace RevitExtensions.Tests
{
    public class ParameterExtensionsTests
    {
        [Fact]
        public void GetParameter_NegativeInt_UsesBuiltInParameter()
        {
            var element = new Element(new ElementId(1));
            var expected = new Parameter((BuiltInParameter)(-5));
            element.Parameters.Add(expected);

            var param = element.GetParameter(ParameterIdentifier.Parse("-5"));

            Assert.Same(expected, param);
        }

        [Fact]
        public void GetParameter_PositiveInt_UsesId()
        {
            var element = new Element(new ElementId(1));
            element.Parameters.Add(new Parameter(new ElementId(7)));
            var param = element.GetParameter(ParameterIdentifier.Parse("7"));

            Assert.NotNull(param);
            Assert.Equal(7L, param.Id.GetElementIdValue());
        }

        [Fact]
        public void GetParameter_Guid_UsesGuid()
        {
            var element = new Element(new ElementId(1));
            var guid = Guid.NewGuid();
            var p = new Parameter(guid);
            element.Parameters.Add(p);
            var param = element.GetParameter(ParameterIdentifier.Parse(guid.ToString()));

            Assert.NotNull(param);
            Assert.Equal(guid, param.Guid);
        }

        [Fact]
        public void GetParameter_Name_UsesLookup()
        {
            var element = new Element(new ElementId(1));
            var expected = new Parameter("Foo");
            element.Parameters.Add(expected);

            var param = element.GetParameter(ParameterIdentifier.Parse("Foo"));

            Assert.Same(expected, param);
        }

        [Fact]
        public void GetParameter_FallsBackToElementType()
        {
            var doc = new Document();
            var type = new Element(doc, new ElementId(20));
            type.Parameters.Add(new Parameter(new ElementId(9)));
            doc.AddElement(type);

            var element = new Element(doc, new ElementId(2)) { TypeId = new ElementId(20) };

            var param = element.GetParameter(ParameterIdentifier.Parse("9"));

            Assert.NotNull(param);
            Assert.Equal(9L, param.Id.GetElementIdValue());
            Assert.Same(type.Parameters[0], param);
        }

        [Fact]
        public void LookupParameter_GuidNotFound_FallsBackToName()
        {
            var source = new Parameter(Guid.NewGuid()) { Definition = { Name = "Foo" } };
            var identifier = source.ToIdentifier();

            var element = new Element(new ElementId(3));
            element.Parameters.Add(new Parameter("Foo"));

            var param = element.LookupParameter(identifier);

            Assert.NotNull(param);
            Assert.Equal("Foo", param.Name);
        }

        [Fact]
        public void GetParameter_GuidNotFound_ReturnsNull()
        {
            var source = new Parameter(Guid.NewGuid()) { Definition = { Name = "Foo" } };
            var identifier = source.ToIdentifier();

            var element = new Element(new ElementId(6));
            element.Parameters.Add(new Parameter("Foo"));

            var param = element.GetParameter(identifier);

            Assert.Null(param);
        }

        [Fact]
        public void LookupParameter_IdNotFound_FallsBackToName()
        {
            var source = new Parameter(new ElementId(8)) { Definition = { Name = "Bar" } };
            var identifier = source.ToIdentifier();

            var element = new Element(new ElementId(4));
            element.Parameters.Add(new Parameter("Bar"));

            var param = element.LookupParameter(identifier);

            Assert.NotNull(param);
            Assert.Equal("Bar", param.Name);
        }

        [Fact]
        public void GetParameterValue_FromParameter_ReturnsStoredValue()
        {
            var parameter = new Parameter("A") { StorageType = StorageType.String };
            parameter.Set("foo");

            var value = parameter.GetParameterValue();

            Assert.Equal("foo", value);
        }

        [Fact]
        public void GetParameterValue_FromElementIdentifier_ReturnsStoredValue()
        {
            var element = new Element(new ElementId(1));
            var parameter = new Parameter(new ElementId(5)) { StorageType = StorageType.Integer };
            parameter.Set(42);
            element.Parameters.Add(parameter);

            var value = element.GetParameterValue(ParameterIdentifier.Parse("5"));

            Assert.Equal(42, value);
        }

        [Fact]
        public void GetParameterValue_Generic_ConvertsValue()
        {
            var parameter = new Parameter("B") { StorageType = StorageType.String };
            parameter.Set("123");

            var value = parameter.GetParameterValue<int>();

            Assert.Equal(123, value);
        }

        [Fact]
        public void Element_GetParameterValue_Generic_ConvertsValue()
        {
            var element = new Element(new ElementId(2));
            var parameter = new Parameter(new ElementId(7)) { StorageType = StorageType.ElementId };
            parameter.Set(new ElementId(7));
            element.Parameters.Add(parameter);

            var value = element.GetParameterValue<long>(ParameterIdentifier.Parse("7"));

            Assert.Equal(7L, value);
        }

        [Fact]
        public void SetParameterValue_SetsValue()
        {
            var parameter = new Parameter("A") { StorageType = StorageType.Double };

            parameter.SetParameterValue(3.5);

            Assert.Equal(3.5, parameter.AsDouble());
        }

        [Fact]
        public void TrySetParameterValue_ReadOnly_ReturnsFalseWithReason()
        {
            var parameter = new Parameter("A") { StorageType = StorageType.String, IsReadOnly = true };

            var result = parameter.TrySetParameterValue("bar", out var reason);

            Assert.False(result);
            Assert.Equal("Parameter is read-only.", reason);
        }

        [Fact]
        public void SetParameterValue_ReadOnly_Throws()
        {
            var parameter = new Parameter("A") { StorageType = StorageType.String, IsReadOnly = true };

            var ex = Assert.Throws<InvalidOperationException>(() => parameter.SetParameterValue("bar"));
            Assert.Equal("Parameter is read-only.", ex.Message);
        }

        [Fact]
        public void Element_SetParameterValue_UpdatesParameter()
        {
            var element = new Element(new ElementId(1));
            var parameter = new Parameter(new ElementId(10)) { StorageType = StorageType.Integer };
            element.Parameters.Add(parameter);

            element.SetParameterValue(ParameterIdentifier.Parse("10"), 5);

            Assert.Equal(5, parameter.AsInteger());
        }

        [Fact]
        public void Element_TrySetParameterValue_NotFound_ReturnsFalse()
        {
            var element = new Element(new ElementId(1));

            var result = element.TrySetParameterValue(ParameterIdentifier.Parse("42"), 1, out var reason);

            Assert.False(result);
            Assert.Equal("Parameter not found.", reason);
        }

        [Fact]
        public void Element_SetParameterValue_NotFound_Throws()
        {
            var element = new Element(new ElementId(1));

            var ex = Assert.Throws<InvalidOperationException>(() => element.SetParameterValue(ParameterIdentifier.Parse("99"), 2));
            Assert.Equal("Parameter not found.", ex.Message);
        }

        [Fact]
        public void LookupParameterValue_NullValue_TriesNextParameter()
        {
            var source = new Parameter(new ElementId(11)) { Definition = { Name = "Baz" }, StorageType = StorageType.String };
            // leave value null
            var identifier = source.ToIdentifier();

            var element = new Element(new ElementId(5));
            element.Parameters.Add(source);
            var second = new Parameter("Baz") { StorageType = StorageType.String };
            second.Set("value");
            element.Parameters.Add(second);

            var value = element.LookupParameterValue(identifier);

            Assert.Equal("value", value);
        }

        [Fact]
        public void GetParameterValue_NullValue_ReturnsNull()
        {
            var source = new Parameter(new ElementId(12)) { Definition = { Name = "Qux" }, StorageType = StorageType.String };
            var identifier = source.ToIdentifier();

            var element = new Element(new ElementId(6));
            element.Parameters.Add(source);
            element.Parameters.Add(new Parameter("Qux") { StorageType = StorageType.String });

            var value = element.GetParameterValue(identifier);

            Assert.Null(value);
        }

        [Fact]
        public void SetParameterValue_BoolToInteger_WritesZeroOrOne()
        {
            var parameter = new Parameter("B") { StorageType = StorageType.Integer };

            parameter.SetParameterValue(true);

            Assert.Equal(1, parameter.AsInteger());

            parameter.SetParameterValue(false);

            Assert.Equal(0, parameter.AsInteger());
        }

        [Fact]
        public void GetParameterValue_BoolFromInteger_ParsesBool()
        {
            var parameter = new Parameter("C") { StorageType = StorageType.Integer };
            parameter.Set(1);

            var value = parameter.GetParameterValue<bool>();

            Assert.True(value);
        }

        [Fact]
        public void SetParameterValue_DateTimeToString_WritesIso()
        {
            var dt = new DateTime(2024, 6, 20, 12, 0, 0, DateTimeKind.Utc);
            var parameter = new Parameter("D") { StorageType = StorageType.String };

            parameter.SetParameterValue(dt);

            Assert.Equal(dt.ToString("o"), parameter.AsString());
        }

        [Fact]
        public void GetParameterValue_DateTimeFromInteger_ParsesUnixSeconds()
        {
            var dt = new DateTime(2024, 6, 20, 12, 0, 0, DateTimeKind.Utc);
            var seconds = (int)new DateTimeOffset(dt).ToUnixTimeSeconds();
            var parameter = new Parameter("E") { StorageType = StorageType.Integer };
            parameter.Set(seconds);

            var value = parameter.GetParameterValue<DateTime>();

            Assert.Equal(dt, value);
        }
    }
}
