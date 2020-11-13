using System.Collections.Generic;

namespace Mcma
{
    public class QueryResults<T>
    {
        public IEnumerable<T> Results { get; set; }
        
        public string NextPageStartToken { get; set; }
    }
}