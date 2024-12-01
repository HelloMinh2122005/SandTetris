using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class SalaryPage : ContentPage
{
	public SalaryPage(SalaryPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}