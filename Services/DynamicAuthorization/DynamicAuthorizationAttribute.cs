using Microsoft.AspNetCore.Authorization;

namespace Services.DynamicAuthorization;

public class DynamicAuthorizationAttribute : AuthorizeAttribute
{

    public DynamicAuthorizationAttribute(string claimToAuthorize) : base(UserConstant.DynamicAuthorizationPolicy)
    {
        ClaimToAuthorize = claimToAuthorize;
    }

    public string ClaimToAuthorize { get; }
}
