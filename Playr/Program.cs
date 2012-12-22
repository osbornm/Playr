using System;
using Microsoft.Owin.Hosting;

namespace Playr
{
    public class Program
    {
        static void Main(string[] args)
        {
            var baseUrl = "http://localhost:5555/";

            using (WebApplication.Start<Startup>(baseUrl, "Microsoft.Owin.Host.HttpListener"))
            {
                Console.WriteLine("Playr is running at {0}", baseUrl);
                Console.WriteLine("See http://github.com/osbornm/playr for more information on setup.");
                Console.WriteLine();
                Console.WriteLine("Press any key to stop server...");
                Console.ReadKey(intercept: true);
            }
        }
    }
}
