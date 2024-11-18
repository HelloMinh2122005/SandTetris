using Microsoft.Extensions.Configuration;
using SandTetris.Data;
using SandTetris.Interfaces;
using SandTetris.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<DatabaseService>();

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ISalaryDetailRepository, SalaryDetailRepository>();
        services.AddScoped<ICheckInRepository, CheckInRepository>();

        return services;
    }
}
