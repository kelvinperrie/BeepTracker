using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Maui.Services
{
    public interface ISettingsService
    {
        string ApiBasePath { get; set; }
        bool TestSetting { get; set; }
    }

    public sealed class SettingsService : ISettingsService
    {
        private const string apiBasePath = "api_base_path";
        private const string apiBasePathDefault = "";

        private const string testSetting = "test_setting";
        private const bool testSettingDefault = true;

        public string ApiBasePath
        {
            get => Preferences.Get(apiBasePath, apiBasePathDefault);
            set => Preferences.Set(apiBasePath, value);
        }

        public bool TestSetting
        {
            get => Preferences.Get(testSetting, testSettingDefault);
            set => Preferences.Set(testSetting, value);
        }
    }
}
