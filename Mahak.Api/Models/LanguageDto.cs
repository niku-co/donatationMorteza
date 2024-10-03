using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models;

public class LanguageKioskDto : BaseDto<LanguageKioskDto, Language>
{
    public string CodeName { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ErrorMessage { get; set; }
    public bool Active { get; set; }
}
public class LanguageDto : BaseDto<LanguageDto, Language>
{
    public bool Active { get; set; }
}
public class LanguageSelectDto : BaseDto<LanguageSelectDto, Language>
{
    public string Name { get; set; }
    public bool Active { get; set; }
}
