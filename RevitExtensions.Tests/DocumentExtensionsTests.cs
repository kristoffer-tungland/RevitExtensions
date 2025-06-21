using System;
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

        [Fact]
        public void StartTransaction_StartsAndReturnsTransaction()
        {
            var doc = new Document();

            using var t = doc.StartTransaction("Foo");

            Assert.Same(doc, t.Document);
            Assert.Equal("Foo", t.Name);
            Assert.True(t.IsStarted);
        }

        [Fact]
        public void CommitAndEnsure_Committed_DoesNotThrow()
        {
            var doc = new Document();
            using var t = doc.StartTransaction("Foo");

            t.CommitAndEnsure();

            Assert.False(t.IsStarted);
        }

        [Fact]
        public void CommitAndEnsure_Failed_Throws()
        {
            var doc = new Document();
            using var t = new Transaction(doc, "Foo");

            var ex = Assert.Throws<InvalidOperationException>(() => t.CommitAndEnsure());
            Assert.Equal("Failed to commit transaction.", ex.Message);
        }

        [Fact]
        public void StartTransactionGroup_StartsAndReturnsGroup()
        {
            var doc = new Document();

            using var g = doc.StartTransactionGroup("Foo");

            Assert.Same(doc, g.Document);
            Assert.Equal("Foo", g.Name);
            Assert.True(g.IsStarted);
        }

        [Fact]
        public void AssimilateAndEnsure_Committed_DoesNotThrow()
        {
            var doc = new Document();
            using var g = doc.StartTransactionGroup("Foo");

            g.AssimilateAndEnsure();

            Assert.False(g.IsStarted);
        }

        [Fact]
        public void AssimilateAndEnsure_Failed_Throws()
        {
            var doc = new Document();
            using var g = new TransactionGroup(doc, "Foo");

            var ex = Assert.Throws<InvalidOperationException>(() => g.AssimilateAndEnsure());
            Assert.Equal("Failed to assimilate transaction group.", ex.Message);
        }

        [Fact]
        public void StartSubTransaction_StartsAndReturnsSubTransaction()
        {
            var doc = new Document();

            using var sub = doc.StartSubTransaction();

            Assert.Same(doc, sub.Document);
            Assert.True(sub.IsStarted);
        }

        [Fact]
        public void CommitSubTransactionAndEnsure_Committed_DoesNotThrow()
        {
            var doc = new Document();
            using var sub = doc.StartSubTransaction();

            sub.CommitAndEnsure();

            Assert.False(sub.IsStarted);
        }

        [Fact]
        public void CommitSubTransactionAndEnsure_Failed_Throws()
        {
            var doc = new Document();
            using var sub = new SubTransaction(doc);

            var ex = Assert.Throws<InvalidOperationException>(() => sub.CommitAndEnsure());
            Assert.Equal("Failed to commit subtransaction.", ex.Message);
        }
    }
}
