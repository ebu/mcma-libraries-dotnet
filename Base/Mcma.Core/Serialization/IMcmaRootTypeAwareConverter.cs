using Newtonsoft.Json;

namespace Mcma.Serialization;

public interface IMcmaRootTypeAwareConverter
{
    JsonConverter ForRootType(Type rootType);
}
