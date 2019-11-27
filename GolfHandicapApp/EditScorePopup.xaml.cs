using Rg.Plugins.Popup.Services;
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
    public partial class EditScorePopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public MyScores mp;
        public DetailedScore OriginalScore;
        public ScoreClick ScoreClick;
        public EditScorePopup(MyScores m, DetailedScore Score, ScoreClick PrevPage)
        {
            InitializeComponent();
            mp = m;
            OriginalScore = Score;
            ScoreClick = PrevPage;
            SelectedScore.Text = Score.Score.ToString();
            if (Score.RoundType == "Front")
            {
                SelectedRoundType.SelectedIndex = 1;
            }
            else if (Score.RoundType == "Back")
            {
                SelectedRoundType.SelectedIndex = 2;
            }
            else
            {
                SelectedRoundType.SelectedIndex = 0;
            }
            //needs to have the courseID from the MyScores Page and also needs to have the InfoID from the Scores table so it can set it to the right tee
            TeePicker.ItemsSource = App.Database.GetCourseTees(mp.GetSelectedCourseID());
            TeePicker.ItemDisplayBinding = new Binding("DisplayName");
            TeePicker.IsEnabled = true;
            foreach (PickerTee item in TeePicker.ItemsSource)
            {
                if (item.InfoID == OriginalScore.InfoID)
                {
                    TeePicker.SelectedItem = item;
                }
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ScoreClick.ClearSelectedItem();
        }

        private void EditScore_Clicked(object sender, EventArgs e)
        {
            var NewScore = new DetailedScore();
            NewScore.Score = int.Parse(SelectedScore.Text);
            NewScore.InfoID = ((PickerTee)TeePicker.SelectedItem).InfoID;
            NewScore.RoundType = SelectedRoundType.SelectedItem.ToString();

            if (NewScore.Score != OriginalScore.Score || NewScore.InfoID != OriginalScore.InfoID || NewScore.RoundType != OriginalScore.RoundType)
            {
                var editingscore = App.Database.GetScore(OriginalScore.ScoreID);
                editingscore.Score = NewScore.Score;
                editingscore.InfoID = NewScore.InfoID;
                editingscore.RoundType = NewScore.RoundType;
                var teeinfo = App.Database.GetTeeInfo(editingscore.InfoID);

                if (editingscore.RoundType == "18")
                {
                    editingscore.Differential = Math.Round((editingscore.Score - teeinfo.Rating) * 113 / teeinfo.Slope, 2);
                }
                else if (editingscore.RoundType == "Front")
                {
                    editingscore.Differential = Math.Round((editingscore.Score - teeinfo.FrontRating) * 113 / teeinfo.Slope, 2);
                }
                else if (editingscore.RoundType == "Back")
                {
                    //some courses dont have a back rating so need to remove the back rating setting if they dont
                    editingscore.Differential = Math.Round((editingscore.Score - teeinfo.BackRating) * 113 / teeinfo.Slope, 2);
                }

                App.Database.UpdateScore(editingscore);
                App.Database.CalculateHandicap(NewScore.RoundType);
                mp.RefreshScoreList();
                PopupNavigation.Instance.PopAllAsync();
            }
        }
        private void ValidityCheck()
        {
            if (!string.IsNullOrEmpty(SelectedScore.Text) && SelectedRoundType.SelectedIndex >= 0 && TeePicker.SelectedIndex >= 0)
            {
                EditScore.IsEnabled = true;
            }
            else
            {
                EditScore.IsEnabled = false;
            }
        }
        private void SelectedScore_Unfocused(object sender, FocusEventArgs e)
        {
            ValidityCheck();
        }

        private void SelectedRoundType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidityCheck();
        }
        private void TeePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidityCheck();
        }
    }
}