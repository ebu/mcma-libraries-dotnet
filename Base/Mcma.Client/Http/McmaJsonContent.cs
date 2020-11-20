﻿using System.Net.Http;
using System.Text;
using Mcma.Serialization;

namespace Mcma.Client
{
    public class McmaJsonContent : StringContent
    {
        public McmaJsonContent(object obj)
            : base(obj.ToMcmaJson().ToString(), Encoding.UTF8, "application/json")
        {
        }
    }
}