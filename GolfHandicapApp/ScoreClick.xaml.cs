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
            switch (e.SelectedItemIndex)
            {
                case 0: //edit score
                    PopupNavigation.Instance.PushAsync(new EditScorePopup(mp, selectedScore));
                    break;

                case 1: //delete score
                    App.Database.DeleteScore(selectedScore.ScoreID);
                    mp.RefreshScoreList();
                    PopupNavigation.Instance.PopAsync();
                    break;
            }
        }
    }
}