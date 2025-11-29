namespace ShopperMaui.Helpers;

public static class ServiceHelper {
	public static IServiceProvider Services { get; set; } = default!;

	public static T GetService<T>() where T : notnull {
		if (Services is null) {
			throw new InvalidOperationException("Service provider has not been initialized.");
		}

		return (T)Services.GetService(typeof(T))!;
	}
}
