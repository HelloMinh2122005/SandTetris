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
            Routing.RegisterRoute(nameof(CheckInPage), typeof(CheckInPage));
        }
    }
}
