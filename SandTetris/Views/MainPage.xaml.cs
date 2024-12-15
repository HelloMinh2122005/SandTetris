using SandTetris.ViewModels;

namespace SandTetris.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled)
        }

        private void OnEmployeeList(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync($"//EmployeeList");
        }

        private void OnCheckIn(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync($"//CheckIn");
        }

        private void OnSummary(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync($"//Summary");

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
