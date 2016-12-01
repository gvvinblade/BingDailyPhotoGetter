using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BingPhotoGetterService.Service;
using BingPhotoGetterService.Configuration;
using Ninject;
using Ninject.Activation;
using RestSharp;
using static System.Configuration.ConfigurationManager;

namespace BingPhotoGetterService
{
    internal class IocModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<ILog>().ToMethod(ctx => LogManager.GetLogger(ctx.Request.ParentContext?.Request.Service));

            Bind<RestClient>().To<RestClient>().WithConstructorArgument("http://www.bing.com");

            Bind<ExecutionConfigurationSection>().ToProvider<ApplicationConfigurationSectionProvider>();

            Bind<Service.WinService>().ToSelf();
        }
    }

    internal class ApplicationConfigurationSectionProvider : IProvider
    {
        public object Create(IContext context) => OpenExeConfiguration(ConfigurationUserLevel.None).GetSection("execution");

        public Type Type => typeof(ExecutionConfigurationSection);
    }

}

