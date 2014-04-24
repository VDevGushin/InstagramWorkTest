using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramDownloaderTest.IOC.Interfaces
{
    public interface IContainer
    {
        T GetInstance<T>();
    }
}
