using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Rg.Plugins.Popup.Services;

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

        private void ScoreList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new ScoreClick(this, (DetailedScore)ScoreList.SelectedItem));
        }
        public void ScoreListMenuClose()
        {
            ScoreList.SelectedItem = null;
        }
        public void RefreshScoreList()
        {
            scorelistdata = App.Database.GetPastScores(GetSelectedDisplayMode());
            ScoreList.ItemsSource = scorelistdata;
            //do the same with 9 hole handicap as well
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

                if (HandicapDisplayMode.IsToggled)
                {
                    HandicapNumberLabel.Text = Preferences.Get("Handicap18", -1.0).ToString();
                }
                else
                {
                    HandicapNumberLabel.Text = Preferences.Get("Handicap9", -1.0).ToString();
                }
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