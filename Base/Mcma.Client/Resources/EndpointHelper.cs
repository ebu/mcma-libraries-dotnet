using Mcma.Utility;

namespace Mcma.Client.Resources;

internal static class EndpointHelper
{
    public static string GetDefaultPathForType<T>()
        => typeof(T).Name.PascalCaseToKebabCase().PluralizeKebabCase();

    public static string GetChildRoute<TChild>(string parentResourceId, string pathToChildren = null)
        => $"{parentResourceId.TrimEnd('/')}/{(pathToChildren ?? GetDefaultPathForType<TChild>()).TrimStart('/')}";
}
