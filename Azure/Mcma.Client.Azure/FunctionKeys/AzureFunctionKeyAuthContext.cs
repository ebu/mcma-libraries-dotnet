namespace Mcma.Client.Azure.FunctionKeys;

public class AzureFunctionKeyAuthContext
{
    public AzureFunctionKeyAuthContext()
    {
    }

    public AzureFunctionKeyAuthContext(string functionKey, bool isEncrypted = true)
    {
        FunctionKey = functionKey;
        IsEncrypted = isEncrypted;
    }

    public string FunctionKey { get; set; }

    public bool IsEncrypted { get; set; }
}