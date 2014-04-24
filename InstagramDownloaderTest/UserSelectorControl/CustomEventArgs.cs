using InstagramDownloaderTest.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramDownloaderTest.UserSelectorControl
{
    public class CustomEventArgs : EventArgs
    {
        public List<Datum> DatumList { get; set; }       
    }
}
