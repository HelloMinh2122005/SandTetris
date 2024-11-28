using SandTetris.Data;
using SandTetris.Interfaces;
using SandTetris.Services;
using SandTetris.ViewModels;
using SandTetris.Views;

namespace SandTetris.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //Register the DatabaseService
        services.AddSingleton<DatabaseService>();

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddSingleton<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ISalaryDetailRepository, SalaryDetailRepository>();
        services.AddScoped<ICheckInRepository, CheckInRepository>();


        services.AddSingleton<MainPage>();
        services.AddSingleton<MainViewModel>();

        services.AddSingleton<DepartmentPage>();
        services.AddSingleton<DepartmentPageViewModel>();
        services.AddSingleton<AddDepartmentPage>();
        services.AddSingleton<AddDepartmentPageViewModel>();
        services.AddSingleton<SelectHeadOfDepartmentPage>();
        services.AddSingleton<SelectHeadOfDepartmentPageViewModel>();

        services.AddSingleton<EmployeePage>();
        services.AddSingleton<EmployeePageViewModel>();
        services.AddSingleton<AddEmployeePage>();
        services.AddSingleton<AddEmployeePageViewModel>();
        services.AddSingleton<EmployeeInfoPage>();
        services.AddSingleton<EmployeeInfoPageViewModel>();

        services.AddSingleton<DepartmentCheckInPage>();
        services.AddSingleton<DepartmentCheckInPageViewModel>();
        services.AddSingleton<CheckInDetailPage>();
        services.AddSingleton<CheckInDetailPageViewModel>();
        services.AddSingleton<EmployeeCheckInPage>();

        services.AddSingleton<ExpenditurePage>();
        services.AddSingleton<SalaryPage>();
        services.AddSingleton<SalaryDetailPage>();
        services.AddSingleton<AddSalaryPage>();

        return services;
    }
}
