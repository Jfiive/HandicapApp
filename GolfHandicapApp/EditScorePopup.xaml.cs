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
        public EditScorePopup(MyScores m, DetailedScore Score)
        {
            InitializeComponent();
            mp = m;
            OriginalScore = Score;
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

        private void EditScore_Clicked(object sender, EventArgs e)
        {
            var NewScore = new DetailedScore();
            NewScore.Score = int.Parse(SelectedScore.Text);
            NewScore.Date = SelectedScoreDate.Date;
            NewScore.RoundType = SelectedRoundType.SelectedItem.ToString();

            if (NewScore != OriginalScore)
            {
                var editingscore = App.Database.GetScore(NewScore.ScoreID);
                editingscore.Score = NewScore.Score;
                editingscore.Date = NewScore.Date;
                editingscore.RoundType = SelectedRoundType.SelectedItem.ToString();

                App.Database.UpdateScore(editingscore);
                App.Database.CalculateHandicap(NewScore.RoundType);
                mp.RefreshScoreList();
                PopupNavigation.Instance.PopAllAsync();
            }
        }
    }
}