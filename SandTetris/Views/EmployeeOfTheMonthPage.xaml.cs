using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class EmployeeOfTheMonthPage : ContentPage
{
	public EmployeeOfTheMonthPage(EmployeeOfTheMonthPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is EmployeeOfTheMonthPageViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }
} 