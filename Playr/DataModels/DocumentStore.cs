using System;
using System.Reflection;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace Playr.DataModels
{
    public static class Database
    {
        private static IDocumentStore docStore;

        public static void Initialize()
        {
            docStore = new EmbeddableDocumentStore();
            docStore.Initialize();

            IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), docStore);
        }

        public static IDocumentSession OpenSession()
        {
            if (docStore == null)
                throw new InvalidOperationException("IDocumentStore has not been initialized.");

            return docStore.OpenSession();
        }
    }
}