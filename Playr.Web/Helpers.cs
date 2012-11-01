using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Playr.Web
{
    public static class Helpers
    {
        public static string GetApiUrl()
        {
            return WebConfigurationManager.AppSettings["playr:ApiUrl"];
        }

        public static string GetNotificationUrl()
        {
            return WebConfigurationManager.AppSettings["playr:NotificationUrl"];
        }

        public static string BuildApiUrl(string path)
        {
            var builder = new UriBuilder(GetApiUrl());
            builder.Path = path;
            return builder.Uri.ToString();
        }

        public static string BuildNotificationUrl()
        {
            var builder = new UriBuilder(GetNotificationUrl());
            builder.Path = "/signalr";
            return builder.Uri.ToString();
        }
    }
}