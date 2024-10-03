using Data.Contracts;
using Entities;
using Mahak.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace Mahak.Api.Controllers.v1;

[ApiVersion("1")]
public class VersionController : BaseController
{
    private readonly IRepository<Category> _repository;

    public VersionController(IRepository<Category> repository)
    {
        _repository = repository;
    }
    [HttpGet]
    public virtual ApiResult<ApiVersionDto> GetApiVersion()
    {
        //var assemblyVersion = typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        return new ApiVersionDto { VersionInfo = "1.2.17" };
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public virtual async Task<ApiResult> UpdateApkVersion(ApkVersionDto apkVersionDto, CancellationToken cancellationToken)
    {
        var kiosk = _repository.Table.FirstOrDefault(x => x.DeviceId == apkVersionDto.DeviceId);
        if (kiosk is null)
            return NotFound();
        kiosk.ApkVersion = apkVersionDto.ApkVersion;
        await _repository.UpdateAsync(kiosk, cancellationToken);
        return Ok();
    }
}
