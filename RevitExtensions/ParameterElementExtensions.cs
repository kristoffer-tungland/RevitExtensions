using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    internal static class ParameterElementExtensions
    {
        public static Definition GetDefinitionSafe(this ParameterElement element)
        {
            return element.GetDefinition();
        }

        public static Guid? GetGuidSafe(this ParameterElement element)
        {
            if (element is SharedParameterElement shared)
            {
                return shared.GuidValue;
            }
            return null;
        }

        public static bool GetIsInstanceSafe(this ParameterElement element, Document doc)
        {
            var binding = doc.ParameterBindings.get_Item(element.GetDefinitionSafe()) as ElementBinding;
            return binding is InstanceBinding;
        }

        public static IEnumerable<BuiltInCategory> GetCategoriesSafe(this ParameterElement element, Document doc)
        {
            var binding = doc.ParameterBindings.get_Item(element.GetDefinitionSafe()) as ElementBinding;
            if (binding != null)
            {
                foreach (Category c in binding.Categories)
                    yield return (BuiltInCategory)c.Id.GetElementIdValue();
            }
        }
    }
}
