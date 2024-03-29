﻿using System;

namespace Mcma.Data.DocumentDatabase.Queries.Custom;

public interface ICustomQueryBuilder<TParameters, out TProviderQuery> : ICustomQueryBuilder
{
    TProviderQuery Build(CustomQuery<TParameters> customQuery);
}

public interface ICustomQueryBuilder
{
    string Name { get; }

    Type ParameterType { get; }
}