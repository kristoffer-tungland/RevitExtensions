using Autodesk.Revit.DB;
using RevitExtensions;
using Xunit;

namespace RevitExtensions.Tests
{
    public class DocumentExtensionsTests
    {
        [Fact]
        public void InstancesOf_Type_ReturnsCollectorWithDocumentAndType()
        {
            var doc = new Document();
            var collector = doc.InstancesOf<Wall>();

            Assert.Same(doc, collector.Document);
            Assert.Equal(typeof(Wall), collector.FilterType);
            Assert.True(collector.ExcludesElementTypes);
        }

        [Fact]
        public void TypesOf_Type_ReturnsCollectorWithDocumentAndType()
        {
            var doc = new Document();
            var collector = doc.TypesOf<Wall>();

            Assert.Same(doc, collector.Document);
            Assert.Equal(typeof(Wall), collector.FilterType);
            Assert.True(collector.OnlyElementTypes);
        }

        [Fact]
        public void InstancesOf_Category_ReturnsCollectorWithDocumentAndCategory()
        {
            var doc = new Document();
            var collector = doc.InstancesOf(BuiltInCategory.GenericModel);

            Assert.Same(doc, collector.Document);
            Assert.Equal(BuiltInCategory.GenericModel, collector.Category);
            Assert.True(collector.ExcludesElementTypes);
        }

        [Fact]
        public void TypesOf_Category_ReturnsCollectorWithDocumentAndCategory()
        {
            var doc = new Document();
            var collector = doc.TypesOf(BuiltInCategory.GenericModel);

            Assert.Same(doc, collector.Document);
            Assert.Equal(BuiltInCategory.GenericModel, collector.Category);
            Assert.True(collector.OnlyElementTypes);
        }

        [Fact]
        public void InstancesOf_Categories_ReturnsCollectorWithDocumentAndCategories()
        {
            var doc = new Document();
            var cats = new[] { BuiltInCategory.GenericModel, BuiltInCategory.GenericModel };
            var collector = doc.InstancesOf(cats);

            Assert.Same(doc, collector.Document);
            Assert.Equal(cats, collector.Categories);
            Assert.True(collector.ExcludesElementTypes);
        }

        [Fact]
        public void TypesOf_Categories_ReturnsCollectorWithDocumentAndCategories()
        {
            var doc = new Document();
            var cats = new[] { BuiltInCategory.GenericModel, BuiltInCategory.GenericModel };
            var collector = doc.TypesOf(cats);

            Assert.Same(doc, collector.Document);
            Assert.Equal(cats, collector.Categories);
            Assert.True(collector.OnlyElementTypes);
        }
    }
}
