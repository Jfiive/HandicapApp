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
    public partial class PastSeasons : ContentPage
    {
        private List<DateTime> seasonlistdata = new List<DateTime>();
        public List<DateTime> SeasonListData { get { return seasonlistdata; } }
        public PastSeasons()
        {
            InitializeComponent();
            var data = App.Database.GetSeasons();
            SeasonList.ItemsSource = data;
            if (data.Count == 0)
            {
                Title = "Only Season is Current Season";
                ClearSeason.IsVisible = false;
            }
            if (Preferences.Get("SeasonsView", 0) > 0)
            {
                //set the selected item to be what the current viewed season
                var item = data.Single(s => s == Preferences.Get("SeasonsView", 0));
                SeasonList.SelectedItem = item;
            }
        }

        private void SeasonList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Preferences.Set("SeasonsView", (int)SeasonList.SelectedItem);
            if (Preferences.Get("SeparateBySeason", false) == true)
            {
                App.Database.CalculateHandicap("18");
                App.Database.CalculateHandicap("9");
            }
            (Application.Current.MainPage as MainPage).ClickPage("Your Scores");
        }

        private void ClearSeason_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("SeasonsView", 0);
            if (SeasonList.SelectedItem != null && Preferences.Get("SeparateBySeason", false) == true)
            {
                App.Database.CalculateHandicap("18");
                App.Database.CalculateHandicap("9");
            }
            SeasonList.SelectedItem = null;
            (Application.Current.MainPage as MainPage).ClickPage("Your Scores");
        }
    }
}