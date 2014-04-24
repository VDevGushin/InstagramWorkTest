using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramDownloaderTest.Service.Interfaces
{
    public interface IGetHttpResponseData
    {
        Task<T> LoadRemote<T>(string url);
    }
}
