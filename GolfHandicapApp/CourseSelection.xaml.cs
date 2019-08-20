using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using dotMorten.Xamarin.Forms;
using Rg.Plugins.Popup.Services;

namespace GolfHandicapApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseSelection : ContentPage
    {
        private List<DisplayCourse> mycourses = new List<DisplayCourse>();
        public List<DisplayCourse> MyCourses { get { return mycourses; } }
        public CourseSelection()
        {
            InitializeComponent();
            mycourses = App.Database.GetPlayedCourses();
            CourseList.ItemsSource = mycourses;
            if (mycourses.Count == 0)
            {
                //EnterCoursePopup.IsVisible = true;
                PopupNavigation.Instance.PushAsync(new EnterCoursePopup(this));
                Title = "Add New Course";
            }
        }
        public void DeselectCourse()
        {
            CourseList.SelectedItem = null;
        }
        public void RefreshCourseList()
        {
            mycourses = App.Database.GetPlayedCourses();
            CourseList.ItemsSource = null;
            CourseList.ItemsSource = mycourses;
            if (mycourses.Count == 1)
            {
                //change the title back to what it normally is
                Title = "Select a Course";
            }
        }
        public DisplayCourse GetSelectedCourse()
        {
            return (DisplayCourse)CourseList.SelectedItem;
        }

        private void OpenPopup_Clicked(object sender, EventArgs e)
        {
            //EnterCoursePopup.IsVisible = true;
            PopupNavigation.Instance.PushAsync(new EnterCoursePopup(this));
        }

        private void CourseList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //CourseClickMenu.IsVisible = true;
            PopupNavigation.Instance.PushAsync(new CourseClickPopup(this));
        }
    }
}