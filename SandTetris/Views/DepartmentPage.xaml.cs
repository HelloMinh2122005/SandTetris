using SandTetris.ViewModels.DepartmentViewModel;

namespace SandTetris.Views;

public partial class DepartmentPage : ContentPage
{
	public DepartmentPage(DepartmentListViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}