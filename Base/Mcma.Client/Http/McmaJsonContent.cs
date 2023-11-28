using System;
using System.Text;
using Mcma.Serialization;

namespace Mcma.Client.Http;

public class McmaJsonContent : StringContent
{
    public McmaJsonContent(object obj)
        : base(obj.ToMcmaJson().ToString(), Encoding.UTF8, "application/json")
    {

    }
}