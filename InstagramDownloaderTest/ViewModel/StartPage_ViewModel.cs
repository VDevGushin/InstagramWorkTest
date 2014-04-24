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

        private ObservableCollection<Datum> _ImageData;

        public ObservableCollection<Datum> ImageData
        {
            get { return _ImageData; }
            set { SetProperty(ref _ImageData, value); }
        }

        private bool _progressBarInDeterminate;
        public bool ProgressBarInDeterminate
        {
            get { return _progressBarInDeterminate; }
            set
            {
                SetProperty(ref _progressBarInDeterminate, value);
            }
        }

        private string _ProgressbarText;

        public string ProgressbarText
        {
            get { return _ProgressbarText; }
            set { SetProperty(ref _ProgressbarText, value); }
        }

        #region Work with tray
        private void ProgressBarProp(string message, bool InDeterminate)
        {
            ProgressBarInDeterminate = InDeterminate;
            ProgressbarText = message;
        }
        #endregion

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
            InputName = "thebrainscoop";
            ImageData = new ObservableCollection<Datum>();
            ProgressBarProp(string.Empty, false);
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
                ProgressBarProp("Поиск информации", true);
            }
            else
            {
                ProgressBarProp(string.Empty, false);
                _dialogService.Show("вы не ввели ни одного имени!");
            }
        }

        //get user id by name
        private async void GetUserId(string InputName)
        {
            var JsonString = Convert.ToString(await _webDataSource.LoadRemote<string>(URLstrings.GetUserIdString(InputName.Trim(), StringDictClass.CLIENT_ID)));           
            var UserInfo = JsonConvert.DeserializeObject<UserInfoClass>(JsonString);
            if (UserInfo.meta.code == 200 && UserInfo.data != null && UserInfo.data.Count > 0)
            {
                GetUserOneUser(UserInfo);
            }
            else
            {
                ProgressBarProp(string.Empty, false);
            }
        }

        private void GetUserOneUser(UserInfoClass UserInfo)
        {
            //get only one user       
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
                GetUserResources(userdatum.id);
            }
            else
            {
                ProgressBarProp(string.Empty, false);
                _dialogService.Show("нет такого пользователя");
            }
        }


        private async void GetUserResources(string p)
        {
          
            var JsonString = Convert.ToString(await _webDataSource.LoadRemote<string>(URLstrings.GetMediaFromUser(p.Trim(), StringDictClass.CLIENT_ID)));
            var UserMediaInfo = JsonConvert.DeserializeObject<RootObject>(JsonString);
            ProgressBarProp(string.Empty, false);
            if (UserMediaInfo.meta.code == 200 && UserMediaInfo.data != null && UserMediaInfo.data.Count > 0)
            {            
                ImageData = UserMediaInfo.data;
            }

        }
        
    }
}
