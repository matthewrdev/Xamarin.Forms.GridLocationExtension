# Xamarin.Forms Grid Markup Extension

**🚨🚨🚨Experimental: Not recommended for production apps🚨🚨🚨**.

Specify the grid locations by name for simpler, more maintainable XAML.

See:

 * [GridLocationExtension](GridLocationMarkupExtension/GridLocationExtension.cs)
 * [GridSpanExtension](GridLocationMarkupExtension/GridSpanExtension.cs)

**Before**

```
<Label Grid.Row="0" />
```

**After**

```
<Label Grid.Row="{local:GridLocation titleRow}"/>
```

## The Problem With Grids In Xamarin.Forms

Grids, while powerful, have one fundamental drawback; we place controls using 0-based indices. Consider the following code:

```
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="2"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>

    <Label Grid.Row="0" /> <!-- Title label -->
    <ContentView Grid.Row="1" /> <!-- Content Divider-->
    <StackLayout Grid.Row="2"/> <!-- Content -->
    <ActivityIndicator Grid.RowSpan="3" /> <!-- Loading Indicator that covers all rows -->

</Grid>
```

Each time we place a control in the grid, we do so using a 0-based location. This has a few problems:

 * Firstly, we can accidentally use an index not declared in the `RowDefinitions/ColumnDefinitions` of the grid, creating a rendering bug.
 * Secondly, if we add or remove a row/column, we now need to update the `RowDefinitions/ColumnDefinitions` and all affected indices in the grid.
 * Lastly, it is difficult to tell if we have correctly placed our control in the grid when looking at syntax such as `Grid.Row="1"`. There is a significant amount of thinking required to validate this location.

To solve these issues, this repository introduces two new markup extensions that allow grid locations to be referenced by name:

 * Use the `GridLocationExtension` to lookup the index of a named row or column in XAML: `Grid.Row={local:GridLocation namedRow}`.
 * Use the `GridSpanExtension` to calculate the span between two named rows or columns in XAML: `Grid.Row={local:GridSpan From=startRow, To=endRow}`.

These extensions require that each row and column definition includes an `x:Name` attribute to expose it to the extension. For example: `<RowDefinition x:Name="contentRow" Height=Auto/>`.

Here is an example these extensions applied to the previous code sample:

```
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition x:Name="titleRow"  Height="Auto"/>
        <RowDefinition x:Name="dividerRow" Height="2"/>
        <RowDefinition x:Name="contentRow" Height="*"/>
    </Grid.RowDefinitions>

    <Label Grid.Row="{local:GridLocation titleRow}" /> <!-- Title label -->
    <ContentView Grid.Row="{local:GridLocation dividerRow}" /> <!-- Content Divider-->
    <StackLayout Grid.Row="{local:GridLocation contentRow}"/> <!-- Content -->
    <ActivityIndicator Grid.RowSpan="{local:GridSpan From=titleRow, To=contentRow}" /> <!-- Loading Indicator that covers all rows -->

</Grid>
```

There are a few benefits here:

 * Each row/column definition is now documented via the `x:Name` attribute.
 * As rows/columns are now resolved at runtime via the `GridLocation` extension, we can freely move and delete rows/columns without needing to readjust indices and spans.
 * We can use `GridSpan` extensions `From` and `To` to easily calculate the correct spans through names. This reduces code complexity and makes it very clear what the intended behaviour of a span is.

If you like this work and would like to see it continue, please raise an issue to start a discussion. 😊

## Disclaimer

While stable and tested, it is **not** recommended that you use this code in your production apps for the following reasons:

 * These extensions are
 * The extensions have no error logging to assist you in diagnosing runtime issues.
 * The extensions use reflection to perform the location and span calculations. This may have adverse runtime performance impacts.
