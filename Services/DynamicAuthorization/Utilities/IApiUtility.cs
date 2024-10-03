using Microsoft.AspNetCore.Http;
using System.Collections.Immutable;

namespace Services.DynamicAuthorization.Utilities;

public interface IApiUtility
{
    public ImmutableHashSet<ApiModel> ApiInfo { get; }

    public string GetClaim(HttpContext httpContext);
}