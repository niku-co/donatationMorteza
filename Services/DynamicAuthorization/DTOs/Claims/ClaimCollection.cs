using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Services.DynamicAuthorization.DTOs.Claims;

public record ClaimCollection
{
    public required string Title { get; init; }
    public required IEnumerable<ClaimInfo> ClaimInfoCollection { get; init; }

    //[NotMapped]
    //[JsonIgnore]
    public IEnumerable<Claim> Claims { get => GetClaims(); }
    private IEnumerable<Claim> GetClaims()
    {
        foreach (var item in ClaimInfoCollection.Where(c => c.Selected))
        {
            yield return new Claim(ClaimStore.UserAccess, item.Name);
        }
    }
}
