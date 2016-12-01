using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingPhotoArchiveRssService.Domain
{
    class Image
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Url { get; set; }
        public string Copyright { get; set; }
        public string Name => Url?.Substring(Url.LastIndexOf("/", StringComparison.Ordinal) + 1);
    }

    class Response
    {
        public List<Image> Images { get; set; }
    }
}