using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.Services.Interfaces;

namespace ShopperMaui.Services;

public class XmlDataService : IDataService {
	public async Task<ShoppingList> LoadShoppingListAsync() {
		var path = GetShoppingListPath();
		if (!File.Exists(path)) {
			var defaultList = DefaultDataProvider.CreateDefaultShoppingList();
			await SaveShoppingListAsync(defaultList);
			return defaultList;
		}

		var xml = await File.ReadAllTextAsync(path);
		var list = XmlSerializationHelper.DeserializeFromXml<ShoppingList>(xml);
		return list ?? DefaultDataProvider.CreateDefaultShoppingList();
	}

	public async Task SaveShoppingListAsync(ShoppingList list) {
		if (list is null) {
			throw new ArgumentNullException(nameof(list));
		}

		list.LastModified = DateTime.UtcNow;
		var xml = XmlSerializationHelper.SerializeToXml(list);
		var path = GetShoppingListPath();
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		await File.WriteAllTextAsync(path, xml);
	}

	public async Task<string> ExportShoppingListAsync(ShoppingList list) {
		var exportFileName = $"shopping_list_{DateTime.UtcNow:yyyyMMddHHmmss}.xml";
		var exportPath = Path.Combine(FileSystem.CacheDirectory, exportFileName);
		var xml = XmlSerializationHelper.SerializeToXml(list);
		await File.WriteAllTextAsync(exportPath, xml);
		return exportPath;
	}

	public async Task<ShoppingList> ImportShoppingListAsync(string filePath) {
		if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) {
			throw new FileNotFoundException("Nie znaleziono pliku listy zakupów.", filePath);
		}

		var xml = await File.ReadAllTextAsync(filePath);
		var shoppingList = XmlSerializationHelper.DeserializeFromXml<ShoppingList>(xml) ?? DefaultDataProvider.CreateDefaultShoppingList();
		await SaveShoppingListAsync(shoppingList);
		return shoppingList;
	}

	private static string GetShoppingListPath()
		=> Path.Combine(FileSystem.AppDataDirectory, Constants.ShoppingListFileName);
}
