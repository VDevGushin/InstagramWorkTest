using InstagramDownloaderTest.Model;
using InstagramDownloaderTest.PopUp;
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
using System.Windows.Media.Imaging;

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

        #endregion

        #region Services

        private readonly IDialogService _dialogService;
        private readonly IGetHttpResponseData _webDataSource;

        private readonly IUser _getUserIdService;
        #endregion

        #region Progress bar 

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

        
        private void ProgressBarProp(string message, bool InDeterminate)
        {
            ProgressBarInDeterminate = InDeterminate;
            ProgressbarText = message;
        }
        
        #endregion

        public StartPage_ViewModel(IDialogService dialogService, IUser getUserIdService)
        {
            //get services for viewmodel
            _dialogService = dialogService;
            _getUserIdService = getUserIdService;
            StartUIState();
        }

        private void StartUIState()
        {
            //"wade0n" eroshka_ia france_faust
            InputName = "france_faust";
            ImageData = new ObservableCollection<Datum>();
            ProgressBarProp(string.Empty, false);
        }


        #region Begin to get collection of photos
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
                ProgressBarProp("Поиск информации", true);
                GetUserId(InputName);               
            }
            else
            {
                ProgressBarProp(string.Empty, false);
                _dialogService.Show("Проверьте поля ввода...");
            }
        }
        #endregion

        //get user id by name
        private async void GetUserId(string InputName)
        {

            UserDatum UserInfo = await _getUserIdService.GetUserObjectClass<UserDatum>(InputName);

            if (UserInfo != null && UserInfo.id != null && UserInfo.id != string.Empty)
            {

                GetUserResources(UserInfo.id);
            }
            else
            {
                _dialogService.Show("Не удается найти пользователя...");
                ProgressBarProp(string.Empty, false);
            }
           
        }


        private async void GetUserResources(string UserID)
        {
            
            ObservableCollection<Datum> GetCollection = await _getUserIdService.GetUserBestColletion<ObservableCollection<Datum>>(UserID);
            if (GetCollection != null && GetCollection.Count > 0)
            {
                ImageData.Clear();
                //get sorted by likes images (for image picker)
                ImageData = GetCollection;
                ProgressBarProp(string.Empty, false);
            }
            else
            {
                _dialogService.Show("Не удается найти фотографии...");
                ProgressBarProp(string.Empty, false);
            }      
        }



        public System.Windows.Input.ICommand _MakeCollectionCommand;
        public System.Windows.Input.ICommand MakeCollectionCommand
        {
            get
            {
                return _MakeCollectionCommand = _MakeCollectionCommand ?? new Command.Command(MakeCollectionCommand_Delegate);
            }
        }

        public void MakeCollectionCommand_Delegate(object p)
        {
           // _dialogService.Show(string.Format("Вы выбрали {0} фоток для работы",((List<Datum>)p).Count));
            PhotoGeneratorPopUpControl popUp = new PhotoGeneratorPopUpControl();
            popUp.Show(p);
            popUp.OnDismiss += popUp_OnDismiss;
        }

        void popUp_OnDismiss(object sender, object returnObject)
        {
            var _sender = sender as PhotoGeneratorPopUpControl;
            _sender.OnDismiss -= popUp_OnDismiss;

            if (returnObject != null)
            {
                WriteableBitmap wb = (WriteableBitmap)returnObject;
                var fileStream = new System.IO.MemoryStream();
                wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 100, 100);
                fileStream.Seek(0, System.IO.SeekOrigin.Begin);




                Microsoft.Xna.Framework.Media.MediaLibrary ml = new Microsoft.Xna.Framework.Media.MediaLibrary();
                Microsoft.Xna.Framework.Media.Picture pic = ml.SavePicture("mycollage.png", fileStream);
                var path = Microsoft.Xna.Framework.Media.PhoneExtensions.MediaLibraryExtensions.GetPath(pic);

                Microsoft.Phone.Tasks.ShareMediaTask shareMediaTask = new Microsoft.Phone.Tasks.ShareMediaTask();
                shareMediaTask.FilePath = path;
                shareMediaTask.Show();
            }
            //send to email
            //if (returnObject != null)
            //{
            //    WriteableBitmap wb = (WriteableBitmap)returnObject;
            //     using (var memoryStream = new System.IO.MemoryStream())
            //     {
            //         wb.SaveJpeg(memoryStream, wb.PixelWidth, wb.PixelHeight, 0, 60);
            //         memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            //         var bytes = memoryStream.ToArray();
            //         var base64String = Convert.ToBase64String(bytes);

            //         var emailComposeTask = new Microsoft.Phone.Tasks.EmailComposeTask
            //         {
            //             Subject = "Коллаж из программы",
            //             Body = base64String                      
            //         };
            //         emailComposeTask.Show();
            //     }
            //}

        }        
    }
}
