using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace Mcma.Api.Routing;

public abstract class McmaApiRoute : IMcmaApiRoute
{
    protected McmaApiRoute(HttpMethod httpMethod, string path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));
            
        HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));

        TemplateMatcher = new TemplateMatcher(TemplateParser.Parse(path), null);
    }

    public HttpMethod HttpMethod { get; }

    private TemplateMatcher TemplateMatcher { get; }

    public bool IsMatch(string path, out IDictionary<string, object> pathVariables)
    {
        var routeValueDictionary = new RouteValueDictionary();
        var isMatch = TemplateMatcher.TryMatch(path, routeValueDictionary);

        pathVariables = routeValueDictionary;
        return isMatch;
    }

    public abstract Task HandleAsync(McmaApiRequestContext requestContext);
}