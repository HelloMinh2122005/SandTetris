using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class BonusSalaryPage : ContentPage
{
	public BonusSalaryPage(BonusSalaryPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}