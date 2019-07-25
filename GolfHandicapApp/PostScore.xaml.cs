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
    public partial class PostScore : ContentPage
    {
        public PostScore(Course SelectedCourse)
        {
            InitializeComponent();
        }

        private void PostNewScore(Scores NewScore)
        {
            
        }

        private void CalculateHandicap(int HandicapType)
        {
            var ScoreCount = App.Database.GetNumberOfScores();
            if (ScoreCount < 5)
            {
                return;
            }

            var ScoresToUse = 0;
            switch (ScoreCount)
            {
                case 5:
                case 6:
                    ScoresToUse = 1;
                    break;

                case 7:
                case 8:
                    ScoresToUse = 2;
                    break;

                case 9:
                case 10:
                    ScoresToUse = 3;
                    break;

                case 11:
                case 12:
                    ScoresToUse = 4;
                    break;

                case 13:
                case 14:
                    ScoresToUse = 5;
                    break;

                case 15:
                case 16:
                    ScoresToUse = 6;
                    break;

                case 17:
                    ScoresToUse = 7;
                    break;

                case 18:
                    ScoresToUse = 8;
                    break;

                case 19:
                    ScoresToUse = 9;
                    break;

                default:
                    if (ScoreCount >= 20)
                    {
                        ScoresToUse = 10;
                    }
                    break;
            }

            var LowestScores = App.Database.GetLowestScoresDifferentials(ScoresToUse);
            var handicap = LowestScores.Average() * 0.96m;
            handicap = handicap - (handicap % 0.01m);
            if (HandicapType == 18)
            {
                Application.Current.Properties["CurrentHandicap18"] = handicap;
            }
            else
            {
                Application.Current.Properties["CurrentHandicap9"] = handicap;
            }
        }
    }
}