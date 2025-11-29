using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopperMaui.Services;

public interface INavigationService
{
    void RegisterRoute<TViewModel>(string route);
    Task NavigateToAsync<TViewModel>(Dictionary<string, object>? parameters = null);
    Task GoBackAsync();
}
