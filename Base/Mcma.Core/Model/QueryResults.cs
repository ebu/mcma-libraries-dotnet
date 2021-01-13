using System.Collections.Generic;

namespace Mcma
{
    /// <summary>
    /// Represents the results of running a query for MCMA resources against an MCMA service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryResults<T>
    {
        /// <summary>
        /// Gets or sets the collection of results returned by the query
        /// </summary>
        public IEnumerable<T> Results { get; set; }
        
        /// <summary>
        /// Gets or sets a string that can be passed back to the MCMA service that executed this query in order to get the next page of results.
        /// If no more results are available for the query, this will be null.
        /// </summary>
        public string NextPageStartToken { get; set; }
    }
}