using System.Collections.Generic;

namespace ShopperMaui.Models;

public static class Store
{
    public static IReadOnlyList<string> All { get; } = new List<string>
    {
        "Biedronka",
        "Lidl",
        "Kaufland",
        "Auchan",
        "Carrefour",
        "Żabka"
    };
}
