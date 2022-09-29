using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Common
{
    public interface IServiceCollection
    {
        IServiceCollection Add<TService, TImplemetation>()
            where TService : class
            where TImplemetation : TService;
        IServiceCollection Add<TService>()
            where TService : class;
        TService GetService<TService>()
            where TService: class;
        Object CreateInstance(Type serviceType); 
    }
}
