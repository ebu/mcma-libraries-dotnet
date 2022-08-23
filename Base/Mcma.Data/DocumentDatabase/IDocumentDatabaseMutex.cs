using System.Threading.Tasks;

namespace Mcma.Data.DocumentDatabase;

public interface IDocumentDatabaseMutex
{
    Task LockAsync();

    Task UnlockAsync();
}