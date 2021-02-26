using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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
            List<string> allowExts = new List<string> { ".png", ".jpg", ".jpeg" };
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

        public static string ResizeImage(string sourceFile,  int maxResAllow)
        {
            var tempfilename =Path.GetTempPath()+Path.GetFileName(sourceFile);
            ResizeImage(sourceFile, tempfilename,maxResAllow);
            return tempfilename;
        }
        public static bool ResizeImage(string sourceFile, string detinitionFile, int maxResAllow)
        {
            // Image.Load(string path) is a shortcut for our default type. 
            // Other pixel formats use Image.Load<TPixel>(string path))
            using (Image<Rgba32> image = (Image<Rgba32>)Image.Load(sourceFile))
            {
                int newWidth = image.Width;
                int newHeight = image.Height;
                var newSize = GetRes(new Size(image.Width, image.Height), maxResAllow);


                //newWidth = (int)Math.Round(((float)image.Width) / devider, 0);
                //newHeight = (int)Math.Round(((float)image.Height) / devider, 0);
                //8,294,400


                image.Mutate(x => x.Resize(newSize));
               // image.Mutate(x => x.Resize(newSize.Width,newSize.Height));

                image.Save(detinitionFile); // Automatic encoder selected based on extension.


                // File.Delete(sourceFile);

                return true;
            }

        }

        private static Size GetRes(Size size, int maxResAllow)
        {
            if (maxResAllow == 0)
                return size;
            //{
            if (size.Height > size.Width)
            {
                if (size.Height > maxResAllow)
                {
                    var rt = (decimal)size.Height / maxResAllow;
                    size.Height = maxResAllow;
                    size.Width = (int)Math.Round(((decimal)size.Height / rt), 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    return size;
                }

            }
            else if (size.Width > maxResAllow)
            {
               
                var rt = (decimal)size.Width / maxResAllow;
                size.Width = maxResAllow;
                size.Height =(int)Math.Round( ((decimal)size.Height / rt),0,MidpointRounding.AwayFromZero);
            }
            // }
            return size;
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
