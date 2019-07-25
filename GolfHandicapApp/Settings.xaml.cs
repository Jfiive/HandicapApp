using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace GolfHandicapApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            //use the Xamarin.Essentials.Preferences class to store settings data. These settings persist and will only be cleared if said so or if the user uninstalls without android back up which is on by default
            InitializeComponent();
        }
    }
}