using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nancy.Bootstrapper;
using Nancy.Hosting.Owin;
using Owin;

public static class NancyOwinExtensions
{
    public static void UseNancy(this IAppBuilder app)
    {
        app.Use(typeof(NancyHandler));
    }

    public static void UseNancy(this IAppBuilder app, INancyBootstrapper bootstrapper)
    {
        app.Use(typeof(NancyHandler), bootstrapper);
    }

    private class NancyHandler
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly NancyOwinHost _owinHost;

        public NancyHandler(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
            _owinHost = new NancyOwinHost();
        }

        public NancyHandler(Func<IDictionary<string, object>, Task> next, INancyBootstrapper bootstrapper)
        {
            _next = next;
            _owinHost = new NancyOwinHost(bootstrapper);
        }

        public Task Invoke(IDictionary<string, object> env)
        {
            return _owinHost.ProcessRequest(env);
        }
    }
}