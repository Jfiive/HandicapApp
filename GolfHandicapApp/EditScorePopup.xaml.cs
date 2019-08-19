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
            SelectedScoreDate.Date = Score.Date;
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
            NewScore.Date = SelectedScoreDate.Date;
            NewScore.RoundType = SelectedRoundType.SelectedItem.ToString();

            if (NewScore.Score != OriginalScore.Score || NewScore.Date != OriginalScore.Date || NewScore.RoundType != OriginalScore.RoundType)
            {
                var editingscore = App.Database.GetScore(OriginalScore.ScoreID);
                editingscore.Score = NewScore.Score;
                editingscore.Date = NewScore.Date;
                editingscore.RoundType = NewScore.RoundType;
                var teeinfo = App.Database.GetTeeInfo(editingscore.PlayedID);

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
    }
}