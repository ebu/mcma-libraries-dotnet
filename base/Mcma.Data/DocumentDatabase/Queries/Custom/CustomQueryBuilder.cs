using System;

namespace Mcma.Data.DocumentDatabase.Queries
{
    public abstract class CustomQueryBuilder<TParameters, TProviderQuery> : ICustomQueryBuilder<TParameters, TProviderQuery>
    {
        public abstract string Name { get; }

        Type ICustomQueryBuilder.ParameterType => typeof(TParameters);

        public abstract TProviderQuery Build(CustomQuery<TParameters> customQuery);
    }
}