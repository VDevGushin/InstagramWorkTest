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
using Microsoft.Phone.Tasks;
using System.Threading.Tasks;



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
            try
            {
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
                    webClientImg.OpenReadAsync(new Uri(Item.images.standard_resolution.url, UriKind.Absolute));
                }
            }
            catch
            {
                LoaderText.Text = "Ошибка...";
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

            SizeInt finalSize = GETSize(_wbList.Count, images);         
            wbFinal = new WriteableBitmap(finalSize.Width,finalSize.Height);
            
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
                //int[] pixelColors = wbFinal.Pixels.ToArray();
                //ApplyFilter(pixelColors);
                //wbFinal.SetPixel( = pixelColors;

                ImageCollage.Source = wbFinal;
                SetLoaderState(true);
                mem.Close();
            }        
        }

       

        private SizeInt GETSize(int p, List<BitmapImage> images)
        {
            SizeInt retuneSize = new SizeInt();
            SizeInt ImagesSize = new SizeInt();
            foreach (var image in images)
            {
                ImagesSize.Width =image.PixelWidth;
               ImagesSize.Height = image.PixelHeight;
                break;
            }

            if (p == 1)
            {
                retuneSize.Height = ImagesSize.Height;
                retuneSize.Width = ImagesSize.Width;
            }
            else if (p > 1 && p < 3)
            {
                retuneSize.Height = ImagesSize.Height;
                retuneSize.Width = ImagesSize.Width *2;
            }

            else if (p >= 3 && p < 5)
            {
                retuneSize.Height = ImagesSize.Height * 2;
                retuneSize.Width = ImagesSize.Width * 2;
            }
            else
            {
                retuneSize.Height = ImagesSize.Height * 3;
                retuneSize.Width = ImagesSize.Width * 2;
            }
            return retuneSize;
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

        #region Close popup
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

        #endregion
    }
}
/*
 *    ////get count of images
            //StreamResourceInfo sri;
            //if (_wbList.Count >= 1 && _wbList.Count < 3)
            //{
            //    sri =  System.Windows.Application.GetResourceStream(new Uri("F2.jpg",
            //      UriKind.Relative));
            //}
            //else if (_wbList.Count >= 3 && _wbList.Count < 5)
            //{
            //    sri = System.Windows.Application.GetResourceStream(new Uri("F4.jpg",
            //    UriKind.Relative));
            //}
            //else
            //{
            //    sri = System.Windows.Application.GetResourceStream(new Uri("F6.jpg",
            //    UriKind.Relative));
            //}
            ////
            //set source of final image
           // finalImage.SetSource(sri.Stream);
Set Background
*/