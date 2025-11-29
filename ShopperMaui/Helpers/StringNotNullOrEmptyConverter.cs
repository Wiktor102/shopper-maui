using System.Globalization;

namespace ShopperMaui.Helpers;

public class StringNotNullOrEmptyConverter : IValueConverter {
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> value is string text && !string.IsNullOrWhiteSpace(text);

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> throw new NotSupportedException();
}
