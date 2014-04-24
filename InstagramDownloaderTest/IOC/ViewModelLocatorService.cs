using InstagramDownloaderTest.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramDownloaderTest.IOC
{
    public class ViewModelLocatorService
    {
        private readonly Interfaces.IContainer _container;
        public ViewModelLocatorService()
        {
            _container = new Container();
        }

        public T ResolveService<T>()
        {
            return _container.GetInstance<T>();
        }

        public StartPage_ViewModel StartPageViewModel
        {
            get
            {
                return _container.GetInstance<StartPage_ViewModel>();
            }
        }
    }
}
