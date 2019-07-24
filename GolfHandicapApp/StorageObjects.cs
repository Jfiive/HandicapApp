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
        [PrimaryKey, AutoIncrement]
        public int CourseID { get; set; }
        public string Name { get; set; }
        public string Tee { get; set; }
        public decimal Rating { get; set; }
        public decimal Slope { get; set; }
    }
    
    public class Scores
    {
        [PrimaryKey, AutoIncrement]
        public int ScoreID { get; set; }
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public decimal Differential { get; set; }
        public int RoundType { get; set; }
        public int CourseID { get; set; }
    }
    public class Handicap
    {
        [PrimaryKey, AutoIncrement]
        public int HandicapID { get; set; }
        public DateTime Date { get; set; }
        public decimal Number { get; set; }
    }
    public class DetailedScore
    {
        [PrimaryKey, AutoIncrement]
        public int DetailedScoreID { get; set; }
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public int RoundType { get; set; }
        public decimal Rating { get; set; }
        public decimal Slope { get; set; }
    }
}
