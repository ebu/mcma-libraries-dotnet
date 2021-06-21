namespace Mcma.Data.DocumentDatabase.Queries.Custom
{
    public class CustomQuery<TParameters>
    {
        public string Name { get; set; }
        
        public TParameters Parameters { get; set; }
        
        public string PageStartToken { get; set; }
    }
}