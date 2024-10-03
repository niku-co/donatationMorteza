namespace Services.DynamicAuthorization;

public class UserConstant
{
    public const string DefaultUserName = "admin";
    public const string DynamicAuthorizationPolicy = nameof(DynamicAuthorizationPolicy);
    public const string ClaimType = "UserAccess";
    public const string ClaimValue = "Fake";
    public const string AdminRole = "مدیر کل";
    public const string ApiGatewayNikuServices = nameof(ApiGatewayNikuServices);
    public const string NikuServiceScope = nameof(NikuServiceScope);
    public static string DefaultClaimValue = "admin";
}
