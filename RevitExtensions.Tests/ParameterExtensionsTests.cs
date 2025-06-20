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

            var value = element.GetParameterValue("5");

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

            var value = element.GetParameterValue<long>("7");

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

            element.SetParameterValue("10", 5);

            Assert.Equal(5, parameter.AsInteger());
        }

        [Fact]
        public void Element_TrySetParameterValue_NotFound_ReturnsFalse()
        {
            var element = new Element(new ElementId(1));

            var result = element.TrySetParameterValue("42", 1, out var reason);

            Assert.False(result);
            Assert.Equal("Parameter not found.", reason);
        }

        [Fact]
        public void Element_SetParameterValue_NotFound_Throws()
        {
            var element = new Element(new ElementId(1));

            var ex = Assert.Throws<InvalidOperationException>(() => element.SetParameterValue("99", 2));
            Assert.Equal("Parameter not found.", ex.Message);
        }
    }
}
