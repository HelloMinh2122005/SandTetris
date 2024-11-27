using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class AddEmployeePage : ContentPage
{
	public AddEmployeePage(AddEmployeePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}