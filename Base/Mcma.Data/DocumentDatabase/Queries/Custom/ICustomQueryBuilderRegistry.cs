﻿namespace Mcma.Data.DocumentDatabase.Queries.Custom;

public interface ICustomQueryBuilderRegistry<out TProviderQuery>
{
    ICustomQueryBuilder<TParameters, TProviderQuery> Get<TParameters>(string name);
}