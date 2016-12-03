using System;
using System.IO;
using System.Linq;
using System.Timers;
using BingPhotoGetterService.Configuration;
using BingPhotoGetterService.Domain;
using Common.Logging;
using Ninject.Infrastructure.Language;
using RestSharp;
using RestSharp.Extensions;
using Topshelf;

namespace BingPhotoGetterService.Service
{
    internal class WinService
    {
        private readonly ILog _log;
        private readonly RestClient _client;
        private readonly Timer _timer;

        private readonly long _executionInterval;
        private readonly int _photosCount;
        private readonly DirectoryInfo _targetDirectory;

        private DateTime _lastExecutionTime;


        public WinService(ExecutionConfigurationSection conf, RestClient client, ILog iLog)
        {
            if (iLog == null)
                throw new ArgumentNullException(nameof(iLog));

            _executionInterval = conf.ExecutionInterval;
            _photosCount = conf.PhotosCount;
            _targetDirectory = Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(conf.Path));

            _log = iLog;
            _client = client;
            _timer = new Timer() {Interval = 1000};
            _timer.Elapsed += (sender, args) => GetPhotos();

        }

        public bool Start(HostControl hostControl)
        {
            _log.Info($"{nameof(WinService)} Start command received.");
            Start();
            GetPhotos();
            return true;
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
            _timer.Start();
            return true;
        }

        private bool Stop()
        {
            _timer.Stop();
            return true;
        }

        private void GetPhotos()
        {
            if ((DateTime.Now - _lastExecutionTime).TotalSeconds >= _executionInterval)
                try
                {
                    var request = new RestRequest("/HPImageArchive.aspx", Method.GET);
                    request.AddParameter("format", "js");
                    request.AddParameter("n", _photosCount);
                    var restResponse = _client.Execute<Response>(request).Data;

        

                    var files = _targetDirectory.EnumerateFiles().Select(file => file.Name);
                    restResponse.Images.Where(image => !files.Contains(image.Name)).Map(Download);

                    var images = restResponse.Images.Select(image => image.Name);
                    _targetDirectory.EnumerateFiles().Where(file => !images.Contains(file.Name)).Map(file => file.Delete());

                }
                catch (Exception e)
                {
                    _log.Error(e);
                }
            _lastExecutionTime = DateTime.Now;
        }

        private void Download(Image image)
        {
            _client.DownloadData(new RestRequest(image.Url)).SaveAs($"{_targetDirectory.FullName}/{image.Name}");
        }
    }
}