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
        }

        private void ClearSeason_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("SeasonsView", 0);
            SeasonList.SelectedItem = null;
        }
    }
}