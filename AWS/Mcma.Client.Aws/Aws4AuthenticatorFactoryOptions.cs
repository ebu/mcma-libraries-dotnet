namespace Mcma.Client.Aws
{
    public class Aws4AuthenticatorFactoryOptions
    {
        public Aws4AuthContext DefaultAuthContext { get; set; } = Aws4AuthContext.Global;
    }
}