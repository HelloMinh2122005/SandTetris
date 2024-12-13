using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class EmployeeOfTheMonthPage : ContentPage
{
	public EmployeeOfTheMonthPage(EmployeeOfTheMonthPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}