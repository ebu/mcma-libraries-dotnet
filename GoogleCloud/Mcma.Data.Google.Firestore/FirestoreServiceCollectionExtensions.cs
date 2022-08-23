using System;
using Google.Cloud.Firestore;
using Mcma.Data.DocumentDatabase;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.Data.Google.Firestore;

public static class FirestoreServiceCollectionExtensions
{
    public static IServiceCollection AddMcmaFirestore(this IServiceCollection services,
                                                      Action<FirestoreTableOptions> configureOptions = null,
                                                      Action<FirestoreTableBuilder> build = null)
    {
        if (configureOptions != null)
            services.Configure(configureOptions);

        var builder = new FirestoreTableBuilder(services);
        build?.Invoke(builder);
        builder.AddDefaults();

        services.TryAddSingleton<ICustomQueryBuilderRegistry<(Query, string)>, FirestoreCustomQueryBuilderRegistry>();
        services.TryAddSingleton<IFirestoreQueryBuilder, FirestoreQueryBuilder>();

        return services.AddSingleton<IDocumentDatabaseTable, FirestoreTable>();
    }

    public static IServiceCollection AddMcmaFirestore(this IServiceCollection services,
                                                      string tableName,
                                                      Action<FirestoreTableBuilder> build = null)
        => services.AddMcmaFirestore(opts => opts.TableName = tableName, build);
}