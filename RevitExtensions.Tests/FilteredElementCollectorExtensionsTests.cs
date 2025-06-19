using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitExtensions;
using Xunit;

namespace RevitExtensions.Tests
{
    public class FilteredElementCollectorExtensionsTests
    {
        [Fact]
        public void ForEach_InvokesActionForEachElement()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc)
                .InstancesOf<Wall>();

            var e1 = new Wall(new ElementId(1));
            var e2 = new Wall(new ElementId(2));
            collector.AddElement(e1);
            collector.AddElement(e2);

            var results = new List<Element>();
            collector.ForEach(results.Add);

            Assert.Equal(new[] { e1, e2 }, results);
            Assert.True(e1.IsDisposed);
            Assert.True(e2.IsDisposed);
        }

        [Fact]
        public void InstancesOf_MultiCategories_FiltersCollector()
        {
            var doc = new Document();
            var cats = new[] { BuiltInCategory.GenericModel, BuiltInCategory.GenericModel };
            var collector = new FilteredElementCollector(doc)
                .InstancesOf(cats);

            Assert.Equal(cats, collector.Categories);
            Assert.True(collector.ExcludesElementTypes);
        }

        [Fact]
        public void TypesOf_MultiCategories_FiltersCollector()
        {
            var doc = new Document();
            var cats = new[] { BuiltInCategory.GenericModel, BuiltInCategory.GenericModel };
            var collector = new FilteredElementCollector(doc)
                .TypesOf(cats);

            Assert.Equal(cats, collector.Categories);
            Assert.True(collector.OnlyElementTypes);
        }
    }
}

