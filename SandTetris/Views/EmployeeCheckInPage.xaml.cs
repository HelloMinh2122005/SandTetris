using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class EmployeeCheckInPage : ContentPage
{
	public EmployeeCheckInPage(EmployeeCheckInPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}