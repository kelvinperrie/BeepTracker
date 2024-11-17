namespace BeepTracker.Maui.View;

public partial class StartPage : ContentPage
{
    private StartPageViewModel _viewModel;

    public StartPage(StartPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

        _viewModel = viewModel;
    }
}