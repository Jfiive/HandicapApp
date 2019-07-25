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
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            //this should load the users current scores and put them in the blank space below the buttons
        }

        private async void PostScoreButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CourseSelection());
        }

        private async void HandicapHistoryButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HandicapHistory());
        }

        private async void SettingsButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings());
        }
    }
}
