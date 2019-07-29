using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            if (App.Database.GetNumberOfScores() <= 5)
            {
                HandicapLabel.Text = "5+ scores are needed for a handicap.";
                //HandicapDisplay.FontSize = 9;
            }
            else
            {
                HandicapLabel.Text = "Handicap: ";
            }

            //needs to also do this with 9 hole handicap if they have that set up
            if (Application.Current.Properties.ContainsKey("Handicap18"))
            {
                HandicapNumberLabel.Text = Application.Current.Properties["Handicap18"].ToString();
            }
        }

        private void ScoreClickMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ScoreClickMenu.IsVisible = false;
            var selectedScore = (DetailedScore)ScoreList.SelectedItem;
            switch (e.SelectedItemIndex)
            {
                case 0: //edit score

                    break;

                case 1: //delete score
                    App.Database.DeleteScore(selectedScore.ScoreID);
                    scorelistdata = App.Database.GetPastScores();
                    ScoreList.ItemsSource = scorelistdata;
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