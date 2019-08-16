using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using dotMorten.Xamarin.Forms;

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
            ScoreDate.Date = DateTime.Today;
            if (mycourses.Count == 0)
            {
                EnterCoursePopup.IsVisible = true;
                Title = "Add New Course";
            }
            SelectedRoundType.SelectedIndex = 0;
            //get the location of the phone and get all the courses that are in the same state/area if possible
        }

        private void AddCourseButton_Clicked(object sender, EventArgs e)
        {
            var NewCourse = new PlayedCourse();
            var TeeInfo = (PickerTee)TeePicker.SelectedItem;
            NewCourse.CourseID = TeeInfo.CourseID;
            NewCourse.InfoID = TeeInfo.InfoID;
            App.Database.SaveCourse(NewCourse);

            StatePicker.SelectedItem = null;
            AutoComplete.Text = "";
            TeePicker.SelectedItem = null;
            AutoComplete.IsEnabled = false;
            TeePicker.IsEnabled = false;

            EnterCoursePopup.IsVisible = false;
            mycourses = App.Database.GetPlayedCourses();
            CourseList.ItemsSource = mycourses;
            if (mycourses.Count == 1)
            {
                //change the title back to what it normally is
                Title = "Select a Course";
            }
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
                    var selectedCourse = (DisplayCourse)CourseList.SelectedItem;
                    App.Database.DeleteCourse(selectedCourse.PlayedID);
                    //delete played course
                    mycourses = App.Database.GetPlayedCourses();
                    CourseList.ItemsSource = mycourses;
                    break;
            }
            CourseSelectMenu.SelectedItem = null;
        }

        private void PostScore_Clicked(object sender, EventArgs e)
        {
            var score = new Scores();
            var selectedCourse = (DisplayCourse)CourseList.SelectedItem;
            var teeinfo = App.Database.GetTeeInfo(selectedCourse.PlayedID);
            score.Score = int.Parse(EnteredScore.Text);
            score.Date = ScoreDate.Date;
            score.RoundType = SelectedRoundType.SelectedItem.ToString();
            if (score.RoundType == "18")
            {
                score.Differential = Math.Round((score.Score - selectedCourse.Rating) * 113 / selectedCourse.Slope, 2);
            }
            else if (score.RoundType == "Front")
            {
                score.Differential = Math.Round((score.Score - teeinfo.FrontRating) * 113 / selectedCourse.Slope, 2);
            }
            else if (score.RoundType == "Back")
            {
                //some courses dont have a back rating so need to remove the back rating setting if they dont
                score.Differential = Math.Round((score.Score - teeinfo.BackRating) * 113 / selectedCourse.Slope, 2);
            }
            score.PlayedID = selectedCourse.PlayedID;
            App.Database.SaveScore(score);
            App.Database.CalculateHandicap(score.RoundType);
            SelectedRoundType.SelectedIndex = 0;

            //Navigation.PushAsync(new MyScores());
            ((App.Current.MainPage as MasterDetailPage).Detail as NavigationPage).Navigation.PushAsync(new MyScores());
            (App.Current.MainPage as MasterDetailPage).IsPresented = false;
        }

        private void StatePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (StatePicker.SelectedItem != null)
            {
                AutoComplete.ItemsSource = App.Database.GetCoursesByState(StatePicker.SelectedItem.ToString());
                AutoComplete.IsEnabled = true;
            }
        }

        private void AutoComplete_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            var item = (Course)e.SelectedItem;
            sender.Text = item.Name;
            TeePicker.ItemsSource = App.Database.GetCourseTees(item.CourseID);
            TeePicker.ItemDisplayBinding = new Binding("DisplayName");
            TeePicker.IsEnabled = true;
        }

        private void AutoComplete_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = App.Database.SearchCourses(sender.Text);
            }
        }
    }
}