using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitExtensions.Collectors;
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
        public void ForEach_Generic_SkipsNonMatchingElements()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var wall = new Wall(new ElementId(1));
            var other = new Element(new ElementId(2));
            collector.AddElement(wall);
            collector.AddElement(other);

            var results = new List<Wall>();
            collector.ForEach<Wall>(results.Add);

            Assert.Equal(new[] { wall }, results);
            Assert.True(wall.IsDisposed);
            Assert.True(other.IsDisposed);
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
        public void Instances_FiltersCollector()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc)
                .Instances();

            Assert.True(collector.ExcludesElementTypes);
        }

        [Fact]
        public void Types_FiltersCollector()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc)
                .Types();

            Assert.True(collector.OnlyElementTypes);
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

        [Fact]
        public void Where_Equals_FiltersByParameterValue()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(1));
            var p1 = new Parameter(new ElementId(10)) { StorageType = StorageType.String };
            p1.Definition.Name = "Foo";
            p1.Set("bar");
            e1.Parameters.Add(p1);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(2));
            var p2 = new Parameter(new ElementId(11)) { StorageType = StorageType.String };
            p2.Definition.Name = "Foo";
            p2.Set("baz");
            e2.Parameters.Add(p2);
            collector.AddElement(e2);

            var filtered = collector.Where(new ElementId(10), StringComparison.Equals, "bar");

            Assert.Same(collector, filtered);
            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }

        [Fact]
        public void Where_Contains_FiltersBySubstring()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(3));
            var p1 = new Parameter(new ElementId(12)) { StorageType = StorageType.String };
            p1.Definition.Name = "Foo";
            p1.Set("hello world");
            e1.Parameters.Add(p1);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(4));
            var p2 = new Parameter(new ElementId(13)) { StorageType = StorageType.String };
            p2.Definition.Name = "Foo";
            p2.Set("goodbye");
            e2.Parameters.Add(p2);
            collector.AddElement(e2);

            var filtered = collector.Where(new ElementId(12), StringComparison.Contains, "world");

            Assert.Same(collector, filtered);
            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }

        [Fact]
        public void Where_FiltersUsingParameterId()
        {
            var doc = new Document();
            doc.ParameterBindings.Add("Foo", new ElementId(20));
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(5));
            var p1 = new Parameter(new ElementId(20)) { StorageType = StorageType.String };
            p1.Definition.Name = "Other";
            p1.Set("bar");
            e1.Parameters.Add(p1);
            collector.AddElement(e1);

            var filtered = collector.Where(new ElementId(20), StringComparison.Equals, "bar");

            Assert.Same(collector, filtered);
            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }


        [Fact]
        public void Or_ReturnsElementsMatchingAnyRule()
        {
            var doc = new Document();
            doc.ParameterBindings.Add("Foo", new ElementId(40));
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(7));
            var p1 = new Parameter(new ElementId(40)) { StorageType = StorageType.String };
            p1.Definition.Name = "Foo";
            p1.Set("a");
            e1.Parameters.Add(p1);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(8));
            var p2 = new Parameter(new ElementId(40)) { StorageType = StorageType.String };
            p2.Definition.Name = "Foo";
            p2.Set("b");
            e2.Parameters.Add(p2);
            collector.AddElement(e2);

            var filtered = collector.WhereOr(
                (new ElementId(40), StringComparison.Equals, "a"),
                (new ElementId(40), StringComparison.Equals, "b"));

            Assert.Equal(new[] { e1, e2 }, new List<Element>(filtered));
        }

        [Fact]
        public void Or_List_ReturnsElementsMatchingAnyValue()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(80));
            var p1 = new Parameter(new ElementId(70)) { StorageType = StorageType.String };
            p1.Set("a");
            e1.Parameters.Add(p1);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(81));
            var p2 = new Parameter(new ElementId(70)) { StorageType = StorageType.String };
            p2.Set("b");
            e2.Parameters.Add(p2);
            collector.AddElement(e2);

            var filtered = collector.WhereOr(new ElementId(70), StringComparison.Equals, new[] { "a", "b" });

            Assert.Equal(new[] { e1, e2 }, new List<Element>(filtered));
        }

        [Fact]
        public void And_ReturnsElementsMatchingAllRules()
        {
            var doc = new Document();
            doc.ParameterBindings.Add("A", new ElementId(50));
            doc.ParameterBindings.Add("B", new ElementId(51));
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(9));
            var pa1 = new Parameter(new ElementId(50)) { StorageType = StorageType.String };
            pa1.Definition.Name = "A";
            pa1.Set("x");
            e1.Parameters.Add(pa1);
            var pb1 = new Parameter(new ElementId(51)) { StorageType = StorageType.String };
            pb1.Definition.Name = "B";
            pb1.Set("y");
            e1.Parameters.Add(pb1);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(10));
            var pa2 = new Parameter(new ElementId(50)) { StorageType = StorageType.String };
            pa2.Definition.Name = "A";
            pa2.Set("x");
            e2.Parameters.Add(pa2);
            collector.AddElement(e2);

            var filtered = collector.WhereAnd(
                (new ElementId(50), StringComparison.Equals, "x"),
                (new ElementId(51), StringComparison.Equals, "y"));

            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }

        [Fact]
        public void And_List_ReturnsElementsMatchingAllValues()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(82));
            var p1 = new Parameter(new ElementId(71)) { StorageType = StorageType.String };
            p1.Set("abc");
            e1.Parameters.Add(p1);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(83));
            var p2 = new Parameter(new ElementId(71)) { StorageType = StorageType.String };
            p2.Set("a");
            e2.Parameters.Add(p2);
            collector.AddElement(e2);

            var filtered = collector.WhereAnd(new ElementId(71), StringComparison.Contains, new[] { "a", "b" });

            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }

        [Fact]
        public void Where_IntComparison_FiltersByValue()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(30));
            var p1 = new Parameter(new ElementId(60)) { StorageType = StorageType.Integer };
            p1.Set(5);
            e1.Parameters.Add(p1);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(31));
            var p2 = new Parameter(new ElementId(60)) { StorageType = StorageType.Integer };
            p2.Set(3);
            e2.Parameters.Add(p2);
            collector.AddElement(e2);

            var filtered = collector.Where(new ElementId(60), Comparison.GreaterOrEqual, 4);

            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }

        [Fact]
        public void Where_DoubleComparison_FiltersByValue()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(32));
            var p1 = new Parameter(new ElementId(61)) { StorageType = StorageType.Double };
            p1.Set(10.0);
            e1.Parameters.Add(p1);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(33));
            var p2 = new Parameter(new ElementId(61)) { StorageType = StorageType.Double };
            p2.Set(20.0);
            e2.Parameters.Add(p2);
            collector.AddElement(e2);

            var filtered = collector.Where(new ElementId(61), Comparison.Less, 15.0);

            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }

        [Fact]
        public void Where_ElementIdComparison_FiltersByValue()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(34));
            var p1 = new Parameter(new ElementId(62)) { StorageType = StorageType.ElementId };
            p1.Set(new ElementId(100));
            e1.Parameters.Add(p1);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(35));
            var p2 = new Parameter(new ElementId(62)) { StorageType = StorageType.ElementId };
            p2.Set(new ElementId(200));
            e2.Parameters.Add(p2);
            collector.AddElement(e2);

            var filtered = collector.Where(new ElementId(62), Comparison.NotEquals, new ElementId(100));

            Assert.Equal(new[] { e2 }, new List<Element>(filtered));
        }

        [Fact]
        public void WherePasses_ComposesComplexFilters()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(40));
            var p1a = new Parameter(new ElementId(70)) { StorageType = StorageType.String };
            p1a.Set("a");
            e1.Parameters.Add(p1a);
            var p1b = new Parameter(new ElementId(71)) { StorageType = StorageType.String };
            p1b.Set("c");
            e1.Parameters.Add(p1b);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(41));
            var p2a = new Parameter(new ElementId(70)) { StorageType = StorageType.String };
            p2a.Set("b");
            e2.Parameters.Add(p2a);
            var p2b = new Parameter(new ElementId(71)) { StorageType = StorageType.String };
            p2b.Set("d");
            e2.Parameters.Add(p2b);
            collector.AddElement(e2);

            var set = new ParameterFilterSetBuilder()
                .OrSet(
                    (new ElementId(70), StringComparison.Equals, "a"),
                    (new ElementId(70), StringComparison.Equals, "b"))
                .Rule(new ElementId(71), StringComparison.Equals, "c")
                .Build();

            var filtered = collector.WherePasses(set);

            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }

        [Fact]
        public void Where_BuilderOverload_ComposesFilters()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(42));
            var p1a = new Parameter(new ElementId(72)) { StorageType = StorageType.String };
            p1a.Set("a");
            e1.Parameters.Add(p1a);
            var p1b = new Parameter(new ElementId(73)) { StorageType = StorageType.String };
            p1b.Set("c");
            e1.Parameters.Add(p1b);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(43));
            var p2a = new Parameter(new ElementId(72)) { StorageType = StorageType.String };
            p2a.Set("b");
            e2.Parameters.Add(p2a);
            var p2b = new Parameter(new ElementId(73)) { StorageType = StorageType.String };
            p2b.Set("d");
            e2.Parameters.Add(p2b);
            collector.AddElement(e2);

            var filtered = collector.Where(b => b
                .OrSet(
                    (new ElementId(72), StringComparison.Equals, "a"),
                    (new ElementId(72), StringComparison.Equals, "b"))
                .Rule(new ElementId(73), StringComparison.Equals, "c"));

            Assert.Same(collector, filtered);
            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }

        [Fact]
        public void Where_BuilderNestedSets_ComposesFilters()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(60));
            var p1a = new Parameter(new ElementId(80)) { StorageType = StorageType.String };
            p1a.Set("A");
            e1.Parameters.Add(p1a);
            var p1b = new Parameter(new ElementId(81)) { StorageType = StorageType.String };
            p1b.Set("B");
            e1.Parameters.Add(p1b);
            var p1c = new Parameter(new ElementId(82)) { StorageType = StorageType.String };
            p1c.Set("C");
            e1.Parameters.Add(p1c);
            var p1d = new Parameter(new ElementId(83)) { StorageType = StorageType.String };
            p1d.Set("D");
            e1.Parameters.Add(p1d);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(61));
            var p2a = new Parameter(new ElementId(80)) { StorageType = StorageType.String };
            p2a.Set("A");
            e2.Parameters.Add(p2a);
            var p2b = new Parameter(new ElementId(81)) { StorageType = StorageType.String };
            p2b.Set("X");
            e2.Parameters.Add(p2b);
            var p2c = new Parameter(new ElementId(82)) { StorageType = StorageType.String };
            p2c.Set("Y");
            e2.Parameters.Add(p2c);
            var p2d = new Parameter(new ElementId(83)) { StorageType = StorageType.String };
            p2d.Set("E");
            e2.Parameters.Add(p2d);
            collector.AddElement(e2);

            var filtered = collector.Where(b => b
                .OrSet(or => or
                    .Rule(new ElementId(80), StringComparison.Equals, "A")
                    .AndSet(and => and
                        .Rule(new ElementId(81), StringComparison.Equals, "B")
                        .Rule(new ElementId(82), StringComparison.Equals, "C")))
                .Rule(new ElementId(83), StringComparison.Equals, "D"));

            Assert.Same(collector, filtered);
            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }

        [Fact]
        public void Where_BuiltInParameter_UsesEnum()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            BuiltInParameter bip = (BuiltInParameter)(-100);

            var e1 = new Element(new ElementId(50));
            var p1 = new Parameter(bip) { StorageType = StorageType.String };
            p1.Set("foo");
            e1.Parameters.Add(p1);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(51));
            var p2 = new Parameter(bip) { StorageType = StorageType.String };
            p2.Set("bar");
            e2.Parameters.Add(p2);
            collector.AddElement(e2);

            var filtered = collector.Where(bip, StringComparison.Equals, "foo");

            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }

        [Fact]
        public void Where_Wildcard_BeginsAndEndsWith()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(70));
            var p1 = new Parameter(new ElementId(90)) { StorageType = StorageType.String };
            p1.Set("foo123bar");
            e1.Parameters.Add(p1);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(71));
            var p2 = new Parameter(new ElementId(90)) { StorageType = StorageType.String };
            p2.Set("foo999baz");
            e2.Parameters.Add(p2);
            collector.AddElement(e2);

            var filtered = collector.Where(new ElementId(90), StringComparison.Wildcard, "foo*bar");

            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }

        [Fact]
        public void Where_Wildcard_MultipleSegments()
        {
            var doc = new Document();
            var collector = new FilteredElementCollector(doc);

            var e1 = new Element(new ElementId(72));
            var p1 = new Parameter(new ElementId(91)) { StorageType = StorageType.String };
            p1.Set("foo-this-bar");
            e1.Parameters.Add(p1);
            collector.AddElement(e1);

            var e2 = new Element(new ElementId(73));
            var p2 = new Parameter(new ElementId(91)) { StorageType = StorageType.String };
            p2.Set("foo-nope-bar");
            e2.Parameters.Add(p2);
            collector.AddElement(e2);

            var filtered = collector.Where(new ElementId(91), StringComparison.Wildcard, "foo*is*bar");

            Assert.Equal(new[] { e1 }, new List<Element>(filtered));
        }

        [Fact]
        public void Where_Name_FiltersUsingAllParameterIds()
        {
            BuiltInParameterCollector.ClearCache();
            BuiltInParameterCollector.FileSystem = new InMemoryFileSystem();

            var doc = new Document();
            doc.Application.VersionNumber = "2026";
            var cat = new Category(BuiltInCategory.GenericModel);
            doc.Settings.Categories.Add(cat);

            var collector = new FilteredElementCollector(doc);

            var element = new Element(doc, new ElementId(74));
            var p1 = new Parameter((BuiltInParameter)(-3)) { StorageType = StorageType.String };
            p1.Definition.Name = "Bar";
            p1.Set("foo");
            element.Parameters.Add(p1);

            var p2 = new Parameter(new ElementId(102)) { StorageType = StorageType.String };
            p2.Definition.Name = "Bar";
            p2.Set("foo");
            element.Parameters.Add(p2);
            collector.AddElement(element);

            var pe = new ParameterElement(new ElementId(102))
            {
                Definition = new Definition { Name = "Bar" },
                IsInstance = true
            };
            pe.Categories.Add(BuiltInCategory.GenericModel);
            doc.AddElement(pe);

            doc.AddElement(element);

            var filtered = collector.Where(doc, "Bar", StringComparison.Equals, "foo");

            Assert.Equal(new[] { element }, new List<Element>(filtered));
        }
    }
}
