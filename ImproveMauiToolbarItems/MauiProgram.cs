using Microsoft.Extensions.Logging;

namespace ImproveMauiToolbarItems;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.ConfigureMauiHandlers(handlers =>
			{
#if IOS
				handlers.AddHandler<ContentPage, Handlers.ContentPageHandler>();
#endif
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
