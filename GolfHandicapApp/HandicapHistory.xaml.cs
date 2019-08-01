using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GolfHandicapApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HandicapHistory : ContentPage
    {
        private List<Handicap> handicaplistdata = new List<Handicap>();
        public List<Handicap> HandicapListData { get { return handicaplistdata; } }
        public HandicapHistory()
        {
            InitializeComponent();
            if (Preferences.ContainsKey("DisplayEUDate"))
            {
                if (Preferences.Get("DisplayEUDate", false))
                {
                    HandicapHistoryList.ItemTemplate = (DataTemplate)Resources["EUDate"];
                }
                else
                {
                    HandicapHistoryList.ItemTemplate = (DataTemplate)Resources["USDate"];
                }
            }
            else
            {
                HandicapHistoryList.ItemTemplate = (DataTemplate)Resources["USDate"];
            }
            handicaplistdata = App.Database.GetHandicaps();
            HandicapHistoryList.ItemsSource = handicaplistdata;
        }
    }
}