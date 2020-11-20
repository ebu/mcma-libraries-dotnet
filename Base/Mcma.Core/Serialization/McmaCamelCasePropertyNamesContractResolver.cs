using System;
using Newtonsoft.Json.Serialization;

namespace Mcma.Serialization
{
    /// <summary>
    /// Inherits from <see cref="CamelCasePropertyNamesContractResolver"/> and implements custom dictionary key resolution
    /// </summary>
    public class McmaCamelCasePropertyNamesContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            var contract = base.CreateDictionaryContract(objectType);

            contract.DictionaryKeyResolver = key => key;

            return contract;
        }
    }
}