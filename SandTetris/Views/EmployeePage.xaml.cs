using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class EmployeePage : ContentPage
{
	public EmployeePage(EmployeePageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}