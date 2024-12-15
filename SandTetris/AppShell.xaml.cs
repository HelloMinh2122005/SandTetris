﻿using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using SandTetris.ViewModels;
using SandTetris.Views;

namespace SandTetris;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Loaded += (_, _) => {
            ForceNoBackground();
        };

        Application.Current!.RequestedThemeChanged += (_, _) =>
        {
            ForceNoBackground();
        };

        Routing.RegisterRoute(nameof(DepartmentPage), typeof(DepartmentPage));
        Routing.RegisterRoute(nameof(AddDepartmentPage), typeof(AddDepartmentPage));
        Routing.RegisterRoute(nameof(SelectHeadOfDepartmentPage), typeof(SelectHeadOfDepartmentPage));
        Routing.RegisterRoute(nameof(EmployeePage), typeof(EmployeePage));
        Routing.RegisterRoute(nameof(EmployeeInfoPage), typeof(EmployeeInfoPage));
        Routing.RegisterRoute(nameof(AddEmployeePage), typeof(AddEmployeePage));
        Routing.RegisterRoute(nameof(DepartmentCheckInPage), typeof(DepartmentCheckInPage));
        Routing.RegisterRoute(nameof(CheckInDetailPage), typeof(CheckInDetailPage));
        Routing.RegisterRoute(nameof(EmployeeCheckInPage), typeof(EmployeeCheckInPage));
        Routing.RegisterRoute(nameof(ExpenditurePage), typeof(ExpenditurePage));
        Routing.RegisterRoute(nameof(SalaryPage), typeof(SalaryPage));
        Routing.RegisterRoute(nameof(SalaryDetailPage), typeof(SalaryDetailPage));
        Routing.RegisterRoute(nameof(EmployeeOfTheMonthPage), typeof(EmployeeOfTheMonthPage));
        Routing.RegisterRoute(nameof(TopEmployeeListPage), typeof(TopEmployeeListPage));
        Routing.RegisterRoute(nameof(BonusSalaryPage), typeof(BonusSalaryPage));
    }
    private static void ForceNoBackground()
    {
#if WINDOWS
        var shellView = Current?.Handler?.PlatformView as ShellView;
        var navigationView = shellView?.Content as MauiNavigationView;

        var contentGrid = navigationView?.GetType()
            .GetProperty("ContentGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
            .GetValue(navigationView) as Microsoft.UI.Xaml.Controls.Grid;

        contentGrid!.Background.Opacity = 0;
#endif
    }
}

