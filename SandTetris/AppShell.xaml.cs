using SandTetris.Views;

namespace SandTetris
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(DepartmentPage), typeof(DepartmentPage));
            Routing.RegisterRoute(nameof(AddDepartmentPage), typeof (AddDepartmentPage));
            Routing.RegisterRoute(nameof(EmployeePage), typeof(EmployeePage));
            Routing.RegisterRoute(nameof(EmployeeInfoPage), typeof(EmployeeInfoPage));
            Routing.RegisterRoute(nameof(AddEmployeePage), typeof(AddEmployeePage));
            Routing.RegisterRoute(nameof(DepartmentCheckInPage), typeof(DepartmentCheckInPage));
            Routing.RegisterRoute(nameof(CheckInDetailPage), typeof(CheckInDetailPage));
            Routing.RegisterRoute(nameof(EmployeeCheckInPage), typeof(EmployeeCheckInPage));
            Routing.RegisterRoute(nameof(ExpenditurePage), typeof(ExpenditurePage));
            Routing.RegisterRoute(nameof(SalaryPage), typeof(SalaryPage));
            Routing.RegisterRoute(nameof(SalaryDetailPage), typeof(SalaryDetailPage));
            Routing.RegisterRoute(nameof(AddSalaryPage), typeof(AddSalaryPage));
        }
    }
}
