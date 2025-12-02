namespace ShopperMaui.Services.Interfaces;

public interface IFileSaver {
	Task<string?> SaveAsync(string fileName, string content, CancellationToken cancellationToken = default);
}
