﻿using System;
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
        WriteableBitmap vb;
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
            this.DataContext = this;                       
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

           
            GetImageFromWeb(getObject as List<Datum>);

          //  SaveImageToIsolatedStorage(getObject as List<Datum>);
           
           // getImage(getObject as List<Datum>);                     
        }


        private void GetImageFromWeb(List<Datum> list)
        {
            List<BitmapImage> _wbList = new List<BitmapImage>();
            Counter = list.Count;
            foreach (var Item in list)
            {
                WebClient webClientImg = new WebClient();
                webClientImg.OpenReadCompleted += (s, e) =>
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(e.Result);
                    //WriteableBitmap wb = new WriteableBitmap(bitmap);
                    _wbList.Add(bitmap);
                    if (_wbList.Count == Counter)
                    {
                        //ImageCollage.Source = wb;
                        MakeCollage(_wbList);
                    }                   
                };              
                webClientImg.OpenReadAsync(new Uri(Item.images.low_resolution.url, UriKind.Absolute));
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

            StreamResourceInfo sri = System.Windows.Application.GetResourceStream(new Uri("White.jpg",
                UriKind.Relative));
            finalImage.SetSource(sri.Stream);

            wbFinal = new WriteableBitmap(finalImage);
            using (MemoryStream mem = new MemoryStream())
            {
                int tempWidth = 0;   // Parameter for Translate.X
                int tempHeight = 0;  // Parameter for Translate.Y

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

                    tempHeight += item.PixelHeight;
                    tempWidth += item.PixelWidth;
                }

                wbFinal.Invalidate();
                wbFinal.SaveJpeg(mem, width, height, 0, 100);
                mem.Seek(0, System.IO.SeekOrigin.Begin);

                // Show image.               
                ImageCollage.Source = wbFinal;            
            }        
        }




        private void getImage(List<Datum> observableCollection)
        {
            string[] files = new string[] { "Roses-Most-Beautiful-485x728.jpg", "Lovely-Sea-House-485x728.jpg" };
            List<BitmapImage> images = new List<BitmapImage>(); // BitmapImage list.
            int width = 0; // Final width.
            int height = 0; // Final height.

            foreach (string image in files)
            {
                // Create a Bitmap from the file and add it to the list                
                BitmapImage img = new BitmapImage();
                StreamResourceInfo r = System.Windows.Application.GetResourceStream(new Uri(image, UriKind.RelativeOrAbsolute));
                img.SetSource(r.Stream);

                WriteableBitmap wb = new WriteableBitmap(img);

                // Update the size of the final bitmap
                width = wb.PixelWidth > width ? wb.PixelWidth : width;
                height = wb.PixelHeight > height ? wb.PixelHeight : height;

                images.Add(img);
            }

            StreamResourceInfo sri = System.Windows.Application.GetResourceStream(new Uri("White.jpg",
                UriKind.Relative));
            finalImage.SetSource(sri.Stream);

            wbFinal = new WriteableBitmap(finalImage);
            using (MemoryStream mem = new MemoryStream())
            {
                int tempWidth = 0;   // Parameter for Translate.X
                int tempHeight = 0;  // Parameter for Translate.Y

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

                    tempHeight += item.PixelHeight;
                }

                wbFinal.Invalidate();
                wbFinal.SaveJpeg(mem, width, height, 0, 100);
                mem.Seek(0, System.IO.SeekOrigin.Begin);

                // Show image.               
                ImageCollage.Source = wbFinal;               
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
/*
 * 














        public string strImageName { get; set; }
        public int counter { get; set; }

        private void SaveImageToIsolatedStorage(List<Datum> list)
        {
            counter = list.Count;
            foreach (var Item in list)
            {
                if (Item.images.low_resolution.url.Contains("http://"))
                {
                    strImageName = Item.images.low_resolution.url.Substring(Item.images.low_resolution.url.LastIndexOf("/") + 1);
                    SaveImageToIsolatedStorage(Item.images.low_resolution.url, strImageName);
                } 
            }
        }


        private void SaveImageToIsolatedStorage(string uri, string strImageName)
        {
            // Use WebClient to download web server's images. 
            WebClient webClientImg = new WebClient();
            webClientImg.OpenReadCompleted += (s, e) =>
                {
                    SaveToJpeg(e.Result, strImageName);
                };
            //webClientImg.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            webClientImg.OpenReadAsync(new Uri(uri, UriKind.Absolute));
        }

      
        private void SaveToJpeg(Stream stream, string fileName)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (iso.FileExists(fileName))
                {
                    iso.DeleteFile(fileName);
                }
                using (IsolatedStorageFileStream isostream = iso.CreateFile(fileName))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                    WriteableBitmap wb = new WriteableBitmap(bitmap);
                    // Encode WriteableBitmap object to a JPEG stream. 
                    Extensions.SaveJpeg(wb, isostream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                    isostream.Close();
                }                
            }
        } 
*/