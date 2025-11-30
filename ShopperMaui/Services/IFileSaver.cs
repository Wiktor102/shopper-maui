namespace ShopperMaui.Services;

public interface IFileSaver {
	Task<string?> SaveAsync(string fileName, string content, CancellationToken cancellationToken = default);
}
