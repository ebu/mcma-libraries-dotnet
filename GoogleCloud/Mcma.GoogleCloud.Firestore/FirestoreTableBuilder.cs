using System.Linq;
using Google.Cloud.Firestore;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.GoogleCloud.Firestore
{
    public class FirestoreTableBuilder
    {
        public FirestoreTableBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
        
        public FirestoreDbBuilder Firestore { get; } = new FirestoreDbBuilder();

        public FirestoreTableBuilder AddCustomQueryBuilder<TParameters, TCustomQueryBuilder>()
            where TCustomQueryBuilder : class, ICustomQueryBuilder<TParameters, Query>
        {
            Services.AddSingleton<ICustomQueryBuilder<TParameters, Query>, TCustomQueryBuilder>();
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