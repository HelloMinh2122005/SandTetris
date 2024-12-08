using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SandTetris.Views;
using SandTetris.Extensions;
using SandTetris.Services;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Microsoft.Maui.LifecycleEvents;

#if WINDOWS10_0_17763_0_OR_GREATER
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Composition.SystemBackdrops;
using SandTetris.WinUI;
#endif

namespace SandTetris
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseSkiaSharp()
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

            builder.ConfigureLifecycleEvents(events =>
            {
#if WINDOWS10_0_17763_0_OR_GREATER
            events.AddWindows(wndLifeCycleBuilder =>
            {
                wndLifeCycleBuilder.OnWindowCreated(window =>
                {
                    //window.SystemBackdrop = new DesktopAcrylicBackdrop();
                    window.SystemBackdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };
                });
            });
#endif
            });
            //Add services or viewmodel or scoped or singleton inside AddApplicationServices
            builder.Services.AddApplicationServices();

            // for popup page
            builder.UseMauiApp<App>().UseMauiCommunityToolkit();

            return builder.Build();
        }
    }
}
