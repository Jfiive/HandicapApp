using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace GolfHandicapApp
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Course>().Wait();
            _database.CreateTableAsync<Scores>().Wait();
            _database.CreateTableAsync<Handicap>().Wait();
        }
        //general database actions
        public Task<List<Course>> GetCourses()
        {
            return _database.Table<Course>().ToListAsync();
        }
        public Task<int> SaveCourse(Course course)
        {
            return _database.InsertAsync(course);
        }
        public Task<List<Handicap>> GetHandicaps()
        {
            return _database.Table<Handicap>().ToListAsync();
        }
        public Task<int> SaveHandicap(Handicap handicap)
        {
            return _database.InsertAsync(handicap);
        }
        public Task<List<DetailedScore>> GetPastScores()
        {
            //this isnt just returning the scores like the other ones are this has to join the course and the score table to make the detailed scores the user is going to see on the front page
            //will figure this one out later when all the other database actions are set up and running
            //might have to ditch the whole async thing if this is going to work in here
            return null;
        }
        public Task<int> PostScore(Scores score)
        {
            return _database.InsertAsync(score);
        }
    }
}
