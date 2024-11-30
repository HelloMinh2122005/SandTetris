using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class ExpenditurePage : ContentPage
{
	public ExpenditurePage(ExpenditurePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}