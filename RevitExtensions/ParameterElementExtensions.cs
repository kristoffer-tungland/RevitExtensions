using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    internal static class ParameterElementExtensions
    {
        extension (ParameterElement element)
        {
            public Definition SafeDefinition => element.GetDefinition();

            public Guid? SafeGuid
            {
                get
                {
                    if (element is SharedParameterElement shared)
                    {
                        return shared.GuidValue;
                    }
                    return null;
                }
            }

            public bool GetIsInstanceSafe(Document doc)
            {
                var binding = doc.ParameterBindings.get_Item(element.SafeDefinition) as ElementBinding;
                return binding is InstanceBinding;
            }

            public IEnumerable<BuiltInCategory> GetCategoriesSafe(Document doc)
            {
                var binding = doc.ParameterBindings.get_Item(element.SafeDefinition) as ElementBinding;
                if (binding != null)
                {
                    foreach (Category c in binding.Categories)
                        yield return (BuiltInCategory)c.Id.Value;
                }
            }
        }
    }
}
