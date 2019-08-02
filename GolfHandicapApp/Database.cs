using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;
using SQLite;
using System.Linq;

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
            return _database.Table<Handicap>().OrderByDescending(o => o.Date).ToList();
        }
        public int SaveHandicap(Handicap handicap)
        {
            return _database.Insert(handicap);
        }
        public List<DetailedScore> GetPastScores()
        {
            return _database.Query<DetailedScore>("SELECT Scores.ScoreID, Scores.Date, Scores.Score, Scores.Differential, Scores.RoundType, Scores.UsedForCalc, Course.Name, Course.Rating, Course.Slope, Course.Tee FROM Scores LEFT JOIN Course ON Scores.CourseID = Course.CourseID ORDER BY Scores.Date DESC"); ;
        }
        public void UpdateLowestScoreFlags(int Number)
        {
            _database.Execute("UPDATE Scores SET UsedForCalc = 0");
            var scorelist = _database.Table<Scores>().ToList();
            if (Number < 10)
            {
                //this turns the scorelist into the lowest X number of scores
                scorelist = scorelist.OrderBy(o => o.Differential).Take(Number).ToList();
            }
            else
            {
                //needs to take the last 20 scores instead of using all the scores that are available
                scorelist = scorelist.OrderByDescending(o => o.ScoreID).Take(20).ToList();
                scorelist = scorelist.OrderBy(o => o.Differential).Take(Number).ToList();
            }
            foreach (var item in scorelist)
            {
                item.UsedForCalc = true;
                UpdateScore(item);
            }
        }
        public int SaveScore(Scores score)
        {
            return _database.Insert(score);
        }
        public int UpdateCourse (Course course)
        {
            return _database.Update(course);
        }
        public int UpdateScore (Scores score)
        {
            return _database.Update(score);
        }
        public Scores GetScore(int ID)
        {
            return _database.Table<Scores>().FirstOrDefault(o => o.ScoreID == ID);
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
            var scorelist = _database.Table<Scores>().ToList();
            if (Number < 10)
            {
                //this turns the scorelist into the lowest X number of scores
                scorelist = scorelist.OrderBy(o => o.Differential).Take(Number).ToList();
            }
            else
            {
                //needs to take the last 20 scores instead of using all the scores that are available
                scorelist = scorelist.OrderByDescending(o => o.ScoreID).Take(20).ToList();
                scorelist = scorelist.OrderBy(o => o.Differential).Take(Number).ToList();
            }
            return scorelist.Select(o => o.Differential).ToList();
        }
        public int DeleteCourse(int ID)
        {
            return _database.Delete<Course>(ID);
        }
        public int DeleteScore(int ID)
        {
            return _database.Delete<Scores>(ID);
        }
        public void CalculateHandicap()
        {
            var ScoreCount = GetNumberOfScores();
            if (ScoreCount < 5)
            {
                return;
            }

            var ScoresToUse = 0;
            switch (ScoreCount)
            {
                case 5:
                case 6:
                    ScoresToUse = 1;
                    break;

                case 7:
                case 8:
                    ScoresToUse = 2;
                    break;

                case 9:
                case 10:
                    ScoresToUse = 3;
                    break;

                case 11:
                case 12:
                    ScoresToUse = 4;
                    break;

                case 13:
                case 14:
                    ScoresToUse = 5;
                    break;

                case 15:
                case 16:
                    ScoresToUse = 6;
                    break;

                case 17:
                    ScoresToUse = 7;
                    break;

                case 18:
                    ScoresToUse = 8;
                    break;

                case 19:
                    ScoresToUse = 9;
                    break;

                default:
                    if (ScoreCount >= 20)
                    {
                        ScoresToUse = 10;
                    }
                    break;
            }

            var LowestScores = GetLowestScoresDifferentials(ScoresToUse);
            UpdateLowestScoreFlags(ScoresToUse);
            var handicap = LowestScores.Average() * 0.96m;
            //eventually make every decimal in the database to be a double since the handicap has to be a double and itll make things a lot easier
            handicap = Convert.ToDecimal(handicap.ToString("0.#"));
            //needs to also take into account for 9 hole handicap scores as well eventually
            if (Preferences.ContainsKey("Handicap18"))
            {
                //only insert the handicap into the handicap history table if it is different than the current handicap
                if (Convert.ToDouble(handicap) != Preferences.Get("Handicap18", -1.0))
                {
                    var hdcp = new Handicap
                    {
                        Date = DateTime.Today,
                        Number = handicap
                    };
                    SaveHandicap(hdcp);
                }
                Preferences.Set("Handicap18", Convert.ToDouble(handicap));
            }
            else
            {
                //this is the first time the user has gotten 5 scores to get a handicap calculated so insert the handicap as the first history
                var hdcp = new Handicap
                {
                    Date = DateTime.Today,
                    Number = handicap
                };
                Preferences.Set("Handicap18", Convert.ToDouble(handicap));
                SaveHandicap(hdcp);
            }
        }
    }
}
