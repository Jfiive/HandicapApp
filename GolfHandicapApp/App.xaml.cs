using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using System.Linq;
using Xamarin.Essentials;

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
            if (!Preferences.ContainsKey("DisplayEUDate"))
            {
                Preferences.Set("DisplayEUDate", false);
            }
            if (!Preferences.ContainsKey("HighlightScores"))
            {
                Preferences.Set("HighlightScores", false);
            }
            if (!Preferences.ContainsKey("HandicapDisplayType"))
            {
                Preferences.Set("HandicapDisplayType", 18);
            }
            MainPage = new MainPage();
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
