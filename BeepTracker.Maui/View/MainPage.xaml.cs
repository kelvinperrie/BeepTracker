namespace BeepTracker.Maui.View;

public partial class MainPage : ContentPage
{
	private BeepEntriesViewModel _viewModel;

	public MainPage(BeepEntriesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

        _viewModel = viewModel;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // when this page is displayed, reload the beep record list
        await _viewModel.GetBeepRecordsAsync();
    }
}

