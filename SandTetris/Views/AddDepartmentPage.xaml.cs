using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class AddDepartmentPage : ContentPage
{
	public AddDepartmentPage(AddDepartmentPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}