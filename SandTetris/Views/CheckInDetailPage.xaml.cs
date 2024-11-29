using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class CheckInDetailPage : ContentPage
{
	public CheckInDetailPage(CheckInDetailPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}