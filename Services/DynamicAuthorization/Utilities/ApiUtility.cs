using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using Services.DynamicAuthorization;

namespace Services.DynamicAuthorization.Utilities;

public class ApiUtility : IApiUtility
{
    public ApiUtility(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
    {
        var apiInfo = new List<ApiModel>();
        var actionDescriptors = actionDescriptorCollectionProvider.ActionDescriptors.Items;
        foreach (var actionDescriptor in actionDescriptors)
        {
            if (!(actionDescriptor is ControllerActionDescriptor descriptor)) continue;

            var controllerTypeInfo = descriptor.ControllerTypeInfo;

            var claimValue = descriptor.MethodInfo.GetCustomAttribute<DynamicAuthorizationAttribute>()?.ClaimToAuthorize;

            if (!string.IsNullOrWhiteSpace(claimValue))
            {
                apiInfo.Add(new ApiModel(controllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue, descriptor.ControllerName, descriptor.ActionName, claimValue));
            }
        }
        ApiInfo = ImmutableHashSet.CreateRange(apiInfo);
    }

    public ImmutableHashSet<ApiModel> ApiInfo { get; }

    public string GetClaim(HttpContext httpContext)
    {
        var areaName = httpContext.GetRouteValue("area")?.ToString();
        var actionName = httpContext.GetRouteValue("action")?.ToString();
        var controllerName = httpContext.GetRouteValue("controller")?.ToString();

        ApiInfo.TryGetValue(new ApiModel(areaName, controllerName, actionName, string.Empty), out var model);

        return model?.ClaimToAuthorize;
    }
}