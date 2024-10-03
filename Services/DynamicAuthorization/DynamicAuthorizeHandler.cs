using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Services.DynamicAuthorization.Utilities;
using System.Threading.Tasks;

namespace Services.DynamicAuthorization;

public class DynamicAuthorizeHandler : AuthorizationHandler<DynamicAuthorizeRequirement>
{
    private readonly IApiUtility _apiUtility;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DynamicAuthorizeHandler(IApiUtility apiUtility, IHttpContextAccessor httpContextAccessor)
    {
        _apiUtility = apiUtility;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicAuthorizeRequirement requirement)
    {
        var claimValue = _apiUtility.GetClaim(_httpContextAccessor.HttpContext);

        if (!string.IsNullOrWhiteSpace(claimValue))
        {
            if (context.User.HasClaim(UserConstant.ClaimType, claimValue) || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }
            else
                context.Fail();
        }
        return Task.CompletedTask;

    }
}
