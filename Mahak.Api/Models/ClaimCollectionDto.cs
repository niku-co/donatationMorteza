using Services.DynamicAuthorization.DTOs.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Mahak.Api.Models;

public record ClaimCollectionDto
{
    public required string Title { get; init; }
    public required IEnumerable<ClaimInfo> ClaimInfoCollection { get; init; }
}

public class TokenDto
{
    public string access_token { get; set; }
    public string refresh_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public ClaimCollectionDto Claims { get; set; }
    public TokenDto(JwtSecurityToken securityToken)
    {
        access_token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        token_type = "Bearer";
        expires_in = (int)(securityToken.ValidTo - DateTime.UtcNow).TotalSeconds;
    }
}