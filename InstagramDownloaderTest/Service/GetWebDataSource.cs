using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InstagramDownloaderTest.Service
{
    public class GetWebDataSource : Interfaces.IGetHttpResponseData
    {
        public async Task<T> LoadRemote<T>(string url)
        {
            string XML = await LoadSyndicationFeed(url);
            return (T)Convert.ChangeType(XML, typeof(T));
        }

        private async Task<string> LoadSyndicationFeed(string url)
        {
            string returnContent = null; ;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
               // request.Credentials = new NetworkCredential(userName, Pas);
                var response =
                   await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);
                using (StreamReader responseStream = new StreamReader(response.GetResponseStream()))
                {

                    returnContent = responseStream.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                returnContent = null;
            }
            return returnContent;
        }
    }
}
