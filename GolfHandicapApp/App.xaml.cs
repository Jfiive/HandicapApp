﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using System.Linq;
using Xamarin.Essentials;

namespace GolfHandicapApp
{
    public partial class App : Application
    {
        static Database database;

        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HandicapDB.db"));
                    }
                    else if (Device.RuntimePlatform == Device.iOS)
                    {
                        string libFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", "Databases");
                        database = new Database(Path.Combine(libFolder, "HandicapDB.db"));
                    }
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
            if (!Preferences.ContainsKey("SeparateBySeason"))
            {
                Preferences.Set("SeparateBySeason", false);
            }
            if (!Preferences.ContainsKey("LatestSeason"))
            {
                Preferences.Set("LatestSeason", Database.GetLatestSeason());
            }
            //SeasonsView does not persist through app restart like all the other ones do so it needs to get set to nothing every time
            if (Preferences.Get("SeasonsView", 0) > 0)
            {
                Preferences.Set("SeasonsView", 0);
                Database.CalculateHandicap("18");
                Database.CalculateHandicap("9");
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
