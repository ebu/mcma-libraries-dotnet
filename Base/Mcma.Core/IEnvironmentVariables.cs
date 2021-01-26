using System.Collections.Generic;

namespace Mcma
{
    public interface IEnvironmentVariables
    {
        IReadOnlyCollection<string> Keys { get; }

        string Get(string name);

        string GetOptional(string name, string defaultValue = null);
    }
}