using ShopperMaui.Services.Interfaces;

namespace ShopperMaui.Services;

public class FileSaverService : IFileSaver {
	public async Task<string?> SaveAsync(string fileName, string content, CancellationToken cancellationToken = default) {
		// Fallback for other platforms: just save to cache and return path
		var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
		await File.WriteAllTextAsync(filePath, content, cancellationToken);

		return filePath;
	}
}
