using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
<<<<<<< HEAD
using SandTetris.Views;
=======
using SandTetris.Extensions;
>>>>>>> b12236ad4d51691e350ca8dc9558ed55de6c4dd4

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

            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<DepartmentPage>();
            builder.Services.AddTransient<AddDepartmentPage>();

            builder.Services.AddTransient<EmployeePage>();
            builder.Services.AddTransient<AddEmployeePage>();
            builder.Services.AddTransient<EmployeeInfoPage>();

            builder.Services.AddTransient<CheckInPage>();


            return builder.Build();
        }
    }
}
