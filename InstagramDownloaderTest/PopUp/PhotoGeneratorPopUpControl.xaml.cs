using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Windows.Storage.Streams;
using System.IO;
using System.Windows.Resources;
using System.Collections.ObjectModel;
using InstagramDownloaderTest.Model;
using System.IO.IsolatedStorage;

namespace InstagramDownloaderTest.PopUp
{
    public partial class PhotoGeneratorPopUpControl : UserControl
    {

        BitmapImage finalImage = new BitmapImage();
        WriteableBitmap wbFinal;
        public int Counter { get; set; }


        PhoneApplicationPage _currentPage;
        PhoneApplicationFrame _currentFrame;
        bool _mustRestoreApplicationBar = false;
        bool _mustRestoreSystemTray = false;

        public PhotoGeneratorPopUpControl()
        {
            InitializeComponent();
        }

        public void Show(object getObject)
        {                         
            LayoutRoot.Width = Application.Current.Host.Content.ActualWidth;
            LayoutRoot.Height = Application.Current.Host.Content.ActualHeight;
            _currentFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            _currentPage = _currentFrame.Content as PhoneApplicationPage;
            if (SystemTray.IsVisible)
            {
                _mustRestoreSystemTray = true;
                SystemTray.IsVisible = false;
            }
            if (_currentPage.ApplicationBar != null)
            {
                if (_currentPage.ApplicationBar.IsVisible)
                    _mustRestoreApplicationBar = true;

                _currentPage.ApplicationBar.IsVisible = false;
            }
            if (_currentPage != null)
            {
                _currentPage.BackKeyPress += OnBackKeyPress;
            }
            RootPopup.IsOpen = true;
            SlideTransition turnstileTransition = new SlideTransition() { Mode = SlideTransitionMode.SlideUpFadeIn };
            ITransition transition = turnstileTransition.GetTransition(LayoutRoot);
            transition.Completed += delegate
            {
                transition.Stop();
            };
            transition.Begin();
            //get Image source
            GetImageFromWeb(getObject as List<Datum>);                
        }


        private void GetImageFromWeb(List<Datum> list)
        {
            SetLoaderState(false);

            List<BitmapImage> _wbList = new List<BitmapImage>();
            //count of download images
            Counter = list.Count;

            foreach (var Item in list)
            {
                WebClient webClientImg = new WebClient();
                webClientImg.OpenReadCompleted += (s, e) =>
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(e.Result);
                    //add new image to list(for make collage)
                    _wbList.Add(bitmap);
                    //check for all images
                    if (_wbList.Count == Counter)
                    {
                        //if all photos
                        MakeCollage(_wbList);
                    }                   
                };              
                webClientImg.OpenReadAsync(new Uri(Item.images.low_resolution.url, UriKind.Absolute));
            }
        }

        private void SetLoaderState(bool isReady)
        {
            if (!isReady)
            {
                ButtonSend.Visibility = System.Windows.Visibility.Collapsed;
                LoaderText.Text = "Собираем коллаж...";
                Prorgessbar.IsIndeterminate = true;
            }
            else
            {
                ButtonSend.Visibility = System.Windows.Visibility.Visible;
                LoaderText.Text = "Коллаж готов...";
                Prorgessbar.IsIndeterminate = false;
            }
        }

       

        private void MakeCollage(List<BitmapImage> _wbList)
        {
            List<BitmapImage> images = new List<BitmapImage>();
            int width = 0; // Final width.
            int height = 0; // Final height.

            foreach (var item in _wbList)
            {
                WriteableBitmap wb = new WriteableBitmap(item);
                width = wb.PixelWidth > width ? wb.PixelWidth : width;
                height = wb.PixelHeight > height ? wb.PixelHeight : height;
                images.Add(item);
            }

            StreamResourceInfo sri;
            if (_wbList.Count >= 1 && _wbList.Count < 3)
            {
                sri =  System.Windows.Application.GetResourceStream(new Uri("F2.jpg",
                  UriKind.Relative));
            }
            else if (_wbList.Count >= 3 && _wbList.Count < 5)
            {
                sri = System.Windows.Application.GetResourceStream(new Uri("F4.jpg",
                UriKind.Relative));
            }
            else
            {
                sri = System.Windows.Application.GetResourceStream(new Uri("F6.jpg",
                UriKind.Relative));
            }

            finalImage.SetSource(sri.Stream);

            wbFinal = new WriteableBitmap(finalImage);
            using (MemoryStream mem = new MemoryStream())
            {
                int tempWidth = 0;   // Parameter for Translate.X
                int tempHeight = 0;  // Parameter for Translate.Y
                int counter = 0;
                foreach (BitmapImage item in images)
                {
                    Image image = new Image();
                    image.Height = item.PixelHeight;
                    image.Width = item.PixelWidth;
                    image.Source = item;

                    // TranslateTransform                      
                    TranslateTransform tf = new TranslateTransform();
                    tf.X = tempWidth;
                    tf.Y = tempHeight;
                    wbFinal.Render(image, tf);

                    if (isEven(counter))
                    {
                        tempWidth += item.PixelWidth;
                    }
                    else
                    {
                        tempHeight += item.PixelHeight;
                    }

                    counter++;

                    if (counter % 2 == 0)
                    {
                        tempWidth = 0;
                    }
                }
                wbFinal.Invalidate();
                wbFinal.SaveJpeg(mem, width, height, 0, 100);
                mem.Seek(0, System.IO.SeekOrigin.Begin);
                // Show image               
                ImageCollage.Source = wbFinal;
                SetLoaderState(true);
            }        
        }


        private bool isEven(int _n)
        {            
            return (_n % 2 == 0 ? true : false);
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            //Dismiss(wbFinal);
            if (wbFinal != null)
            {
                WriteableBitmap wb = (WriteableBitmap)wbFinal;
                var fileStream = new System.IO.MemoryStream();
                wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 100, 100);
                fileStream.Seek(0, System.IO.SeekOrigin.Begin);
                Microsoft.Xna.Framework.Media.MediaLibrary ml = new Microsoft.Xna.Framework.Media.MediaLibrary();
                Microsoft.Xna.Framework.Media.Picture pic = ml.SavePicture("TestCollage.png", fileStream);
                var path = Microsoft.Xna.Framework.Media.PhoneExtensions.MediaLibraryExtensions.GetPath(pic);
                Microsoft.Phone.Tasks.ShareMediaTask shareMediaTask = new Microsoft.Phone.Tasks.ShareMediaTask();
                shareMediaTask.FilePath = path;
                shareMediaTask.Show();
            }
        }

        private void Dismiss(object returnObj)
        {
            _currentFrame.Focus();
            if (_currentPage != null)
            {
                _currentPage.BackKeyPress -= OnBackKeyPress;
            }
            if (_mustRestoreApplicationBar)
                _currentPage.ApplicationBar.IsVisible = true;
            if (_mustRestoreSystemTray)
                SystemTray.IsVisible = true;
            if (OnDismiss != null)
                OnDismiss(this, returnObj);

            SlideTransition turnstileTransition = new SlideTransition() { Mode = SlideTransitionMode.SlideDownFadeOut };
            ITransition transition = turnstileTransition.GetTransition(LayoutRoot);
            transition.Completed += delegate
            {
                transition.Stop();
                RootPopup.IsOpen = false;
            };
            transition.Begin();
        }

        void OnBackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Dismiss(null);
        }

        public event OnDismissEventHandler OnDismiss;
        public delegate void OnDismissEventHandler(object sender, object returnObject);
      
    }
}
