namespace ShopperMaui.Services;

public class DialogService : IDialogService {
	public Task ShowAlertAsync(string title, string message)
		=> MainThreadInvoke(() => {
			var page = GetActivePage();
			return page is null
				? Task.CompletedTask
				: page.DisplayAlert(title, message, "OK");
		});

	public Task<bool> ShowConfirmAsync(string title, string message)
		=> MainThreadInvoke(async () => {
			var page = GetActivePage();
			return page is null
				? false
				: await page.DisplayAlert(title, message, "Tak", "Nie");
		});

	public Task<string?> ShowPromptAsync(string title, string message, string placeholder)
		=> MainThreadInvoke(async () => {
			var page = GetActivePage();
			return page is null
				? null
				: await page.DisplayPromptAsync(title, message, placeholder: placeholder);
		});

	private static Page? GetActivePage()
		=> Application.Current?.Windows.FirstOrDefault()?.Page;

	private static Task<T> MainThreadInvoke<T>(Func<Task<T>> action) {
		if (action is null) {
			throw new ArgumentNullException(nameof(action));
		}

		var tcs = new TaskCompletionSource<T>();
		MainThread.BeginInvokeOnMainThread(async () => {
			try {
				var result = await action();
				tcs.SetResult(result);
			} catch (Exception ex) {
				tcs.SetException(ex);
			}
		});

		return tcs.Task;
	}

	private static Task MainThreadInvoke(Func<Task> action) {
		if (action is null) {
			throw new ArgumentNullException(nameof(action));
		}

		var tcs = new TaskCompletionSource();
		MainThread.BeginInvokeOnMainThread(async () => {
			try {
				await action();
				tcs.SetResult();
			} catch (Exception ex) {
				tcs.SetException(ex);
			}
		});

		return tcs.Task;
	}
}
