using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SQLite;

namespace GolfHandicapApp
{
    public class Database
    {
        readonly SQLiteConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Course>();
            _database.CreateTable<Scores>();
            _database.CreateTable<Handicap>();
        }
        //general database actions
        public List<Course> GetCourses()
        {
            return _database.Table<Course>().ToList();
        }
        public int SaveCourse(Course course)
        {
            return _database.Insert(course);
        }
        public List<Handicap> GetHandicaps()
        {
            return _database.Table<Handicap>().ToList();
        }
        public int SaveHandicap(Handicap handicap)
        {
            return _database.Insert(handicap);
        }
        public List<DetailedScore> GetPastScores()
        {
            return _database.Query<DetailedScore>("SELECT Scores.ScoreID, Scores.Date, Scores.Score, Scores.Differential, Scores.RoundType, Course.Name, Course.Rating, Course.Slope, Course.Tee FROM Scores LEFT JOIN Course ON Scores.CourseID = Course.CourseID"); ;
        }
        public int SaveScore(Scores score)
        {
            return _database.Insert(score);
        }
        public int UpdateCourse (Course course)
        {
            return _database.Update(course);
        }
        public decimal GetCurrentHandicap()
        {
            return _database.Table<Handicap>().OrderByDescending(o => o.Date).FirstOrDefault().Number;
        }
        public int GetNumberOfScores()
        {
            return _database.Table<Scores>().Count();
        }
        public List<Scores> GetLowestScores(int Number)
        {
            return _database.Table<Scores>().OrderBy(o => o.Score).Take(Number).ToList();
        }
        public List<decimal> GetLowestScoresDifferentials(int Number)
        {
            return _database.Query<decimal>("SELECT TOP " + Number + " Differential FROM Scores ORDER BY Score");
        }
        public int DeleteCourse(int ID)
        {
            return _database.Delete<Course>(ID);
        }
        public int DeleteScore(int ID)
        {
            return _database.Delete<Scores>(ID);
        }
    }
}
