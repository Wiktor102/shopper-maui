using Windows.Storage;
using Windows.Storage.Pickers;
using ShopperMaui.Services;
using WinRT.Interop;

namespace ShopperMaui.Platforms.Windows;

public class FileSaverService : IFileSaver {
	public async Task<string?> SaveAsync(
		string fileName,
		string content,
		CancellationToken cancellationToken = default) {
		var picker = new FileSavePicker {
			SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
			SuggestedFileName = fileName
		};

		// Define the file type choice
		picker.FileTypeChoices.Add("XML File", [".xml"]);

		// Initialize the picker with the current window handle
		var window = Application.Current?.Windows.FirstOrDefault();
		if (window?.Handler?.PlatformView is MauiWinUIWindow winuiWindow) {
			var hwnd = winuiWindow.WindowHandle;
			InitializeWithWindow.Initialize(picker, hwnd);
		}

		// Let the user pick a file location
		var file = await picker.PickSaveFileAsync();
		if (file is null) return null;

		// Write file content
		await FileIO.WriteTextAsync(file, content);
		return file.Path;
	}
}