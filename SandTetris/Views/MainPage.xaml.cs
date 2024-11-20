using SandTetris.ViewModels;

namespace SandTetris.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
        //protected override async void OnAppearing()
        //{
        //    base.OnAppearing();
        //    await Task.Yield();

        //    if (BindingContext is MainViewModel viewModel)
        //    {
        //        await viewModel.InitializeDbAndNavigate();
        //    }
        //}
    }

}
