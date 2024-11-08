namespace BeepTracker.Maui.View;

public partial class MainPage : ContentPage
{
	public MainPage(BeepEntriesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

