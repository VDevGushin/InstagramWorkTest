using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramDownloaderTest.Model
{  
    public class Meta1
    {
        public int code { get; set; }
    }

    public class UserDatum
    {
        public string username { get; set; }
        public string bio { get; set; }
        public string website { get; set; }
        public string profile_picture { get; set; }
        public string full_name { get; set; }
        public string id { get; set; }
    }

    public class UserInfoClass
    {
        public Meta1 meta { get; set; }
        public List<UserDatum> data { get; set; }
    }
}
