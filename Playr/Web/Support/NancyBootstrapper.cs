using System;
using System.Reflection;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Responses;
using Nancy.TinyIoc;
using Nancy.ViewEngines;
using Nancy.ViewEngines.Razor;

namespace Playr.Web.Support
{
    public class NancyBootstrapper : DefaultNancyBootstrapper
    {
        private static readonly Assembly thisAssembly = typeof(NancyBootstrapper).Assembly;

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get { return NancyInternalConfiguration.WithOverrides(config => config.ViewLocationProvider = typeof(ResourceViewLocationProvider)); }
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            ResourceViewLocationProvider.Ignore.Add(typeof(RazorViewEngine).Assembly);
            ResourceViewLocationProvider.RootNamespaces.Add(thisAssembly, "Playr.Web.Views");
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add((ctx, rootPath) =>
            {
                var fullName = "Playr.Web.Content" + ctx.Request.Url.Path.Replace('/', '.');
                if (thisAssembly.GetManifestResourceInfo(fullName) == null)
                    return null;

                var parts = ctx.Request.Url.Path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var fileName = parts[parts.Length - 1];
                var path = fullName.Substring(0, fullName.Length - fileName.Length - 1);

                return new EmbeddedFileResponse(thisAssembly, path, fileName);
            });
        }
    }
}