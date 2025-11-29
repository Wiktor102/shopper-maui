using System.Threading.Tasks;
using ShopperMaui.Models;

namespace ShopperMaui.Services;

public interface IDataService
{
    Task<ShoppingList> LoadShoppingListAsync();
    Task SaveShoppingListAsync(ShoppingList list);
    Task<string> ExportShoppingListAsync(ShoppingList list);
    Task<ShoppingList> ImportShoppingListAsync(string filePath);
}
