﻿using dotMorten.Xamarin.Forms;
using Rg.Plugins.Popup.Services;
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
    public partial class EnterCoursePopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public CourseSelection mp;
        public int SelectedCourseID = 0;
        public EnterCoursePopup(CourseSelection m)
        {
            InitializeComponent();
            mp = m;
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
            SelectedCourseID = item.CourseID;
            sender.Text = item.Name;
            ValidityCheck();
        }

        private void AutoComplete_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = App.Database.SearchCourses(sender.Text);
            }
        }
        private void AddCourseButton_Clicked(object sender, EventArgs e)
        {
            var NewCourse = new PlayedCourse();
            NewCourse.CourseID = SelectedCourseID;

            if (Preferences.Get("SeasonsView", 0) > 0)
            {
                NewCourse.Season = Preferences.Get("SeasonsView", 0);
            }
            else
            {
                NewCourse.Season = Preferences.Get("LatestSeason", 0);
            }
            App.Database.SaveCourse(NewCourse);
            //mp.RefreshCourseList();
            PopupNavigation.Instance.PopAllAsync();
            (Application.Current.MainPage as MainPage).ClickPage("Post a Score");
        }
        private void ValidityCheck()
        {
            if (SelectedCourseID > 0)
            {
                AddCourseButton.IsEnabled = true;
            }
            else
            {
                AddCourseButton.IsEnabled = false;
            }
        }
    }
}