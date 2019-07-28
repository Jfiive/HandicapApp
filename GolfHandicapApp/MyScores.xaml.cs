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
            //if (Application.Current.Properties["Handicap18"] != null)
            //{
            //    CurrentHandicapLabel.Text = Application.Current.Properties["Handicap18"].ToString();
            //}
        }
    }
}