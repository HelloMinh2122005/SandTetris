using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SandTetris.Views;
using SandTetris.Extensions;
using SandTetris.Services;

namespace SandTetris
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            //Add services or viewmodel or scoped or singleton inside AddApplicationServices
            builder.Services.AddApplicationServices();

            return builder.Build();
        }
    }
}
