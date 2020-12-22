using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GridLocationMarkupExtension.GridMarkupExtensionHelper;

namespace GridLocationMarkupExtension
{
    [ContentProperty(nameof(From))]
    public class GridSpanExtension : IMarkupExtension
    {
        public string From { get; set; }

        public string To { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService<IProvideValueTarget>();
            var referenceProvider = serviceProvider.GetService<IReferenceProvider>();

            if (provideValueTarget is null || referenceProvider is null)
            {
                return DefaultSpanValue;
            }

            if (string.IsNullOrEmpty(From) || string.IsNullOrEmpty(To))
            {
                return DefaultSpanValue;
            }

            var grid = GetParentGrid(provideValueTarget);
            if (grid is null)
            {
                return DefaultSpanValue;
            }

            var targetProperty = provideValueTarget.TargetProperty as BindableProperty;
            if (!IsSpanProperty(targetProperty))
            {
                return DefaultSpanValue;
            }

            if (targetProperty.PropertyName == Grid.ColumnSpanProperty.PropertyName)
            {
                return CalculateSpan(From, To, grid.ColumnDefinitions, referenceProvider);
            }
            else if (targetProperty.PropertyName == Grid.RowSpanProperty.PropertyName)
            {
                return CalculateSpan(From, To, grid.RowDefinitions, referenceProvider);
            }

            return DefaultSpanValue;
        }
    }
}
