using System.Globalization;

namespace ShopperMaui.Helpers;

public class TabBackgroundConverter : IValueConverter {
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is int selectedIndex && parameter is string paramStr && int.TryParse(paramStr, out var tabIndex)) {
			var isSelected = selectedIndex == tabIndex;
			return isSelected
				? Application.Current?.RequestedTheme == AppTheme.Dark
					? Color.FromArgb("#3D3D3D")
					: Colors.White
				: Colors.Transparent;
		}

		return Colors.Transparent;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		throw new NotImplementedException();
	}
}

public class TabVisibilityConverter : IValueConverter {
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is int selectedIndex && parameter is string paramStr && int.TryParse(paramStr, out var tabIndex)) {
			return selectedIndex == tabIndex;
		}

		return false;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		throw new NotImplementedException();
	}
}
