using System;

namespace Mcma.Model;

/// <summary>
/// An MCMA resource, defining an ID and properties for tracking date created and modified
/// </summary>
public abstract class McmaResource : McmaObject
{
    /// <summary>
    /// Gets or sets the ID of the resource
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or set the date and time at which the resource was created
    /// </summary>
    public DateTimeOffset? DateCreated { get; set; }

    /// <summary>
    /// Gets or set the date and time at which the resource was last modified
    /// </summary>
    public DateTimeOffset? DateModified { get; set; }

    /// <summary>
    /// Sets the ID and created/modified dates of the resource when it is first created
    /// </summary>
    /// <param name="id">The ID of the newly-created resource</param>
    public void OnCreate(string id)
    {
        Id = id;
        DateModified = DateCreated = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Sets the ID and created/modified dates of the resource on an upsert
    /// </summary>
    /// <param name="id"></param>
    public void OnUpsert(string id)
    {
        Id = id;
        DateModified = DateTimeOffset.UtcNow;
        DateCreated ??= DateModified;
    }
}