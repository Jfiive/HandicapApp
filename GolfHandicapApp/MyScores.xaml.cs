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

            if (Preferences.ContainsKey("Handicap18"))
            {
                HandicapLabel.Text = "Handicap: ";
                HandicapNumberLabel.Text = Preferences.Get("Handicap18", -1.0).ToString();
            }
            if (Preferences.ContainsKey("Handicap9"))
            {
                HandicapLabel9.Text = "Handicap 9: ";
                HandicapNumberLabel9.Text = Preferences.Get("Handicap9", -1.0).ToString();
            }
            if (App.Database.GetNumberOfScores() < 5)
            {
                HandicapLabel.Text = "5+ scores are needed for a handicap.";
                HandicapDisplay9.IsVisible = false;
            }
            SetDisplayInformation();
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
                        SelectedFrontNine.IsChecked = true;
                    }
                    else if (selectedScore.RoundType == "Back")
                    {
                        SelectedBackNine.IsChecked = true;
                    }
                    else
                    {
                        SelectedEighteenHoles.IsChecked = true;
                    }
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
            else if (button.ClassId == "CloseEditScore")
            {
                EditScorePopup.IsVisible = false;
                ScoreList.SelectedItem = null;
            }
        }

        private void FrontNine_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (SelectedEighteenHoles.IsChecked)
            {
                SelectedEighteenHoles.IsChecked = false;
            }
            if (SelectedBackNine.IsChecked)
            {
                SelectedBackNine.IsChecked = false;
            }


            if (SelectedFrontNine.IsChecked)
            {
                SelectedFrontNine.IsChecked = true;
            }
        }

        private void EighteenHoles_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (SelectedFrontNine.IsChecked)
            {
                SelectedFrontNine.IsChecked = false;
            }
            if (SelectedBackNine.IsChecked)
            {
                SelectedBackNine.IsChecked = false;
            }

            if (SelectedEighteenHoles.IsChecked)
            {
                SelectedEighteenHoles.IsChecked = true;
            }
        }
        private void BackNine_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (SelectedFrontNine.IsChecked)
            {
                SelectedFrontNine.IsChecked = false;
            }
            if (SelectedEighteenHoles.IsChecked)
            {
                SelectedEighteenHoles.IsChecked = false;
            }

            if (SelectedBackNine.IsChecked)
            {
                SelectedBackNine.IsChecked = true;
            }
        }

        private void EditScore_Clicked(object sender, EventArgs e)
        {
            var editingscore = App.Database.GetScore(((DetailedScore)ScoreList.SelectedItem).ScoreID);
            editingscore.Score = int.Parse(SelectedScore.Text);
            editingscore.Date = SelectedScoreDate.Date;
            if (SelectedFrontNine.IsChecked)
            {
                editingscore.RoundType = "Front";
            }
            else if (SelectedBackNine.IsChecked)
            {
                editingscore.RoundType = "Back";
            }
            else
            {
                editingscore.RoundType = "18";
            }

            App.Database.UpdateScore(editingscore);
            EditScorePopup.IsVisible = false;
            ScoreList.SelectedItem = null;
            App.Database.CalculateHandicap();
            scorelistdata = App.Database.GetPastScores();
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
        private void SetDisplayInformation()
        {
            if (Preferences.Get("SeparateHandicaps", false) == true)
            {
                HandicapDisplay.HorizontalOptions = LayoutOptions.Start;
                HandicapDisplay9.IsVisible = true;
            }
            else
            {
                HandicapDisplay9.IsVisible = false;
            }
        }
    }
}