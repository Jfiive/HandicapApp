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
        public List<DetailedScore> GetPastScores(string RoundType)
        {
            if (RoundType == "18")
            {
                return _database.Query<DetailedScore>("SELECT Scores.ScoreID, Scores.Date, Scores.Score, Scores.Differential, Scores.RoundType, Scores.UsedForCalc, Course.Name, Course.Rating, Course.Slope, Course.Tee FROM Scores LEFT JOIN Course ON Scores.CourseID = Course.CourseID WHERE Scores.RoundType = '18' ORDER BY Scores.Date DESC");
            }
            else
            {
                return _database.Query<DetailedScore>("SELECT Scores.ScoreID, Scores.Date, Scores.Score, Scores.Differential, Scores.RoundType, Scores.UsedForCalc, Course.Name, Course.Rating, Course.Slope, Course.Tee FROM Scores LEFT JOIN Course ON Scores.CourseID = Course.CourseID WHERE Scores.RoundType = 'Front' OR Scores.RoundType = 'Back' ORDER BY Scores.Date DESC");
            }
        }
        public void UpdateLowestScoreFlags(int Number, string RoundType)
        {
            if (RoundType == "18")
            {
                _database.Execute("UPDATE Scores SET UsedForCalc = 0 WHERE RoundType = '18'");
                var scorelist = _database.Table<Scores>().Where(s => s.RoundType == "18").ToList();
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
            else
            {
                _database.Execute("UPDATE Scores SET UsedForCalc = 0 WHERE RoundType = 'Front' OR RoundType = 'Back'");
                var scorelist = _database.Table<Scores>().Where(s => s.RoundType == "Front" || s.RoundType == "Back").ToList();
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
        public int GetNumberOfScores(string RoundType)
        {
            if (RoundType == "Front" || RoundType == "Back")
            {
                return _database.Table<Scores>().Where(s => s.RoundType == "Front" || s.RoundType == "Back").Count();
            }
            else
            {
                return _database.Table<Scores>().Where(s => s.RoundType == "18").Count();
            }
        }
        public List<Scores> GetLowestScores(int Number)
        {
            return _database.Table<Scores>().OrderBy(o => o.Score).Take(Number).ToList();
        }
        public List<decimal> GetLowestScoresDifferentials(int Number, string RoundType)
        {
            var scorelist = new List<Scores>();
            if (RoundType == "18")
            {
                scorelist = _database.Table<Scores>().Where(s => s.RoundType == "18").ToList();
            }
            else
            {
                scorelist = _database.Table<Scores>().Where(s => s.RoundType == "Front" || s.RoundType == "Back").ToList();
            }

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
        public void CalculateHandicap9()
        {

        }
        public void CalculateHandicap(string RoundType)
        {
            var ScoreCount = GetNumberOfScores(RoundType);
            if (ScoreCount < 5)
            {
                return;
            }

            var ScoresToUse = GetNumberOfScoresToUse(ScoreCount);
            var LowestScores = GetLowestScoresDifferentials(ScoresToUse, RoundType);
            UpdateLowestScoreFlags(ScoresToUse, RoundType);
            var handicap = LowestScores.Average() * 0.96m;
            //eventually make every decimal in the database to be a double since the handicap has to be a double and itll make things a lot easier
            handicap = Convert.ToDecimal(handicap.ToString("0.#"));
            if (RoundType == "18")
            {
                if (Preferences.ContainsKey("Handicap18"))
                {
                    //only insert the handicap into the handicap history table if it is different than the current handicap
                    if (Convert.ToDouble(handicap) != Preferences.Get("Handicap18", -1.0))
                    {
                        var hdcp = new Handicap
                        {
                            Date = DateTime.Today,
                            Number = handicap,
                            Type = RoundType
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
                        Number = handicap,
                        Type = RoundType
                    };
                    Preferences.Set("Handicap18", Convert.ToDouble(handicap));
                    SaveHandicap(hdcp);
                }
            }
            else
            {
                if (Preferences.ContainsKey("Handicap9"))
                {
                    if (Convert.ToDouble(handicap) != Preferences.Get("Handicap9", -1.0))
                    {
                        var hdcp = new Handicap
                        {
                            Date = DateTime.Today,
                            Number = handicap,
                            Type = RoundType
                        };
                        Preferences.Set("Handicap9", Convert.ToDouble(handicap));
                        SaveHandicap(hdcp);
                    }
                }
                else
                {
                    //first time a user has gotten 5 scores to get a handicap
                    var hdcp = new Handicap
                    {
                        Date = DateTime.Today,
                        Number = handicap,
                        Type = RoundType
                    };
                    Preferences.Set("Handicap9", Convert.ToDouble(handicap));
                    SaveHandicap(hdcp);
                }
            }
        }
        private int GetNumberOfScoresToUse(int ScoreCount)
        {
            switch (ScoreCount)
            {
                case 5:
                case 6:
                    return 1;

                case 7:
                case 8:
                    return 2;

                case 9:
                case 10:
                    return 3;

                case 11:
                case 12:
                    return 4;

                case 13:
                case 14:
                    return 5;

                case 15:
                case 16:
                    return 6;

                case 17:
                    return 7;

                case 18:
                    return 8;

                case 19:
                    return 9;

                default:
                    if (ScoreCount >= 20)
                    {
                        return 10;
                    }
                    else
                    {
                        return 0;
                    }
            }
        }
    }
}
