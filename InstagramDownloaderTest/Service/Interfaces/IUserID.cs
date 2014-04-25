using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramDownloaderTest.Service.Interfaces
{
    public interface IUser
    {
        Task<T> GetUserObjectClass<T>(string UserName);
        Task<T> GetUserBestColletion<T>(string UserID);
    }
}
