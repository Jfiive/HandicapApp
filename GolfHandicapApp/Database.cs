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
            _database.CreateTable<PlayedCourse>();
            _database.CreateTable<TeeInfo>();
        }
        //general database actions
        public List<Course> GetCoursesByState(string State)
        {
            return _database.Table<Course>().Where(c => c.State == State).ToList();
        }
        
        public List<DisplayCourse> GetPlayedCourses()
        {
            var output = _database.Query<DisplayCourse>("SELECT PlayedCourse.PlayedID, PlayedCourse.Season, Course.Name, Course.CourseID FROM PlayedCourse LEFT JOIN Course ON PlayedCourse.CourseID = Course.CourseID").ToList();
            if (Preferences.Get("SeparateBySeason", false) == true)
            {
                int year;
                if (Preferences.Get("SeasonsView", 0) > 0)
                {
                    year = Preferences.Get("SeasonsView", 0);
                    output = output.Where(c => c.Season == year).GroupBy(c => c.Name).Select(c => c.FirstOrDefault()).ToList();
                }
                else
                {
                    if (_database.Table<Scores>().Any())
                    {
                        year = Preferences.Get("LatestSeason", 0);
                        output = output.Where(c => c.Season == year).GroupBy(c => c.Name).Select(c => c.FirstOrDefault()).ToList();
                    }
                }
            }
            else
            {
                output = output.GroupBy(c => c.Name).Select(c => c.FirstOrDefault()).ToList();
            }
            return output;
        }
        public TeeInfo GetTeeInfo(int InfoID)
        {
            return _database.Table<TeeInfo>().Single(t => t.InfoID == InfoID);
        }
        public List<Course> SearchCourses(string Criteria)
        {
            return _database.Query<Course>("SELECT * FROM Course WHERE Name LIKE '%" + Criteria + "%'");
        }
        public List<PickerTee> GetCourseTees(int CourseID)
        {
            var tees = _database.Table<TeeInfo>().Where(t => t.CourseID == CourseID).ToList();
            var output = new List<PickerTee>();
            foreach (var item in tees)
            {
                var tee = new PickerTee()
                {
                    InfoID = item.InfoID,
                    CourseID = item.CourseID,
                    TeeName = item.TeeName,
                    Rating = item.Rating,
                    Slope = item.Slope,
                    FrontRating = item.FrontRating,
                    BackRating = item.BackRating,
                    DisplayName = item.TeeName + " (" + item.Gender + ")"
                };
                output.Add(tee);
            }
            return output;
        }
        public List<int> GetSeasons()
        {
            var currentSeason = Preferences.Get("LatestSeason", 0);
            var table = _database.Table<Scores>().ToList();
            return table.Where(s => s.Date.Year != currentSeason).Select(s => s.Date.Year).Distinct().ToList();
        }
        public void ResetApp()
        {
            _database.Execute("DELETE FROM Scores");
            _database.Execute("DELETE FROM Handicap");
            _database.Execute("DELETE FROM PlayedCourse");
        }
        public int SaveCourse(PlayedCourse course)
        {
            return _database.Insert(course);
        }
        public List<Handicap> GetHandicaps(int RoundType)
        {
            var output = new List<Handicap>();
            if (RoundType == 18)
            {
                output = _database.Table<Handicap>().Where(h => h.Type == "18").OrderByDescending(o => o.Date).ToList();
            }
            else
            {
                output = _database.Table<Handicap>().Where(h => h.Type == "Front" || h.Type == "Back").OrderByDescending(o => o.Date).ToList();
            }

            //filter more based on user settings
            if (Preferences.Get("SeparateBySeason", false) == true)
            {
                int year;
                if (Preferences.Get("SeasonsView", 0) > 0)
                {
                    year = Preferences.Get("SeasonsView", 0);
                    output = output.Where(h => h.Date.Year == year).ToList();
                }
                else
                {
                    year = Preferences.Get("LatestSeason", 0);
                    output = output.Where(h => h.Date.Year == year).ToList();
                }
            }
            return output;
        }
        public int SaveHandicap(Handicap handicap)
        {
            return _database.Insert(handicap);
        }
        public List<DetailedScore> GetPastScores(string RoundType)
        {
            var output = new List<DetailedScore>();
            if (RoundType == "18")
            {
                output = _database.Query<DetailedScore>("SELECT Scores.ScoreID, Scores.Date, Scores.Score, Scores.Differential, Scores.RoundType, Scores.UsedForCalc, Course.Name, Course.CourseID, TeeInfo.TeeName, TeeInfo.InfoID FROM Scores LEFT JOIN PlayedCourse ON Scores.PlayedID = PlayedCourse.PlayedID LEFT JOIN Course ON PlayedCourse.CourseID = Course.CourseID LEFT JOIN TeeInfo ON Scores.InfoID = TeeInfo.InfoID WHERE Scores.RoundType = '18' ORDER BY Scores.Date DESC");
            }
            else
            {
                output =  _database.Query<DetailedScore>("SELECT Scores.ScoreID, Scores.Date, Scores.Score, Scores.Differential, Scores.RoundType, Scores.UsedForCalc, Course.Name, Course.CourseID, TeeInfo.TeeName, TeeInfo.InfoID FROM Scores LEFT JOIN PlayedCourse ON Scores.PlayedID = PlayedCourse.PlayedID LEFT JOIN Course ON PlayedCourse.CourseID = Course.CourseID LEFT JOIN TeeInfo ON Scores.InfoID = TeeInfo.InfoID WHERE Scores.RoundType = 'Front' OR Scores.RoundType = 'Back' ORDER BY Scores.Date DESC");
            }

            //filter more based on user settings
            if (Preferences.Get("SeparateBySeason", false) == true)
            {
                int year;
                if (Preferences.Get("SeasonsView", 0) > 0)
                {
                    year = Preferences.Get("SeasonsView", 0);
                    output = output.Where(s => s.Date.Year == year).ToList();
                }
                else
                {
                    year = Preferences.Get("LatestSeason", 0);
                    output = output.Where(s => s.Date.Year == year).ToList();
                }
            }
            return output;
        }
        public DateTime GetLastEnteredScoreDate(string RoundType)
        {
            if (RoundType == "18")
            {
                return _database.Table<Scores>().Where(s => s.RoundType == "18").OrderByDescending(s => s.Date).First().Date;
            }
            else
            {
                return _database.Table<Scores>().Where(s => s.RoundType == "Front" || s.RoundType == "Back").OrderByDescending(s => s.Date).First().Date;
            }
        }
        public void UpdateLowestScoreFlags(int Number, string RoundType)
        {
            //gets the current season to be used below if it is needed
            var currentSeason = 0;
            if (Preferences.Get("SeparateBySeason", false) == true)
            {
                if (Preferences.Get("SeasonsView", 0) > 0)
                {
                    currentSeason = Preferences.Get("SeasonsView", 0);
                }
                else
                {
                    currentSeason = Preferences.Get("LatestSeason", 0);
                }
            }

            if (RoundType == "18")
            {
                _database.Execute("UPDATE Scores SET UsedForCalc = 0 WHERE RoundType = '18'");
                var scorelist = _database.Table<Scores>().Where(s => s.RoundType == "18").ToList();
                if (Number < 10)
                {
                    //this turns the scorelist into the lowest X number of scores
                    if (Preferences.Get("SeparateBySeason", false) == true)
                    {
                        scorelist = scorelist.Where(o => o.Date.Year == currentSeason).OrderBy(o => o.Differential).Take(Number).ToList();
                    }
                    else
                    {
                        scorelist = scorelist.OrderBy(o => o.Differential).Take(Number).ToList();
                    }
                }
                else
                {
                    //needs to take the last 20 scores instead of using all the scores that are available
                    if (Preferences.Get("SeparateBySeason", false) == true)
                    {
                        scorelist = scorelist.Where(o => o.Date.Year == currentSeason).OrderByDescending(o => o.ScoreID).Take(20).ToList();
                    }
                    else
                    {
                        scorelist = scorelist.OrderByDescending(o => o.ScoreID).Take(20).ToList();
                    }
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
                    if (Preferences.Get("SeparateBySeason", false) == true)
                    {
                        scorelist = scorelist.Where(o => o.Date.Year == currentSeason).OrderBy(o => o.Differential).Take(Number).ToList();
                    }
                    else
                    {
                        //this turns the scorelist into the lowest X number of scores
                        scorelist = scorelist.OrderBy(o => o.Differential).Take(Number).ToList();
                    }
                }
                else
                {
                    if (Preferences.Get("SeparateBySeason", false) == true)
                    {
                        scorelist = scorelist.Where(o => o.Date.Year == currentSeason).OrderByDescending(o => o.ScoreID).Take(20).ToList();
                    }
                    else
                    {
                        //needs to take the last 20 scores instead of using all the scores that are available
                        scorelist = scorelist.OrderByDescending(o => o.ScoreID).Take(20).ToList();
                    }
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
            if (score.Date.Year > Preferences.Get("LatestSeason", 0))
            {
                Preferences.Set("LatestSeason", score.Date.Year);
            }
            return _database.Insert(score);
        }
        public int GetLatestSeason()
        {
            if (_database.Table<Scores>().Any())
            {
                return _database.Table<Scores>().Max(s => s.Date.Year);
            }
            else
            {
                return 0;
            }
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
        public double GetCurrentHandicap()
        {
            return _database.Table<Handicap>().OrderByDescending(o => o.Date).FirstOrDefault().Number;
        }
        public List<Scores> GetUsedScores(string RoundType)
        {
            int year;
            var output = new List<Scores>();
            if (RoundType == "Front" || RoundType == "Back")
            {
                output = _database.Table<Scores>().Where(s => s.RoundType == "Front" || s.RoundType == "Back").ToList();
            }
            else
            {
                output = _database.Table<Scores>().Where(s => s.RoundType == "18").ToList();
            }

            if (Preferences.Get("SeparateBySeason", false) == true)
            {
                if (Preferences.Get("SeasonsView", 0) > 0)
                {
                    year = Preferences.Get("SeasonsView", 0);
                }
                else
                {
                    year = Preferences.Get("LatestSeason", 0);
                }
                output = output.Where(s => s.Date.Year == year).ToList();
            }

            return output;
        }
        public List<Scores> GetLowestScores(int Number)
        {
            return _database.Table<Scores>().OrderBy(o => o.Score).Take(Number).ToList();
        }
        public List<double> GetLowestScoresDifferentials(int Number, List<Scores> ScoreList)
        {
            if (Number < 10)
            {
                //this turns the scorelist into the lowest X number of scores
                ScoreList = ScoreList.OrderBy(o => o.Differential).Take(Number).ToList();
            }
            else
            {
                //needs to take the last 20 scores instead of using all the scores that are available
                ScoreList = ScoreList.OrderByDescending(o => o.ScoreID).Take(20).ToList();
                ScoreList = ScoreList.OrderBy(o => o.Differential).Take(Number).ToList();
            }
            return ScoreList.Select(o => o.Differential).ToList();
        }
        public int DeleteCourse(int ID)
        {
            return _database.Delete<PlayedCourse>(ID);
        }
        public int DeleteScore(int ID)
        {
            return _database.Delete<Scores>(ID);
        }
        public bool IsLastScoreOfSeason(int Year)
        {
            var scores = _database.Table<Scores>().ToList();
            var count = scores.Where(s => s.Date.Year == Year).ToList().Count;
            if (count == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void CalculateHandicap(string RoundType)
        {
            if (!_database.Table<Scores>().Where(s => s.RoundType == RoundType).Any())
            {
                //will throw an error if there are no scores yet in the database for that round type and this is called and it doesnt need to be called with no scores anyways
                return;
            }
            var ScoreData = GetUsedScores(RoundType);
            if (ScoreData.Count < 5)
            {
                return;
            }

            var ScoresToUse = GetNumberOfScoresToUse(ScoreData.Count);
            var LowestScores = GetLowestScoresDifferentials(ScoresToUse, ScoreData);
            UpdateLowestScoreFlags(ScoresToUse, RoundType);
            var handicap = LowestScores.Average();
            //eventually make every decimal in the database to be a double since the handicap has to be a double and itll make things a lot easier
            handicap = Convert.ToDouble(handicap.ToString("0.#"));
            if (RoundType == "18")
            {
                if (Preferences.ContainsKey("Handicap18"))
                {
                    //only insert the handicap into the handicap history table if it is different than the current handicap
                    if (Convert.ToDouble(handicap) != Preferences.Get("Handicap18", -1.0))
                    {
                        var hdcp = new Handicap
                        {
                            Date = GetLastEnteredScoreDate(RoundType),
                            Number = handicap,
                            Type = RoundType
                        };
                        SaveHandicap(hdcp);
                    }
                    Preferences.Set("Handicap18", handicap);
                }
                else
                {
                    //this is the first time the user has gotten 5 scores to get a handicap calculated so insert the handicap as the first history
                    var hdcp = new Handicap
                    {
                        Date = GetLastEnteredScoreDate(RoundType),
                        Number = handicap,
                        Type = RoundType
                    };
                    Preferences.Set("Handicap18", handicap);
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
                            Date = GetLastEnteredScoreDate(RoundType),
                            Number = handicap,
                            Type = RoundType
                        };
                        Preferences.Set("Handicap9", handicap);
                        SaveHandicap(hdcp);
                    }
                }
                else
                {
                    //first time a user has gotten 5 scores to get a handicap
                    var hdcp = new Handicap
                    {
                        Date = GetLastEnteredScoreDate(RoundType),
                        Number = handicap,
                        Type = RoundType
                    };
                    Preferences.Set("Handicap9", handicap);
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
                    return 2;

                case 11:
                case 12:
                    return 3;

                case 13:
                case 14:
                    return 3;

                case 15:
                case 16:
                    return 4;

                case 17:
                    return 5;

                case 18:
                    return 6;

                case 19:
                    return 7;

                default:
                    if (ScoreCount >= 20)
                    {
                        return 8;
                    }
                    else
                    {
                        return 0;
                    }
            }
        }
    }
}
