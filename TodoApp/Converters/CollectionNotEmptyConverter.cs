using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;

namespace TodoApp.Converters;

public sealed class CollectionNotEmptyConverter : IValueConverter
{
    public static readonly CollectionNotEmptyConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is IEnumerable items && items.Cast<object>().Any();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
