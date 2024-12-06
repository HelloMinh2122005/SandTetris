using CommunityToolkit.Maui.Views;
using SandTetris.ViewModels;

namespace SandTetris.Views;

public partial class DatePickerPopUp : Popup
{
    public DatePickerPopUp(DatePickerPopUpViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        vm.SetPopup(this);
    }
}
