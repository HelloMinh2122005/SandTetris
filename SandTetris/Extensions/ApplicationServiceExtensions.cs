using SandTetris.Data;
using SandTetris.Entities;
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
        services.AddScoped<ISalaryService, SalaryService>();
        services.AddSingleton<CheckInSummary>();


        services.AddSingleton<MainPage>();
        services.AddSingleton<MainViewModel>();

        services.AddSingleton<DepartmentPage>();
        services.AddSingleton<DepartmentPageViewModel>();
        services.AddTransient<AddDepartmentPage>();
        services.AddTransient<AddDepartmentPageViewModel>();
        services.AddTransient<SelectHeadOfDepartmentPage>();
        services.AddTransient<SelectHeadOfDepartmentPageViewModel>();

        services.AddTransient<EmployeePage>();
        services.AddTransient<EmployeePageViewModel>();
        services.AddTransient<AddEmployeePage>();
        services.AddTransient<AddEmployeePageViewModel>();
        services.AddTransient<EmployeeInfoPage>();
        services.AddTransient<EmployeeInfoPageViewModel>();

        services.AddSingleton<DepartmentCheckInPage>();
        services.AddSingleton<DepartmentCheckInPageViewModel>();
        services.AddTransient<CheckInDetailPage>();
        services.AddTransient<CheckInDetailPageViewModel>();
        services.AddTransient<EmployeeCheckInPage>();
        services.AddTransient<EmployeeCheckInPageViewModel>();

        services.AddSingleton<ExpenditurePage>();
        services.AddSingleton<ExpenditurePageViewModel>();
        services.AddTransient<SalaryPage>();
        services.AddTransient<SalaryPageViewModel>();
        services.AddTransient<SalaryDetailPage>();
        services.AddTransient<AddSalaryPage>();

        return services;
    }
}
