using InstagramDownloaderTest.Model;
using InstagramDownloaderTest.Service.Interfaces;
using InstagramDownloaderTest.StringDict;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramDownloaderTest.Service
{
    public class UserINFO : Interfaces.IUser
    {
        #region services

        private readonly IGetHttpResponseData _webDataSource;

        #endregion

        public UserINFO(IGetHttpResponseData webDataSource)
        {
            _webDataSource = webDataSource;
        }

        public async Task<T> GetUserObjectClass<T>(string UserName)
        {
            var UDAT = await GetUserInformation(UserName.ToLower().Trim());
            return (T)Convert.ChangeType(UDAT, typeof(T));
        }

        private async Task<UserDatum> GetUserInformation(string InputName)
        {
            UserDatum returnClass = null; ;
            try
            {
                var JsonString = Convert.ToString(await _webDataSource.LoadRemote<string>(URLstrings.GetUserIdString(InputName, StringDictClass.CLIENT_ID)));
                if (JsonString != null && JsonString != string.Empty)
                {
                     var UserInfo = JsonConvert.DeserializeObject<UserInfoClass>(JsonString);
                     if (UserInfo != null && UserInfo.meta.code == 200 && UserInfo.data != null && UserInfo.data.Count > 0)
                     {
                         UserDatum userdatum = null;
                         foreach (var user in UserInfo.data)
                         {
                             if (user.username.Equals(InputName))
                             {
                                 userdatum = user;
                                 break;
                             }
                         }
                         if (userdatum != null && userdatum.id != null && userdatum.id != string.Empty)
                         {
                             returnClass = userdatum;
                         }
                         else
                         {
                             returnClass = null;
                         }
                     }
                     else
                     {
                         returnClass = null;
                     }
                }
                else
                {
                    returnClass = null;
                }
            }
            catch (Exception ex)
            {
                returnClass = null;
            }
            return returnClass;
        }

        public Task<T> GetUserObjectClass<T>()
        {
            throw new NotImplementedException();
        }


        public async Task<T> GetUserBestColletion<T>(string UserID)
        {
            var BestCollection = await GetUserBestColletion(UserID);
            return (T)Convert.ChangeType(BestCollection, typeof(T));
        }

        

        private async Task<ObservableCollection<Datum>> GetUserBestColletion(string UserID)
        {
            ObservableCollection<Datum> RetunColletion = null;
            try
            {
                var JsonString = Convert.ToString(await _webDataSource.LoadRemote<string>(URLstrings.GetMediaFromUser(UserID.Trim(), StringDictClass.CLIENT_ID)));
                if (JsonString != null)
                {
                    var UserMediaInfo = JsonConvert.DeserializeObject<RootObject>(JsonString);
                    if (UserMediaInfo != null && UserMediaInfo.meta.code == 200 && UserMediaInfo.data != null && UserMediaInfo.data.Count > 0)
                    {
                        RetunColletion = new ObservableCollection<Datum>();

                       
                         foreach (var dat in UserMediaInfo.data)
                         {
                             if (dat.type == "image")
                             {
                                 RetunColletion.Add(dat);
                             }
                         }
                       
                        //sort by like (to get best images)
                         RetunColletion = new ObservableCollection<Datum>(RetunColletion.OrderByDescending(a => a.likes.count));
                    }
                    else
                    {
                        RetunColletion = null;
                    }
                }
                else
                {
                    RetunColletion = null;
                }            
            }
            catch
            {
                RetunColletion = null;
            }
            return RetunColletion ;
            
        }
    }
}
