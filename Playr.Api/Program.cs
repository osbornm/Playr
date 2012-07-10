using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.SelfHost;
using System.Web.Http;
using System.Net.Http;

namespace Playr.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:5555");
            config.Routes.MapHttpRoute("default", "{controller}/{action}");
            var server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();


            Console.WriteLine("Press any key to stop server...");
            Console.ReadLine();
            server.CloseAsync().Wait();
        }
    }
}
