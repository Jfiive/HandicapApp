using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace GolfHandicapApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
            //toggled gets set here so that the toggled event doesnt get fired on initial page load
            if (Preferences.ContainsKey("DisplayEUDate"))
            {
                DisplayEUDate.IsToggled = Preferences.Get("DisplayEUDate", false);
                DisplayEUDate.Toggled += DisplayEUDate_Toggled;
            }
            if (Preferences.ContainsKey("HighlightScores"))
            {
                HighlightScores.IsToggled = Preferences.Get("HighlightScores", false);
                HighlightScores.Toggled += HighlightScores_Toggled;
            }
            if (Preferences.ContainsKey("SeparateBySeason"))
            {
                SeparateBySeason.IsToggled = Preferences.Get("SeparateBySeason", false);
                SeparateBySeason.Toggled += SeparateBySeason_Toggled;
            }
        }

        private void DisplayEUDate_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("DisplayEUDate", DisplayEUDate.IsToggled);
        }

        private void HighlightScores_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("HighlightScores", HighlightScores.IsToggled);
        }

        private void SeparateBySeason_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("SeparateBySeason", SeparateBySeason.IsToggled);
            App.Database.CalculateHandicap("18");
            App.Database.CalculateHandicap("9");
            if (SeparateBySeason.IsToggled == false && Preferences.Get("SeasonsView", 0) > 0)
            {
                Preferences.Set("SeasonsView", 0);
            }
            (App.Current.MainPage as MainPage).SetMenuItems();
        }
    }
}