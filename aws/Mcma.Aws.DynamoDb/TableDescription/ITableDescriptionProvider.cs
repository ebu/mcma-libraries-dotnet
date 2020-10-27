using System.Threading.Tasks;

namespace Mcma.Aws.DynamoDb
{
    public interface ITableDescriptionProvider
    {
        Task<DynamoDbTableDescription> GetTableDescriptionAsync(string tableName);
    }
}