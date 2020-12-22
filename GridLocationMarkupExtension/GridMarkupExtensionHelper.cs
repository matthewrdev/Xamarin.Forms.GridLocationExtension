using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GridLocationMarkupExtension
{
    public static class GridMarkupExtensionHelper
    {
        public const int DefaultLocationValue = 0;

        public const int DefaultSpanValue = 1;

        static readonly object parentObjectsPropertyLock = new object();
        static PropertyInfo parentObjectsProperty;

        public static Grid GetParentGrid(IProvideValueTarget provideValueTarget)
        {
            try
            {
                var property = GetParentObjectsProperty(provideValueTarget);

                var parentObjects = (property.GetValue(provideValueTarget) as object[])?.ToList();
                if (parentObjects is null)
                {
                    return default;
                }

                if (!parentObjects.Contains(provideValueTarget.TargetObject))
                {
                    return default;
                }

                var targetIndex = parentObjects.IndexOf(provideValueTarget.TargetObject);

                var parentIndex = targetIndex + 1;

                if (parentIndex >= parentObjects.Count)
                {
                    return default;
                }

                return parentObjects[parentIndex] as Grid;
            }
            catch (Exception ex)
            {
                // TODO: Log?
                return default;
            }
        }

        static PropertyInfo GetParentObjectsProperty(IProvideValueTarget provideValueTarget)
        {
            lock (parentObjectsPropertyLock)
            {
                if (parentObjectsProperty == null)
                {
                    parentObjectsProperty = provideValueTarget.GetType().GetProperty("Xamarin.Forms.Xaml.IProvideParentValues.ParentObjects", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                }

                return parentObjectsProperty;
            }
        }

        public static int CalculateLocation<TDefinition>(string name, DefinitionCollection<TDefinition> definitions, IReferenceProvider referenceProvider) where TDefinition : class, IDefinition
        {
            if (definitions is null
                || definitions.Count == 0)
            {
                return DefaultLocationValue;
            }

            var definition = referenceProvider.FindByName(name) as TDefinition;
            if (definition is null
                || !definitions.Contains(definition))
            {
                return DefaultLocationValue;
            }

            return definitions.IndexOf(definition);
        }

        public static int CalculateSpan<TDefinition>(string from, string to, DefinitionCollection<TDefinition> definitions, IReferenceProvider referenceProvider) where TDefinition : class, IDefinition
        {
            var fromLocation = CalculateLocation(from, definitions, referenceProvider);
            var toLocation = CalculateLocation(to, definitions, referenceProvider);

            var span = toLocation - fromLocation;

            if (span <= 0)
            {
                span = DefaultSpanValue;
            }
            else
            {
                span += 1; // Adjustment to include ending element.
            }

            return span;
        }

        public static bool IsLocationProperty(BindableProperty targetProperty)
        {
            if (targetProperty is null)
            {
                return false;
            }

            return targetProperty.PropertyName == Grid.ColumnProperty.PropertyName
                   || targetProperty.PropertyName == Grid.RowProperty.PropertyName;
        }

        public static bool IsSpanProperty(BindableProperty targetProperty)
        {
            if (targetProperty is null)
            {
                return false;
            }

            return targetProperty.PropertyName == Grid.ColumnSpanProperty.PropertyName
                   || targetProperty.PropertyName == Grid.RowSpanProperty.PropertyName;
        }
    }
}
