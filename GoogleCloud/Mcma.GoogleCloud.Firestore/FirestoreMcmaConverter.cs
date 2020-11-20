﻿using System.Collections.Generic;
using Google.Cloud.Firestore;
using Mcma.Serialization;
using Newtonsoft.Json.Linq;

namespace Mcma.GoogleCloud.Firestore
{
    public class FirestoreMcmaConverter : IFirestoreConverter<McmaObject>
    {
        public object ToFirestore(McmaObject value) => value.ToMcmaJson().ToObject<Dictionary<string, object>>();

        public McmaObject FromFirestore(object value) => JObject.FromObject(value).ToMcmaObject<McmaObject>();
    }
}