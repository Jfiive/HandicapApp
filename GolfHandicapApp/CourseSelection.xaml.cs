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
    public partial class CourseSelection : ContentPage
    {
        private List<Course> mycourses = new List<Course>();
        public List<Course> MyCourses { get { return mycourses; } }
        public CourseSelection()
        {
            InitializeComponent();
            mycourses = App.Database.GetCourses();
            CourseList.ItemsSource = mycourses;
            ScoreDate.Date = DateTime.Today;
        }

        private void AddCourseButton_Clicked(object sender, EventArgs e)
        {
            var NewCourse = new Course();
            NewCourse.Name = CourseName.Text;
            NewCourse.Tee = TeeName.Text;
            NewCourse.Slope = decimal.Parse(CourseSlope.Text);
            NewCourse.Rating = decimal.Parse(CourseRating.Text);

            App.Database.SaveCourse(NewCourse);
            CourseName.Text = "";
            TeeName.Text = "";
            CourseSlope.Text = "";
            CourseRating.Text = "";

            EnterCoursePopup.IsVisible = false;
            mycourses = App.Database.GetCourses();
            CourseList.ItemsSource = mycourses;
            //needs to reload the course selection list that will be here once it is done
        }

        private void OpenPopup_Clicked(object sender, EventArgs e)
        {
            //might want to add an ActivityIndicator which is just a loading gif so that when this is up people know it is loading something
            //might also want to make it so that the popup closes if they click somewhere outside the popup
            EnterCoursePopup.IsVisible = true;
        }

        private void CourseList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //this needs to open the enter score popup so the person can post a score
            var grid = (ListView)sender;
            var selectedCourse = (Course)grid.SelectedItem;
        }

        private void NineHoles_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (EighteenHoles.IsChecked)
            {
                EighteenHoles.IsChecked = false;
            }
        }

        private void EighteenHoles_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (NineHoles.IsChecked)
            {
                NineHoles.IsChecked = false;
            }
        }
    }
}