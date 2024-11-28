using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class SelectHeadOfDepartmentPage : ContentPage
{
	public SelectHeadOfDepartmentPage(SelectHeadOfDepartmentPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}