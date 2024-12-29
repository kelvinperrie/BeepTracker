using MetroLog;
using MetroLog.Maui;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace BeepTracker.Maui.View;

public partial class SettingsPage : ContentPage
{
    public SettingsViewModel _viewModel;
    private readonly ILogger<SettingsPage> _logger;

    public SettingsPage(SettingsViewModel viewModel, ILogger<SettingsPage> logger)
	{
		InitializeComponent();

        _logger = logger;
        _viewModel = viewModel;
        
        BindingContext = viewModel;
    }

    private async void OnShareBeepRecordsClicked(object? sender, EventArgs e)
    {
        try
        {
            var filePath = await _viewModel.GenerateCompressedBeepRecordFile();

            // we only seem to be able to do this from the code behind on a page - it doesn't work from 
            // a viewmodel for some reason
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                File = new ShareFile(filePath),
                Title = "Share beep records from this device"
            });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!", "There was a problem generating the beep record compressed file for sharing: " + ex.Message, "OK");
            _logger.LogError(ex, "Error while clicking on share beeprecords");
        }
    }
}