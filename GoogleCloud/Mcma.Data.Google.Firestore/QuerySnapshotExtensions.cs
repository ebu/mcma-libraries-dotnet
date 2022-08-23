using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace Mcma.Data.Google.Firestore;

public static class QuerySnapshotExtensions
{
    public static async Task<string> GetNextPageStartTokenAsync(this QuerySnapshot querySnapshot, string sortBy)
    {
        if (!querySnapshot.Documents.Any())
            return null;

        var lastItem = querySnapshot.Documents.Last();

        var nextResults = await querySnapshot.Query.Limit(1).StartAfter(lastItem).GetSnapshotAsync();
        if (nextResults.Count == 0)
            return null;

        return sortBy != null ? lastItem.ToDictionary()[sortBy]?.ToString() : lastItem.Id;
    }
}