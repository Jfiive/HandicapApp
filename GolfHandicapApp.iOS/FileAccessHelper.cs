using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Foundation;
using UIKit;

namespace GolfHandicapApp.iOS
{
    class FileAccessHelper
    {
        public static void CopyDatabaseIfNotExists()
        {
            string libFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            string dbPath = Path.Combine(libFolder, "HandicapDB.db");
            if (!File.Exists(dbPath))
            {
                var existingDb = NSBundle.MainBundle.PathForResource("HandicapDB", "db");
                File.Copy(existingDb, dbPath);
            }
        }
    }
}