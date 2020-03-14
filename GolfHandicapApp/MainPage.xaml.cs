using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace GolfHandicapApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        private List<string> menudata = new List<string>();
        public List<string> MenuData { get { return menudata; } }
        public MainPage()
        {
            InitializeComponent();
            Detail = new NavigationPage(new MyScores());
            IsPresented = false;
            SetMenuItems();
        }

        private void MenuList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var index = (string)e.SelectedItem;
            MenuList.SelectedItem = null;
            switch (index)
            {
                case "Your Scores":
                    Detail = new NavigationPage(new MyScores());
                    IsPresented = false;
                    break;

                case "Post a Score":
                    Detail = new NavigationPage(new CourseSelection());
                    IsPresented = false;
                    break;

                case "Handicap History":
                    Detail = new NavigationPage(new HandicapHistory());
                    IsPresented = false;
                    break;

                case "Settings":
                    Detail = new NavigationPage(new Settings());
                    IsPresented = false;
                    break;

                case "Past Seasons":
                    Detail = new NavigationPage(new PastSeasons());
                    IsPresented = false;
                    break;
            }
        }

        public void SetMenuItems()
        {
            if (Preferences.Get("SeparateBySeason", false))
            {
                menudata = new List<string>() { "Your Scores", "Post a Score", "Handicap History", "Past Seasons", "Settings" };
            }
            else
            {
                menudata = new List<string>() { "Your Scores", "Post a Score", "Handicap History", "Settings" };
            }
            MenuList.ItemsSource = menudata;
        }

        public void ClickPage(string PageName)
        {
            MenuList.SelectedItem = null;
            switch (PageName)
            {
                case "Your Scores":
                    Detail = new NavigationPage(new MyScores());
                    IsPresented = false;
                    break;

                case "Post a Score":
                    Detail = new NavigationPage(new CourseSelection());
                    IsPresented = false;
                    break;

                case "Handicap History":
                    Detail = new NavigationPage(new HandicapHistory());
                    IsPresented = false;
                    break;

                case "Settings":
                    Detail = new NavigationPage(new Settings());
                    IsPresented = false;
                    break;

                case "Past Seasons":
                    Detail = new NavigationPage(new PastSeasons());
                    IsPresented = false;
                    break;
            }
        }
    }
}
