using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class DepartmentCheckInPage : ContentPage
{
	public DepartmentCheckInPage(DepartmentCheckInPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}