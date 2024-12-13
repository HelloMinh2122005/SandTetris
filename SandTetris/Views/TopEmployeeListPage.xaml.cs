using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class TopEmployeeListPage : ContentPage
{
	public TopEmployeeListPage(TopEmployeeListPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}