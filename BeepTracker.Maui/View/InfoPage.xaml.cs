using System.Windows.Input;

namespace BeepTracker.Maui.View;

public partial class InfoPage : ContentPage
{

    public ICommand LoadWebsiteCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));

    public InfoPage()
	{
		InitializeComponent();
	}
}