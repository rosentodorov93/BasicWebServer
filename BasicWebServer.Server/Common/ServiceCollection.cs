using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Common
{
    public class ServiceCollection : IServiceCollection
    {
        private readonly Dictionary<Type, Type> services;

        public ServiceCollection()
        {
            this.services = new Dictionary<Type, Type>();
        }

        public IServiceCollection Add<TService, TImplemetation>()
            where TService : class
            where TImplemetation : TService
        {
            services.Add(typeof(TService), typeof(TImplemetation));

            return this;
        }

        public IServiceCollection Add<TService>() where TService : class
        {
            return Add<TService, TService>();
        }

        public object CreateInstance(Type serviceType)
        {
            if (services.ContainsKey(serviceType))
            {
                serviceType = services[serviceType];
            }
            else if (serviceType.IsInterface)
            {
                throw new InvalidOperationException($"Service {serviceType.Name} is not registered!");
            }

            var constructors = serviceType.GetConstructors();

            if (constructors.Length > 1)
            {
                throw new InvalidOperationException("Multiple constructors are not suported!");
            }

            var constuctor = constructors.First();
            var parameters = constuctor.GetParameters();
            var parametersValues = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var parameterValue = CreateInstance(parameterType);

                parametersValues[i] = parameterValue;
            }

            return constuctor.Invoke(parametersValues);
        }

        public TService GetService<TService>() where TService : class
        {
            var serviceType = typeof(TService);

            if (!services.ContainsKey(serviceType))
            {
                return null;
            }

            var service = services[serviceType];

            return (TService)CreateInstance(service);
        }
    }
}
