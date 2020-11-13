namespace Mcma.Data.DocumentDatabase.Queries
{
    public class CustomQuery<TParameters>
    {
        public string Name { get; set; }
        
        public TParameters Parameters { get; set; }
        
        public string PageStartToken { get; set; }
    }
}