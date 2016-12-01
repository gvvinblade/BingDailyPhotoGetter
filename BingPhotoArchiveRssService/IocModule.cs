using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BingPhotoArchiveRssService.Service;
using Ninject;
using RestSharp;

namespace BingPhotoArchiveRssService
{
    class IocModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<ILog>().ToMethod(ctx => LogManager.GetLogger(ctx.Request.ParentContext?.Request.Service));

            Bind<RestClient>().To<RestClient>().WithConstructorArgument("http://www.bing.com");

            Bind<RssRequestHandler>().ToSelf();

            Bind<HttpServer>().ToSelf().OnActivation((cx, server) =>
            {
                server.ProcessRequest += cx.Kernel.Get<RssRequestHandler>().Handle;
            });

            Bind<Service.WinService>().ToSelf();
        }
    }
}

