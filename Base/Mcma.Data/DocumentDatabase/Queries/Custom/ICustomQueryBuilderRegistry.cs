namespace Mcma.Data.DocumentDatabase.Queries
{
    public interface ICustomQueryBuilderRegistry<out TProviderQuery>
    {
        ICustomQueryBuilder<TParameters, TProviderQuery> Get<TParameters>(string name);
    }
}