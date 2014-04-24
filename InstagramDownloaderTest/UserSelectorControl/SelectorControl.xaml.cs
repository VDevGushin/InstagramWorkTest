using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using InstagramDownloaderTest.Model;
using System.ComponentModel;

namespace InstagramDownloaderTest.UserSelectorControl
{
    public partial class SelectorControl : UserControl
    {



        public SelectorControl()
        {
            InitializeComponent();
        }

        public ObservableCollection<Datum> CollectionImages
        {
            get { return (ObservableCollection<Datum>)GetValue(CollectionImagesProperty); }
            set { SetValue(CollectionImagesProperty, value); }
        }

        public static readonly DependencyProperty CollectionImagesProperty =
            DependencyProperty.Register("CollectionImages", typeof(ObservableCollection<Datum>), typeof(SelectorControl), new PropertyMetadata(OnCollectionImagesChangedCallback));


        private static void OnCollectionImagesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SelectorControl)d).OnCollectionImagesChanged(e);
        }

        private void OnCollectionImagesChanged(DependencyPropertyChangedEventArgs e)
        {
            var Source = (ObservableCollection<Datum>)e.NewValue;
            GridSelector.ItemsSource = Source;
        }


        public List<Datum> ImageSelectedData
        {
            get { return (List<Datum>)GetValue(ImageSelectedDataProperty); }
            set { SetValue(ImageSelectedDataProperty, value); }
        }

        public static readonly DependencyProperty ImageSelectedDataProperty =
            DependencyProperty.Register("ImageSelectedData", typeof(List<Datum>), typeof(SelectorControl), new PropertyMetadata(ImageSelectedDataChangedCallback));


        private static void ImageSelectedDataChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SelectorControl)d).ImageSelectedDataChanged(e);
        }

        private void ImageSelectedDataChanged(DependencyPropertyChangedEventArgs e)
        {
            if (((List<Datum>)e.NewValue).Count > 0)
            {
                GetButton.Visibility = System.Windows.Visibility.Visible;
            }

            else
            {
                GetButton.Visibility = System.Windows.Visibility.Collapsed;
            }
        }


        private void GridSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var _tmpSelected = new List<Datum>();
            
                foreach (var item in GridSelector.SelectedItems)
                {
                    _tmpSelected.Add(item as Datum);
                }     
           
            ImageSelectedData = _tmpSelected;
        }


        public delegate void CollectionSelectedEventHandler(object sender, CustomEventArgs e);

        public event CollectionSelectedEventHandler CollectionSelectedChanged;

        private void GetButton_Click(object sender, RoutedEventArgs e)
        {
            if (CollectionSelectedChanged != null)
            {
                CollectionSelectedChanged(this, new CustomEventArgs() {DatumList = this.ImageSelectedData});
            }
        }
    }
}
