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
    public partial class HandicapHistory : ContentPage
    {
        private List<Handicap> handicaplistdata = new List<Handicap>();
        public List<Handicap> HandicapListData { get { return handicaplistdata; } }
        public HandicapHistory()
        {
            InitializeComponent();
            handicaplistdata = App.Database.GetHandicaps();
            HandicapHistoryList.ItemsSource = handicaplistdata;
        }
    }
}