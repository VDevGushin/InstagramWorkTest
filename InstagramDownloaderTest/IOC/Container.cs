using InstagramDownloaderTest.Service;
using InstagramDownloaderTest.Service.Interfaces;
using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramDownloaderTest.IOC
{
    public class Container : Interfaces.IContainer
    {
        private readonly ServiceContainer _currentContainer;
        public Container()
        {
            _currentContainer = new LightInject.ServiceContainer();
            _currentContainer.Register<InstagramDownloaderTest.ViewModel.StartPage_ViewModel>();

            _currentContainer.Register<IDialogService,DialogService>(new LightInject.PerContainerLifetime());
            _currentContainer.Register<IGetHttpResponseData, GetWebDataSource>();
            _currentContainer.Register<IUser, UserINFO>();
        }

        public T GetInstance<T>()
        {
            return _currentContainer.GetInstance<T>();
        }
    }
}
/*CLIENT ID	05a491f2598e4719aa6b601196240e38
CLIENT SECRET	4a764ddc39484b0ebdfd59df0af9e3d9*/