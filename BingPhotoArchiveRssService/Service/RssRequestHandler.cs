using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using BingPhotoArchiveRssService.Domain;
using Common.Logging;
using RestSharp;

namespace BingPhotoArchiveRssService.Service
{
    class RssRequestHandler
    {
        private readonly RestClient _client;
        private readonly ILog _log;

        public RssRequestHandler(RestClient client, ILog log)
        {
            _client = client;
            _log = log;
        }

        public void Handle(HttpListenerContext context)
        {
            using (var response = context.Response)
            {
                var requestUrl = context.Request.Url;
                if (requestUrl.Segments.Last().EndsWith("images.rss"))
                {
                    var request = new RestRequest("/HPImageArchive.aspx?format=js&n=5", Method.GET);
                    var restResponse = _client.Execute<Response>(request).Data;

                    response.ContentEncoding = Encoding.UTF8;
                    response.ContentType = "text/xml";

                    Write(requestUrl.ToString(), restResponse, response.OutputStream);

                }
                else
                {
                    response.StatusCode = 404;
                }
            }
        }

        private static void Write(string url, Response response, Stream output)
        {
            var writer = new XmlTextWriter(output, Encoding.UTF8);

            writer.WriteStartDocument();

            writer.WriteStartElement("rss");
            writer.WriteAttributeString("version", "2.0");

            writer.WriteStartElement("channel");
            writer.WriteElementString("title", "Bing Daily Photo Archive");
            writer.WriteElementString("link", url);
            writer.WriteElementString("description", "Bing Daily Photo Archive");
            writer.WriteElementString("ttl", "240");

            foreach (var image in response.Images)
            {
                writer.WriteStartElement("item");
                writer.WriteElementString("title", image.Copyright);
                writer.WriteElementString("description", image.Copyright);
                writer.WriteElementString("link", $"http://www.bing.com{image.Url}");
                writer.WriteStartElement("enclosure");
                writer.WriteAttributeString("url", $"http://www.bing.com{image.Url}");
                writer.WriteAttributeString("type", "image/jpeg");
                writer.WriteEndElement();
                writer.WriteElementString("pubDate", image.StartDate.ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }
    }
}