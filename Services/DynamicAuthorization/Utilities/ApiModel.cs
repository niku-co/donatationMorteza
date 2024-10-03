using System;

namespace Services.DynamicAuthorization.Utilities;

public class ApiModel : IEquatable<ApiModel>
{
    public string AreaName { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public string ClaimToAuthorize { get; set; }
    public ApiModel(string areaName, string controllerName, string actionName, string claimToAuthorize)
    {
        AreaName = areaName;
        ControllerName = controllerName;
        ActionName = actionName;
        ClaimToAuthorize = claimToAuthorize;
    }

    public bool Equals(ApiModel other)
    {
        if (ReferenceEquals(null, other)) return false;

        if (ReferenceEquals(this, other)) return true;

        if (GetType() != other.GetType()) return false;

        return other.ActionName == ActionName
             && other.ControllerName == ControllerName
             && other.AreaName == AreaName;
    }
    public override bool Equals(object obj) => Equals((ApiModel)obj);
    public override int GetHashCode()
    {
        return HashCode.Combine(AreaName, ControllerName, ActionName);
    }
}
