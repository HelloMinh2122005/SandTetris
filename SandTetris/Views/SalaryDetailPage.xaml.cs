using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class SalaryDetailPage : ContentPage
{
	public SalaryDetailPage(SalaryDetailPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}