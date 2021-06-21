using System.Linq;
using Google.Cloud.Firestore;
using Mcma.Data.DocumentDatabase.Queries;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using Mcma.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.Data.Google.Firestore
{
    public class FirestoreTableBuilder
    {
        public FirestoreTableBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
        
        public FirestoreDbBuilder Firestore { get; } = new();

        public FirestoreTableBuilder AddCustomQueryBuilder<TParameters, TCustomQueryBuilder>()
            where TCustomQueryBuilder : class, ICustomQueryBuilder<TParameters, Query>
        {
            Services.AddCustomQueryBuilder<TParameters, Query, TCustomQueryBuilder>();
            return this;
        }

        internal void AddDefaults()
        {
            Services.TryAddSingleton<IFirestoreConverter<McmaObject>, FirestoreMcmaConverter>();
            Services.TryAddSingleton(provider =>
            {
                if (!Firestore.ConverterRegistry.OfType<IFirestoreConverter<McmaObject>>().Any())
                    Firestore.ConverterRegistry.Add(provider.GetRequiredService<IFirestoreConverter<McmaObject>>());
                
                return Firestore.Build();
            });
        }
    }
}