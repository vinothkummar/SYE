using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SYE.Tests.TestHelpers
{
    public static class FileHelper
    {
        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static bool DeleteFilesWithExtension(string folder, string extension)
        {
            try
            {
                string[] fileNames = Directory.GetFiles(folder);
                foreach (string filePath in fileNames)
                {
                    if (filePath.Contains(extension))
                    {
                        File.Delete(filePath);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                //log error
                return false;
            }
        }
    }
}