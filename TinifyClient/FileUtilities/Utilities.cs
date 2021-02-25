using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TinifyClient.FileUtilities
{
    public class Utilities
    {
        public List<string> GetAllImages(string fromPath, bool isRecursive)
        {
            List<string> allowExts = new List<string> { ".png", ".jpg",".jpeg" };
            var imgList = new List<string>();
            imgList.AddRange(Directory.GetFiles(fromPath, "*.*", SearchOption.AllDirectories).Where(d => allowExts.Contains(Path.GetExtension(d.ToLower()))).ToList());
            //var dirs = Directory.GetDirectories(fromPath).ToList();
            //foreach (var dir in dirs)
            //{
            //    imgList.AddRange( GetAllImages(dir,true));
            //}
            return imgList;
        }

        public static void CreateFolderIfNotExists(string destpathforcheck)
        {
            if (Directory.Exists(destpathforcheck))
            {
                return;
            }
            else
            {

                Directory.CreateDirectory(destpathforcheck);
            }
        }

    }

    public static class FileSizeFormatter
    {
        // Load all suffixes in an array  
        static readonly string[] suffixes =
        { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        public static string FormatSize(Int64 bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }
    }

}
