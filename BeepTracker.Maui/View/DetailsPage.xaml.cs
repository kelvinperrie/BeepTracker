namespace BeepTracker.Maui.View;

public partial class DetailsPage : ContentPage
{
	public DetailsPage(BeepEntryDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}