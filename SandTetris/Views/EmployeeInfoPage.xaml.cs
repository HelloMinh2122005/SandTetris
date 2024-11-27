using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class EmployeeInfoPage : ContentPage
{
	public EmployeeInfoPage(EmployeeInfoPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}