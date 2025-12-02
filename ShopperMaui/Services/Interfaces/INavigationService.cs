namespace ShopperMaui.Services.Interfaces;

public interface INavigationService {
	void RegisterRoute<TViewModel>(string route);
	Task NavigateToAsync<TViewModel>(Dictionary<string, object>? parameters = null);
	Task GoBackAsync();
}
