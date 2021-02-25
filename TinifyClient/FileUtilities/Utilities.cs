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
            List<string> allowExts = new List<string> { ".png", ".jpg" };
            var imgList = new List<string>();
            imgList.AddRange(Directory.GetFiles(fromPath,"*.*", SearchOption.AllDirectories).Where(d=> allowExts.Contains(Path.GetExtension(d.ToLower()))).ToList());
            //var dirs = Directory.GetDirectories(fromPath).ToList();
            //foreach (var dir in dirs)
            //{
            //    imgList.AddRange( GetAllImages(dir,true));
            //}
            return imgList;
        }
    }
}
