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
            scorelistdata = App.Database.GetPastScores(Preferences.Get("HandicapDisplayType", 18).ToString());
            ScoreList.ItemsSource = scorelistdata;

            if (Preferences.Get("HandicapDisplayType", 18) == 18)
            {
                HandicapDisplayMode.IsToggled = true;
            }
            else
            {
                scorelistdata = App.Database.GetPastScores("9");
                ScoreList.ItemsSource = scorelistdata;
                if (scorelistdata.Count < 5)
                {
                    HandicapLabel.Text = "5+ scores are needed for a handicap.";
                    HandicapNumberLabel.Text = "";
                }
                else
                {
                    if (HandicapLabel.Text != "Handicap: ")
                    {
                        HandicapLabel.Text = "Handicap: ";
                    }
                    HandicapNumberLabel.Text = Preferences.Get("Handicap9", -1.0).ToString();
                }
            }
            //This is 18 because by default the page is going to be in 18 handicap mode
            if (scorelistdata.Count < 5)
            {
                HandicapLabel.Text = "5+ scores are needed for a handicap.";
            }
            SetDataType();
        }

        private void ScoreClickMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ScoreClickPopup.IsVisible = false;
            ScoreClickMenu.SelectedItem = null;
            var selectedScore = (DetailedScore)ScoreList.SelectedItem;
            switch (e.SelectedItemIndex)
            {
                case 0: //edit score
                    EditScorePopup.IsVisible = true;
                    SelectedScore.Text = selectedScore.Score.ToString();
                    SelectedScoreDate.Date = selectedScore.Date;
                    if (selectedScore.RoundType == "Front")
                    {
                        SelectedRoundType.SelectedIndex = 1;
                    }
                    else if (selectedScore.RoundType == "Back")
                    {
                        SelectedRoundType.SelectedIndex = 2;
                    }
                    else
                    {
                        SelectedRoundType.SelectedIndex = 0;
                    }
                    break;

                case 1: //delete score
                    App.Database.DeleteScore(selectedScore.ScoreID);
                    scorelistdata = App.Database.GetPastScores(GetSelectedDisplayMode());
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
            else if (button.ClassId == "CloseEditScore")
            {
                EditScorePopup.IsVisible = false;
                ScoreList.SelectedItem = null;
            }
        }

        private void EditScore_Clicked(object sender, EventArgs e)
        {
            var editingscore = App.Database.GetScore(((DetailedScore)ScoreList.SelectedItem).ScoreID);
            editingscore.Score = int.Parse(SelectedScore.Text);
            editingscore.Date = SelectedScoreDate.Date;

            editingscore.RoundType = SelectedRoundType.SelectedItem.ToString();

            App.Database.UpdateScore(editingscore);
            EditScorePopup.IsVisible = false;
            ScoreList.SelectedItem = null;
            App.Database.CalculateHandicap(editingscore.RoundType);
            scorelistdata = App.Database.GetPastScores(GetSelectedDisplayMode());
            ScoreList.ItemsSource = scorelistdata;
            if (Preferences.ContainsKey("Handicap18") && scorelistdata.Count >= 5)
            {
                HandicapLabel.Text = "Handicap: ";
                HandicapNumberLabel.Text = Preferences.Get("Handicap18", -1.0).ToString();
            }
        }
        private void SetDataType()
        {
            var DisplayEUDate = false;
            var HighlightScores = false;
            if (Preferences.ContainsKey("DisplayEUDate"))
            {
                DisplayEUDate = Preferences.Get("DisplayEUDate", false);
            }
            if (Preferences.ContainsKey("HighlightScores"))
            {
                HighlightScores = Preferences.Get("HighlightScores", false);
            }

            if (DisplayEUDate == true && HighlightScores == true) //eu date with highlighted scores
            {
                ScoreList.ItemTemplate = (DataTemplate)Resources["EUDateHighlight"];
            }
            else if (DisplayEUDate == true && HighlightScores == false) //eu date with no highlights
            {
                ScoreList.ItemTemplate = (DataTemplate)Resources["EUDate"];
            }
            else if (DisplayEUDate == false && HighlightScores == true) //US date with highlights
            {
                ScoreList.ItemTemplate = (DataTemplate)Resources["USDateHighlight"];
            }
            else if (DisplayEUDate == false && HighlightScores == false)
            {
                ScoreList.ItemTemplate = (DataTemplate)Resources["USDate"];
            }
        }

        private string GetSelectedDisplayMode()
        {
            if (HandicapDisplayMode.IsToggled)
            {
                return "18";
            }
            else
            {
                return "9";
            }
        }

        private void HandicapDisplayMode_Toggled(object sender, ToggledEventArgs e)
        {
            if (HandicapDisplayMode.IsToggled)
            {
                //display 18 hole handicap information
                scorelistdata = App.Database.GetPastScores("18");
                ScoreList.ItemsSource = scorelistdata;
                if (scorelistdata.Count < 5)
                {
                    HandicapLabel.Text = "5+ scores are needed for a handicap.";
                    HandicapNumberLabel.Text = "";
                }
                else
                {
                    if (HandicapLabel.Text != "Handicap: ")
                    {
                        HandicapLabel.Text = "Handicap: ";
                    }
                    HandicapNumberLabel.Text = Preferences.Get("Handicap18", -1.0).ToString();
                }
                Preferences.Set("HandicapDisplayType", 18);
            }
            else
            {
                //display 9 hole handicap information
                scorelistdata = App.Database.GetPastScores("9");
                ScoreList.ItemsSource = scorelistdata;
                if (scorelistdata.Count < 5)
                {
                    HandicapLabel.Text = "5+ scores are needed for a handicap.";
                    HandicapNumberLabel.Text = "";
                }
                else
                {
                    if (HandicapLabel.Text != "Handicap: ")
                    {
                        HandicapLabel.Text = "Handicap: ";
                    }
                    HandicapNumberLabel.Text = Preferences.Get("Handicap9", -1.0).ToString();
                }
                Preferences.Set("HandicapDisplayType", 9);
            }
        }
    }
}