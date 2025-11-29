using System.Collections.Generic;

namespace ShopperMaui.Models;

public static class Unit
{
    public static IReadOnlyList<string> All { get; } = new List<string>
    {
        "szt.",
        "kg",
        "g",
        "l",
        "ml",
        "opakowanie"
    };
}
