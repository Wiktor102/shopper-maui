namespace ShopperMaui.Services;

public interface IDialogService {
	Task ShowAlertAsync(string title, string message);
	Task<bool> ShowConfirmAsync(string title, string message);
	Task<string?> ShowPromptAsync(string title, string message, string placeholder);
}
