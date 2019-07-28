using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using System.Linq;

namespace GolfHandicapApp
{
    public partial class App : Application
    {
        //possibly use Application.Current.Properties ( Application.Current.Properties["Scores"] as an example ) instead of a database like this
        //would need to call Application.Current.SavePropertiesAsync() after changing something so it would be saved immediately otherwise it wont be saved until the application gets closed out or goes to sleep mode
        static Database database;

        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    //this is for android the connection string needs to be different for iOS but that will come at some other time
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HandicapDB.db3"));
                }
                return database;
            }
        }
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
        public void CalculateHandicap()
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
            handicap = (Math.Floor(handicap * 100) / 100);
            //handicap = handicap - (handicap % 0.01m);
            Current.Properties["Handicap18"] = handicap;
            //if (HandicapType == 18)
            //{
            //    Current.Properties["Handicap18"] = handicap;
            //}
            //else
            //{
            //    Current.Properties["Handicap9"] = handicap;
            //}
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
