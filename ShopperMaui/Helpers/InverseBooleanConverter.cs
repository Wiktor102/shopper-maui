using System.Globalization;
using Microsoft.Maui.Controls;

namespace ShopperMaui.Helpers;

public class InverseBooleanConverter : IValueConverter {
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is bool booleanValue) {
			return !booleanValue;
		}

		return true;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is bool booleanValue) {
			return !booleanValue;
		}

		return false;
	}
}
