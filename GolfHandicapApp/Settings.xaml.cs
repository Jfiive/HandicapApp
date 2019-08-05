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
            //switch all the switches to the saved settings
            if (Preferences.ContainsKey("DisplayEUDate"))
            {
                DisplayEUDate.IsToggled = Preferences.Get("DisplayEUDate", false);
            }
            if (Preferences.ContainsKey("HighlightScores"))
            {
                HighlightScores.IsToggled = Preferences.Get("HighlightScores", false);
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
    }
}