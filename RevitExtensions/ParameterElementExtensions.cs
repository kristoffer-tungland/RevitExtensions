using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    internal static class ParameterElementExtensions
    {
        public static Definition GetDefinitionSafe(this ParameterElement element)
        {
#if REVIT2019_OR_LESS
            return element.GetDefinition();
#else
            return element.Definition;
#endif
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
#if REVIT2019_OR_LESS
            var binding = doc.ParameterBindings.get_Item(element.GetDefinitionSafe()) as ElementBinding;
            return binding is InstanceBinding;
#else
            return element.IsInstance;
#endif
        }

        public static IEnumerable<BuiltInCategory> GetCategoriesSafe(this ParameterElement element, Document doc)
        {
#if REVIT2019_OR_LESS
            var binding = doc.ParameterBindings.get_Item(element.GetDefinitionSafe()) as ElementBinding;
            if (binding != null)
            {
                foreach (Category c in binding.Categories)
                    yield return (BuiltInCategory)c.Id.GetElementIdValue();
            }
#else
            foreach (var bic in element.Categories)
                yield return bic;
#endif
        }
    }
}
