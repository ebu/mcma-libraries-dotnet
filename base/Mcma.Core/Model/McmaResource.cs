using System;

namespace Mcma
{
    public abstract class McmaResource : McmaObject
    {
        public string Id { get; set; }

        public DateTimeOffset? DateCreated { get; set; }

        public DateTimeOffset? DateModified { get; set; }

        public void OnCreate(string id)
        {
            Id = id;
            DateModified = DateCreated = DateTimeOffset.UtcNow;
        }

        public void OnUpsert(string id)
        {
            Id = id;
            DateModified = DateTimeOffset.UtcNow;
            if (!DateCreated.HasValue)
                DateCreated = DateModified;
        }
    }
}