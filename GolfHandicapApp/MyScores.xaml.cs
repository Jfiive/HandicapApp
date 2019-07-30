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
    public partial class MyScores : ContentPage
    {
        private List<DetailedScore> scorelistdata = new List<DetailedScore>();
        public List<DetailedScore> ScoreListData { get { return scorelistdata; } }
        public MyScores()
        {
            InitializeComponent();
            scorelistdata = App.Database.GetPastScores();
            ScoreList.ItemsSource = scorelistdata;
            if (App.Database.GetNumberOfScores() < 5)
            {
                HandicapLabel.Text = "5+ scores are needed for a handicap.";
            }
            
            //needs to also do this with 9 hole handicap if they have that set up
            if (Preferences.ContainsKey("Handicap18"))
            {
                HandicapLabel.Text = "Handicap: ";
                HandicapNumberLabel.Text = Preferences.Get("Handicap18", -1.0).ToString();
            }
        }

        private void ScoreClickMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ScoreClickPopup.IsVisible = false;
            ScoreClickMenu.SelectedItem = null;
            var selectedScore = (DetailedScore)ScoreList.SelectedItem;
            switch (e.SelectedItemIndex)
            {
                case 0: //edit score

                    break;

                case 1: //delete score
                    App.Database.DeleteScore(selectedScore.ScoreID);
                    scorelistdata = App.Database.GetPastScores();
                    ScoreList.ItemsSource = scorelistdata;
                    if (scorelistdata.Count < 5)
                    {
                        HandicapLabel.Text = "5+ scores are needed for a handicap.";
                        if (Preferences.ContainsKey("Handicap18"))
                        {
                            Preferences.Remove("Handicap18");
                            HandicapNumberLabel.Text = "";
                        }
                    }
                    break;
            }
        }

        private void ScoreList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ScoreClickPopup.IsVisible = true;
        }
        private void ClosePopup(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;
            if (button.ClassId == "CloseScoreClick")
            {
                ScoreClickPopup.IsVisible = false;
                ScoreList.SelectedItem = null;
            }
        }
    }
}