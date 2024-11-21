namespace BeepTracker.Maui.View;

public partial class DetailsPage : ContentPage
{
	public DetailsPage(BeepEntryDetailsViewModel viewModel)
	{

        viewModel.MyPage = this;

        InitializeComponent();
		BindingContext = viewModel;

    }

    public void ScrollToBeepEntry(BeepEntry recordToScrollTo)
    {
        beepEntriesCollectionView.ScrollTo(recordToScrollTo);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }
}