using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GolfHandicapApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseClickPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public CourseSelection mp;
        public CourseClickPopup(CourseSelection m)
        {
            InitializeComponent();
            mp = m;
        }

        private void CourseSelectMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            switch (e.SelectedItemIndex)
            {
                case 0: //Post a Score
                    PopupNavigation.Instance.PushAsync(new PostScorePopup(mp, this));
                    break;

                case 1: //Delete Course
                    var selectedCourse = mp.GetSelectedCourse();
                    App.Database.DeleteCourse(selectedCourse.PlayedID);
                    mp.RefreshCourseList();
                    PopupNavigation.Instance.PopAsync();
                    break;
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            mp.DeselectCourse();
        }
        public void DeselectItem()
        {
            CourseSelectMenu.SelectedItem = null;
        }
    }
}