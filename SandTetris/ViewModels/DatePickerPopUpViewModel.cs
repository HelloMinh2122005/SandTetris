using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.ViewModels;

public partial class DatePickerPopUpViewModel : ObservableObject
{
    private Popup _popup;

    [ObservableProperty]
    private DateTime date = DateTime.Now;

    public void SetPopup(Popup popup)
    {
        _popup = popup;
    }

    [RelayCommand]
    private void Save()
    {
        _popup?.Close(Date);
    }

    [RelayCommand]
    private void Cancel()
    {
        _popup?.Close(null);
    }
}
