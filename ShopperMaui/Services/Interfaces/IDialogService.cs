namespace ShopperMaui.Services.Interfaces;

public interface IDialogService {
	Task ShowAlertAsync(string title, string message);
	Task<bool> ShowConfirmAsync(string title, string message);
	Task<string?> ShowPromptAsync(string title, string message, string placeholder);
	Task<string?> ShowSelectionAsync(string title, IEnumerable<string> options, string cancelButtonText);
	Task ShowToastAsync(string message);
}
