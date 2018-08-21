using System;
using System.IO;

namespace PositionTracker.Utility
{
    public class FileHelper
    {
        public static string FilesDir;

        static FileHelper()
        {
            const string filesDir = "PositionTrackerFiles";
            var filesRootDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            FilesDir = Path.Combine(filesRootDir, filesDir);

            Directory.CreateDirectory(FilesDir);
        }

        public static bool WriteToFile(string text, string filePath)
        {
            try { File.AppendAllText(filePath, $"{text} {Environment.NewLine}"); }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot write to file! Ex: " + ex.Message);

                return false;
            }

            return true;
        }
    }
}