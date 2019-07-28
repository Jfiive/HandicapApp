using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GolfHandicapApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
            Detail = new NavigationPage(new MyScores());
            IsPresented = false;
        }

        private void MenuList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            switch (e.SelectedItemIndex)
            {
                case 0: //My Scores
                    Detail = new NavigationPage(new MyScores());
                    IsPresented = false;
                    break;

                case 1: //Post a Score
                    Detail = new NavigationPage(new CourseSelection());
                    IsPresented = false;
                    break;

                case 2: //Handicap History
                    Detail = new NavigationPage(new HandicapHistory());
                    IsPresented = false;
                    break;

                case 3: //Settings
                    Detail = new NavigationPage(new Settings());
                    IsPresented = false;
                    break;
            }
        }
    }
}
