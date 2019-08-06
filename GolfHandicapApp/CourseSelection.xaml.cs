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
            if (mycourses.Count == 0)
            {
                EnterCoursePopup.IsVisible = true;
                Title = "Add New Course";
            }
            SelectedRoundType.SelectedIndex = 0;
        }

        private void AddCourseButton_Clicked(object sender, EventArgs e)
        {
            var NewCourse = new Course();
            NewCourse.Name = CourseName.Text;
            NewCourse.Tee = TeeName.Text;
            var enteredSlope = decimal.Parse(CourseSlope.Text);
            var enteredRating = decimal.Parse(CourseRating.Text);
            if (enteredRating < 48)
            {
                enteredRating = 48;
            }
            if (enteredRating > 85)
            {
                enteredRating = 85;
            }
            if (enteredSlope < 55)
            {
                enteredSlope = 55;
            }
            if (enteredSlope > 155)
            {
                enteredSlope = 155;
            }
            NewCourse.Slope = enteredSlope;
            NewCourse.Rating = enteredRating;

            App.Database.SaveCourse(NewCourse);
            CourseName.Text = "";
            TeeName.Text = "";
            CourseSlope.Text = "";
            CourseRating.Text = "";

            EnterCoursePopup.IsVisible = false;
            mycourses = App.Database.GetCourses();
            CourseList.ItemsSource = mycourses;
            if (mycourses.Count == 1)
            {
                //change the title back to what it normally is
                Title = "Select a Course";
            }
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
            CourseClickMenu.IsVisible = true;
        }

        private void ClosePopup(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;
            if (button.ClassId == "CloseEnterCourse")
            {
                EnterCoursePopup.IsVisible = false;
            }
            else if (button.ClassId == "CloseEnterScore")
            {
                EnterScorePopup.IsVisible = false;
                CourseList.SelectedItem = null;
                SelectedRoundType.SelectedIndex = 0;
                EnteredScore.Text = null;
                ScoreDate.Date = DateTime.Today;
            }
            else if (button.ClassId == "CloseCourseClickMenu")
            {
                CourseClickMenu.IsVisible = false;
                CourseList.SelectedItem = null;
            }
        }

        private void CourseSelectMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            CourseClickMenu.IsVisible = false;
            switch (e.SelectedItemIndex)
            {
                case 0: //Post a Score
                    EnterScorePopup.IsVisible = true;
                    break;

                case 1: //Delete Course
                    var selectedCourse = (Course)CourseList.SelectedItem;
                    App.Database.DeleteCourse(selectedCourse.CourseID);
                    mycourses = App.Database.GetCourses();
                    CourseList.ItemsSource = mycourses;
                    break;
            }
            CourseSelectMenu.SelectedItem = null;
        }

        private void PostScore_Clicked(object sender, EventArgs e)
        {
            var score = new Scores();
            var selectedCourse = (Course)CourseList.SelectedItem;
            score.Score = int.Parse(EnteredScore.Text);
            score.Date = ScoreDate.Date;
            score.RoundType = SelectedRoundType.SelectedItem.ToString();
            if (score.RoundType == "18")
            {
                score.Differential = Math.Round((score.Score - selectedCourse.Rating) * 113 / selectedCourse.Slope, 2);
            }
            else
            {
                //9 hole calculations only use half the course rating
                score.Differential = Math.Round((score.Score - (selectedCourse.Rating / 2)) * 113 / selectedCourse.Slope, 2);
            }
            score.CourseID = selectedCourse.CourseID;
            App.Database.SaveScore(score);
            App.Database.CalculateHandicap(score.RoundType);
            SelectedRoundType.SelectedIndex = 0;

            //Navigation.PushAsync(new MyScores());
            ((App.Current.MainPage as MasterDetailPage).Detail as NavigationPage).Navigation.PushAsync(new MyScores());
            (App.Current.MainPage as MasterDetailPage).IsPresented = false;
        }
    }
}