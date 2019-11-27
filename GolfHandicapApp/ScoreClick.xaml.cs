using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GolfHandicapApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScoreClick : Rg.Plugins.Popup.Pages.PopupPage
    {
        public MyScores mp;
        public DetailedScore selectedScore;
        public ScoreClick(MyScores m, DetailedScore SelectedItem)
        {
            InitializeComponent();
            mp = m;
            selectedScore = SelectedItem;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            mp.ScoreListMenuClose();
        }
        private void ScoreClickMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var lastScoreOfSeason = App.Database.IsLastScoreOfSeason(selectedScore.Date.Year);
            switch (e.SelectedItemIndex)
            {
                case 0: //edit score
                    PopupNavigation.Instance.PushAsync(new EditScorePopup(mp, selectedScore, this));
                    break;

                case 1: //delete score
                    App.Database.DeleteScore(selectedScore.ScoreID);

                    if (lastScoreOfSeason == true)
                    {
                        var allSeasons = App.Database.GetSeasons();
                        if (allSeasons.Count > 0)
                        {
                            if (Preferences.Get("SeasonsView", 0) > 0)
                            {
                                Preferences.Set("SeasonsView", 0);
                            }
                            var latestSeason = allSeasons.Max();
                            Preferences.Set("LatestSeason", latestSeason);
                        }
                        else
                        {
                            //the last score in the whole table was deleted so it needs to reset the seasons information and start from new
                            Preferences.Set("SeasonsView", 0);
                            Preferences.Set("LatestSeason", 0);
                        }
                    }

                    App.Database.CalculateHandicap(Preferences.Get("HandicapDisplayType", 18).ToString());
                    mp.RefreshScoreList();
                    PopupNavigation.Instance.PopAsync();
                    break;
            }
        }
        public void ClearSelectedItem()
        {
            ScoreClickMenu.SelectedItem = null;
        }
    }
}