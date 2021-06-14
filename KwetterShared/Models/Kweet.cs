using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KwetterShared.Models
{
    public class Kweet
    {
        public int KweetID { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public int Likes { get; set; }
        public long TimeCreated { get; set; }
        public string UserID { get; set; }
    }
}
