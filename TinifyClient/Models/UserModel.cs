using System;
using System.Collections.Generic;
using System.Text;

namespace TinifyClient.Models
{
   public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime LastAccessDate { get; set; }
        public int TotalFilesCompressed { get; set; }
        public int Priority { get; set; }

    }
}
