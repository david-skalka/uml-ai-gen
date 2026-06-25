using System.Globalization;
using Avalonia.Data.Converters;

namespace TodoApp.Converters;

public sealed class DateTimeOffsetFormatConverter : IValueConverter
{
    public static readonly DateTimeOffsetFormatConverter Instance = new();

    private const string Format = "yyyy-MM-dd HH:mm";

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        ((DateTimeOffset)value!).ToString(Format, CultureInfo.InvariantCulture);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        DateTimeOffset.Parse((string)value!, CultureInfo.InvariantCulture);
}
