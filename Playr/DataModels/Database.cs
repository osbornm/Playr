using System;
using System.Configuration;
using System.Reflection;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace Playr.DataModels
{
    public static class Database
    {
        private static Lazy<IDocumentStore> docStore = new Lazy<IDocumentStore>(Initialize, isThreadSafe: false);

        private static IDocumentStore Initialize()
        {
            var result = new EmbeddableDocumentStore();
#if DEBUG
            result.UseEmbeddedHttpServer = true;
#else
            var embeddedHttpServer = false;
            var embeddedConfigValue = ConfigurationManager.AppSettings["RavenDB:Embedded"];

            if (embeddedConfigValue != null)
                Boolean.TryParse(embeddedConfigValue, out embeddedHttpServer);

            result.UseEmbeddedHttpServer = embeddedHttpServer;
#endif
            result.Initialize();

            IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), result);

            return result;
        }

        public static IDocumentSession OpenSession()
        {
            return docStore.Value.OpenSession();
        }
    }
}