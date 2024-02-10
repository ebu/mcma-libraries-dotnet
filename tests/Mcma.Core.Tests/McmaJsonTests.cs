using Mcma.Model;
using Mcma.Serialization;

namespace Mcma.Core.Tests
{
    [TestClass]
    public class McmaJsonTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var json = @"{ ""@type"": ""Container1"", ""jobParameters"": { ""@type"": ""JobParameterBag"", ""test"": { ""@type"": ""Test"", ""value"": ""testVal"" } } }";
            var jToken = McmaJson.Parse(json);

            var container1 = jToken?.ToMcmaObject<Ambiguous1.Container1>();
            Assert.IsNotNull(container1);

            var container2 = jToken?.ToMcmaObject<Ambiguous2.Container2>();
            Assert.IsNotNull(container2);
        }
    }
}

namespace Mcma.Core.Tests.Ambiguous1
{
    public class Container1 : McmaResource { public JobParameterBag? JobParameters { get; set; } }

    public class Test : McmaResource { public string? Value { get; set; } }
}

namespace Mcma.Core.Tests.Ambiguous2
{
    public class Container2 : McmaResource { public JobParameterBag? JobParameters { get; set; } }

    public class Test : McmaResource { public string? Value { get; set; } }
}