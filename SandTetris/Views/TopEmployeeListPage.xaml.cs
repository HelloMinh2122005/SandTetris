using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class TopEmployeeListPage : ContentPage
{
	public TopEmployeeListPage(TopEmployeeListPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TopEmployeeListPageViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }
}