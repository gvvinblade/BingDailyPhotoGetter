using System;
using Common.Logging;
using Topshelf;

namespace BingPhotoArchiveRssService.Service
{
    class WinService
    {
        private readonly ILog _log;
        private readonly HttpServer _server;

        public WinService(HttpServer server, ILog logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _log = logger;
            _server = server;
        }

        public bool Start(HostControl hostControl)
        {
            _log.Info($"{nameof(WinService)} Start command received.");

            return Start();
        }

        public bool Stop(HostControl hostControl)
        {
            _log.Trace($"{nameof(WinService)} Stop command received.");

            return Stop();
        }

        public bool Pause(HostControl hostControl)
        {
            _log.Trace($"{nameof(WinService)} Pause command received.");

            return Stop();
        }

        public bool Continue(HostControl hostControl)
        {
            _log.Trace($"{nameof(WinService)} Continue command received.");

            return Start();
        }

        public bool Shutdown(HostControl hostControl)
        {
            _log.Trace($"{nameof(WinService)} Shutdown command received.");

            return Stop();
        }

        private bool Start()
        {
            _server.Start(9925);
            return true;
        }

        private bool Stop()
        {
            _server.Stop();
            return true;
        }
    }
}