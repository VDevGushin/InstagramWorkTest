using InstagramDownloaderTest.Model;
using InstagramDownloaderTest.Service.Interfaces;
using InstagramDownloaderTest.StringDict;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InstagramDownloaderTest.ViewModel
{
    public class StartPage_ViewModel : Base.VMBase
    {
        #region prop

        private string _InputName;

        public string InputName
        {
            get { return _InputName; }
            set { SetProperty(ref _InputName, value); }
        }

        #endregion

        #region Services

        private readonly IDialogService _dialogService;
        private readonly IGetHttpResponseData _webDataSource;
        #endregion

        public StartPage_ViewModel(IDialogService dialogService , IGetHttpResponseData webDataSource)
        {
            //get services for viewmodel
            _dialogService = dialogService;
            _webDataSource = webDataSource;
            StartUIState();
        }

        private void StartUIState()
        {
            InputName = "vasumitra";
        }
        
        public System.Windows.Input.ICommand _lets_Сollage_Command;
        public System.Windows.Input.ICommand Lets_Сollage_Command
        {
            get
            {
                return _lets_Сollage_Command = _lets_Сollage_Command ?? new Command.Command(Lets_Сollage_Delegate);
            }
        }

        public void Lets_Сollage_Delegate(object p)
        {
            if (InputName != string.Empty)
            {
                //1 get user id
                GetUserId(InputName);
            }
            else
            {
                _dialogService.Show("вы не ввели ни одного имени!");
            }
        }

        //get user id by name
        private async void GetUserId(string InputName)
        {
            var JsonString = Convert.ToString(await _webDataSource.LoadRemote<string>(URLstrings.GetUserIdString(InputName, StringDictClass.CLIENT_ID)));           
            var UserInfo = JsonConvert.DeserializeObject<UserInfoClass>(JsonString);
            if (UserInfo.meta.code == 200 && UserInfo.data != null && UserInfo.data.Count > 0)
            {
                GetUserPhotos(UserInfo);
            }
        }

        private void GetUserPhotos(UserInfoClass UserInfo)
        {
            if (UserInfo.data.Count > 1)
            {

            }
            else
            {

            }
        }
        
    }
}
