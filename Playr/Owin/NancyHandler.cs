using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nancy.Bootstrapper;
using Nancy.Owin;

namespace Playr.Owin
{
    public class NancyHandler
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly NancyOwinHost _owinHost;

        public NancyHandler(Func<IDictionary<string, object>, Task> next, INancyBootstrapper bootstrapper)
        {
            _next = next;
            _owinHost = new NancyOwinHost(next, bootstrapper);
        }

        public Task Invoke(IDictionary<string, object> env)
        {
            return _owinHost.Invoke(env);
        }
    }
}
