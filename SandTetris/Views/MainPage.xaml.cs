using SandTetris.ViewModels;

namespace SandTetris.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
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

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is MainViewModel viewModel)
            {
                bool ac = await viewModel.LoginAsync();
                if (ac)
                {
                    Shell.SetFlyoutBehavior(this, FlyoutBehavior.Flyout);
                    Shell.SetNavBarIsVisible(this, false);
                }
            }
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            Button_Clicked(LoginButton, EventArgs.Empty);
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
