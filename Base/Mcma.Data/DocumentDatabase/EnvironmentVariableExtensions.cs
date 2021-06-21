using Mcma.Utility;

namespace Mcma.Data.DocumentDatabase
{
    public class DocumentDatabaseTableOptions
    {
        public string TableName { get; set; } = McmaEnvironmentVariables.Get("TABLE_NAME", false);
    }
}