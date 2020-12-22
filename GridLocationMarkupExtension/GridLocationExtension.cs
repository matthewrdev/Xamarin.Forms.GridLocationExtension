using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GridLocationMarkupExtension.GridMarkupExtensionHelper;

namespace GridLocationMarkupExtension
{
    [ContentProperty(nameof(Name))]
    public class GridLocationExtension : IMarkupExtension
    {
        /// <summary>
        /// The name of the row or column definition with an x:Name applied to lookup.
        /// </summary>
        public string Name { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService<IProvideValueTarget>();
            var referenceProvider = serviceProvider.GetService<IReferenceProvider>();

            if (provideValueTarget is null || referenceProvider is null)
            {
                return DefaultLocationValue;
            }

            if (string.IsNullOrEmpty(Name))
            {
                return DefaultLocationValue;
            }

            var grid = GetParentGrid(provideValueTarget);
            if (grid is null)
            {
                return DefaultLocationValue;
            }

            var targetProperty = provideValueTarget.TargetProperty as BindableProperty;
            if (!IsLocationProperty(targetProperty))
            {
                return DefaultLocationValue;
            }

            if (targetProperty.PropertyName == Grid.ColumnProperty.PropertyName)
            {
                return CalculateLocation(Name, grid.ColumnDefinitions, referenceProvider);
            }
            else if (targetProperty.PropertyName == Grid.RowProperty.PropertyName)
            {
                return CalculateLocation(Name, grid.RowDefinitions, referenceProvider);
            }

            return DefaultLocationValue;
        }
    }
}
