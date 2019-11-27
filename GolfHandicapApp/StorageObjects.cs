using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace GolfHandicapApp
{
    //How to save the data if sqlite isnt going to be used. Example of how you would get the properties:
    //var scores = App.Current.Properties["Scores"] as List<PastScore>;

    //how you would save when something new is inserted:
    //Application.Current.SavePropertiesAsync();
    //lists can be joined so there might not be a need to make an entire sql database in order to do this since there shouldnt be that much information thats going to be stored in the app
    public class Course
    {
        //in the future might want to add the front and the back individual ratings/slope and not just the 18 hole slopes and ratings
        [PrimaryKey, AutoIncrement]
        public int CourseID { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
    public class PlayedCourse
    {
        [PrimaryKey, AutoIncrement]
        public int PlayedID { get; set; }
        public int CourseID { get; set; }
    }
    public class TeeInfo
    {
        [PrimaryKey, AutoIncrement]
        public int InfoID { get; set; }
        public int CourseID { get; set; }
        public string TeeName { get; set; }
        public double Rating { get; set; }
        public int Slope { get; set; }
        public double FrontRating { get; set; }
        public int FrontSlope { get; set; }
        public double BackRating { get; set; }
        public int BackSlope { get; set; }
        public string Gender { get; set; }
    }
    
    public class Scores
    {
        [PrimaryKey, AutoIncrement]
        public int ScoreID { get; set; }
        public int InfoID { get; set; }
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public double Differential { get; set; }
        public string RoundType { get; set; }
        public int PlayedID { get; set; }
        public bool UsedForCalc { get; set; }
    }
    public class Handicap
    {
        [PrimaryKey, AutoIncrement]
        public int HandicapID { get; set; }
        public DateTime Date { get; set; }
        public double Number { get; set; }
        public string Type { get; set; }
    }
    public class DetailedScore
    {
        //this is not a table and is only used to store the joined query results into an object so no primary key/ auto increment needed
        public int ScoreID { get; set; }
        public int CourseID { get; set; }
        public int InfoID { get; set; }
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public double Differential { get; set; }
        public string RoundType { get; set; }
        public bool UsedForCalc { get; set; }
        public string Name { get; set; }
        public string TeeName { get; set; }
    }
    public class PickerTee
    {
        public int InfoID { get; set; }
        public int CourseID { get; set; }
        public string TeeName { get; set; }
        public double Rating { get; set; }
        public int Slope { get; set; }
        public double FrontRating { get; set; }
        public double BackRating { get; set; }
        public string DisplayName { get; set; }
    }
    public class DisplayCourse
    {
        public int PlayedID { get; set; }
        public int CourseID { get; set; }
        public string Name { get; set; }
        public string TeeName { get; set; }
        public double Rating { get; set; }
        public int Slope { get; set; }
        public DateTime Date { get; set; }
    }
}
