using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace ShopperMaui.Services;

public class NavigationService : INavigationService
{
    private readonly Dictionary<Type, string> _routes = new();

    public void RegisterRoute<TViewModel>(string route)
    {
        var key = typeof(TViewModel);
        if (string.IsNullOrWhiteSpace(route))
        {
            throw new ArgumentException("Route must be provided", nameof(route));
        }

        _routes[key] = route;
    }

    public Task NavigateToAsync<TViewModel>(Dictionary<string, object>? parameters = null)
    {
        if (!_routes.TryGetValue(typeof(TViewModel), out var route))
        {
            throw new InvalidOperationException($"Brak zarejestrowanej trasy dla {typeof(TViewModel).Name}.");
        }

        var shell = Shell.Current ?? throw new InvalidOperationException("Brak aktywnej powłoki nawigacyjnej.");
        return parameters is null
            ? shell.GoToAsync(route)
            : shell.GoToAsync(route, parameters);
    }

    public Task GoBackAsync()
    {
        var shell = Shell.Current ?? throw new InvalidOperationException("Brak aktywnej powłoki nawigacyjnej.");
        return shell.GoToAsync("..");
    }
}
