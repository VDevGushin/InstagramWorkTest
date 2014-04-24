using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramDownloaderTest.StringDict
{
    public static class URLstrings    
    {       
       public static string GetUserIdString(string InputName, string p)
       {
           return string.Format("https://api.instagram.com/v1/users/search?q={0}&client_id={1}", InputName, p);
       }

       public static string GetMediaFromUser(string UserID, string p)
       {
           return string.Format("https://api.instagram.com/v1/users/{0}/media/recent/?client_id={1}", UserID, p);
       }
    }
}
