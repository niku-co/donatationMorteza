using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Entities;
using Mahak.Api.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.DynamicAuthorization;
using Services.DynamicAuthorization.DTOs.Claims;
using Services.Services;
using System.Text.Json;
using WebFramework.Api;
using static Mahak.Api.Models.SettingSelectDto;

namespace Mahak.Api.Controllers.v1;

[ApiVersion("1")]
public class SettingController : BaseController
{
    private readonly IRepository<Setting> _repository;
    private readonly IRepository<Category> _catCepository;
    private readonly IFileService _fileService;

    //private readonly I_repository<SettingTranslation> settingTransRepo;
    private readonly IRepository<Language> langTransRepo;
    private readonly IMapper _mapper;

    public SettingController(IRepository<Setting> repository,
                             IRepository<Category> catCepository,
                             IFileService fileService,
                             IRepository<Language> langTransRepo,
                             IMapper mapper)
    {
        _repository = repository;
        _fileService = fileService;
        //this.settingTransRepo = settingTransRepo;
        this.langTransRepo = langTransRepo;
        _mapper = mapper;
        _catCepository = catCepository;
    }
    [HttpGet("[action]")]
    [DynamicAuthorization(KioskClaims.KioskList)]
    public virtual async Task<ApiResult<SettingSelectDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _repository.TableNoTracking.ProjectTo<SettingSelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Category.Id == id, cancellationToken);

        if (dto == null)
            return NotFound();

        return dto;
    }
    [HttpPut("[Action]")]
    [DynamicAuthorization(KioskClaims.EditKiosk)]
    public virtual async Task<ApiResult<SettingSelectDto>> Update(SettingDto dto, CancellationToken cancellationToken)
    {
        var model = await _repository.Entities.Include(i => i.SettingTranslations).Include(i => i.Languages).Include(i => i.Category)
            .SingleOrDefaultAsync(i => i.Id == dto.Id);

        //var model = await _repository.GetByIdAsync(cancellationToken, dto.Id);
        if (model == null) return NotFound();

        model = dto.ToEntity(_mapper, model);
        model.SettingTranslations = dto.SettingTranslations.Adapt<IList<SettingTranslation>>();

        //var settingTrans = settingTransRepo.Table.Where(i => i.SettingId == model.Id);
        //model.SettingTranslations = settingTrans.ToList();

        if (dto.Languages?.Any() ?? false)
        {
            model.Languages = langTransRepo.Entities.Where(i => dto.Languages.Select(j => j.Id).Contains(i.Id)).ToList();
        }
        else
        {
            model.Languages = null;
        }

        await _repository.UpdateAsync(model, cancellationToken);

        var resultDto = await _repository.TableNoTracking.ProjectTo<SettingSelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

        return resultDto;
    }

    [HttpPut("[Action]")]
    [DynamicAuthorization(KioskClaims.EditKiosk)]
    public virtual async Task<ApiResult<SettingSelectDto>> UpdateMultiCat(MultiCatDto dto, CancellationToken cancellationToken)
    {
        if (dto == null) return NotFound();
        var catIds = JsonSerializer.Deserialize<int[]>(dto.CatIds);
        var tagIds = JsonSerializer.Deserialize<Guid[]>(dto.TagIds);
        var provinceIds = JsonSerializer.Deserialize<Guid[]>(dto.ProvinceIds);

        var items = _catCepository.TableNoTracking.Include(i => i.Tags)
            .Where(c =>
            (catIds.Length == 0 || catIds.Contains(c.Id))
            &&
            (tagIds.Length == 0 || c.Tags.Any(k => tagIds.Contains(k.Id)))
            &&
            (provinceIds.Length == 0 || provinceIds.Contains(c.ProvinceId ?? Guid.Empty))
            );

        Setting lastEntity = null;
        foreach (var item in items)
        {
            var model = await _repository.Entities.Include(i => i.SettingTranslations).Include(i => i.Languages).Include(i => i.Category)
            .SingleOrDefaultAsync(i => i.CategoryId == item.Id);


            if (model == null) return NotFound();

            dto.Data.Id = model.Id;

            model = dto.Data.ToEntity(_mapper, model);
            model.SettingTranslations = dto.Data.SettingTranslations.Adapt<IList<SettingTranslation>>();

            if (dto.Data.Languages?.Any() ?? false)
            {
                model.Languages = langTransRepo.Entities.Where(i => dto.Data.Languages.Select(j => j.Id).Contains(i.Id)).ToList();
            }
            else
            {
                model.Languages = null;
            }
            try
            {
                await _repository.UpdateAsync(model, cancellationToken);
            }
            catch (Exception ex)
            {

            }

            lastEntity = model;
        }


        var resultDto = await _repository.TableNoTracking.ProjectTo<SettingSelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(lastEntity.Id), cancellationToken);

        return resultDto;
    }

    [HttpPut("[Action]")]
    [DynamicAuthorization(KioskClaims.EditKiosk)]
    public async Task<ApiResult> UpdateImage([FromForm] SettingImageDto dto, CancellationToken cancellationToken)
    {


        if (dto.Id == Guid.Empty)
        {
            var catIds = dto.CatIds ?? new List<int>().ToArray();

            var tagIds = JsonSerializer.Deserialize<Guid[]>(dto.TagIds ?? "[]");
            var provinceIds = JsonSerializer.Deserialize<Guid[]>(dto.ProvinceIds ?? "[]");

            var items = _catCepository.TableNoTracking.Include(i => i.Tags)
            .Where(c =>
            (catIds.Length == 0 || catIds.Contains(c.Id))
            &&
            (tagIds.Length == 0 || c.Tags.Any(k => tagIds.Contains(k.Id)))
            &&
            (provinceIds.Length == 0 || provinceIds.Contains(c.ProvinceId ?? Guid.Empty))
            );

            foreach (var item in items)
            {
                var model = await _repository.Entities.SingleOrDefaultAsync(i => i.CategoryId == item.Id);
                if (model == null) return NotFound();
                dto.Id = model.Id;

                model = dto.ToEntity(_mapper, model);

                if (dto.LogoImageFile is not null)
                {
                    var inputImage = await dto.LogoImageFile.GetInputImageAsync(cancellationToken);
                    model.LogoPhysicalPath = _fileService.Save(inputImage, dto.Id.ToString(), ImageType.Logo);
                }


                if (dto.FirstScreenImageFile is not null)
                {
                    var inputImage = await dto.FirstScreenImageFile.GetInputImageAsync(cancellationToken);
                    model.FirstScreenPhysicalPath = _fileService.Save(inputImage, dto.Id.ToString(), ImageType.FirstScreen);
                }

                if (dto.LastScreenImageFile is not null)
                {
                    var inputImage = await dto.LastScreenImageFile.GetInputImageAsync(cancellationToken);
                    model.LastScreenPhysicalPath = _fileService.Save(inputImage, dto.Id.ToString(), ImageType.LastScreen);
                }

                if (dto.BackgroundImageFile is not null)
                {
                    var inputImage = await dto.BackgroundImageFile.GetInputImageAsync(cancellationToken);
                    model.BackgroundPhysicalPath = _fileService.Save(inputImage, dto.Id.ToString(), ImageType.Background);
                }

                if (dto.SwipeCardScreenImageFile is not null)
                {
                    var inputImage = await dto.SwipeCardScreenImageFile.GetInputImageAsync(cancellationToken);
                    model.SwipeCardPhysicalPath = _fileService.Save(inputImage, dto.Id.ToString(), ImageType.SwipeCard);
                }
                await _repository.UpdateAsync(model, cancellationToken);
            }
        }
        else
        {
            var model = await _repository.Entities.SingleOrDefaultAsync(i => i.Id == dto.Id);
            if (model == null) return NotFound();

            model = dto.ToEntity(_mapper, model);

            if (dto.LogoImageFile is not null)
            {
                var inputImage = await dto.LogoImageFile.GetInputImageAsync(cancellationToken);
                model.LogoPhysicalPath = _fileService.Save(inputImage, dto.Id.ToString(), ImageType.Logo);
            }

            if (dto.FirstScreenImageFile is not null)
            {
                var inputImage = await dto.FirstScreenImageFile.GetInputImageAsync(cancellationToken);
                model.FirstScreenPhysicalPath = _fileService.Save(inputImage, dto.Id.ToString(), ImageType.FirstScreen);
            }

            if (dto.LastScreenImageFile is not null)
            {
                var inputImage = await dto.LastScreenImageFile.GetInputImageAsync(cancellationToken);
                model.LastScreenPhysicalPath = _fileService.Save(inputImage, dto.Id.ToString(), ImageType.LastScreen);
            }

            if (dto.BackgroundImageFile is not null)
            {
                var inputImage = await dto.BackgroundImageFile.GetInputImageAsync(cancellationToken);
                model.BackgroundPhysicalPath = _fileService.Save(inputImage, dto.Id.ToString(), ImageType.Background);
            }

            if (dto.SwipeCardScreenImageFile is not null)
            {
                var inputImage = await dto.SwipeCardScreenImageFile.GetInputImageAsync(cancellationToken);
                model.SwipeCardPhysicalPath = _fileService.Save(inputImage, dto.Id.ToString(), ImageType.SwipeCard);
            }
            await _repository.UpdateAsync(model, cancellationToken);
        }


        return Ok();
    }

    [HttpGet("[Action]/{deviceId}/{imageType}")]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public async Task<ApiResult<FileDto>> GetImage(string deviceId, ImageType imageType, CancellationToken cancellationToken)
    {
        var dto = await _repository.TableNoTracking.ProjectTo<SettingSelectDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(i => i.Category.DeviceId.Equals(deviceId), cancellationToken);
        if (dto is null)
            return NotFound();

        var result = _fileService.Get($"{dto.Id}_{imageType}");
        return new FileDto { Content = result.content, FileName = result.fileName };
    }

    [HttpGet("[action]/{deviceId}")]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public virtual async Task<ApiResult<SettingSelectDto>> GetByDeviceId(string deviceId, CancellationToken cancellationToken)
    {
        var dto = await _repository.TableNoTracking.ProjectTo<SettingSelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Category.DeviceId == deviceId, cancellationToken);

        if (dto == null)
            return NotFound();

        return dto;
    }

    [HttpGet("[Action]/{id}")]
    public ApiResult<IList<FileDto>> GetImages(string id)
    {
        List<FileDto> items = new();

        //var zipName = $"{id}-{DateTime.Now:yyyy_MM_dd-HH_mm_ss}.zip";
        //using MemoryStream ms = new();
        ////required: using System.IO.Compression;
        //using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
        //{
        //    _fileService.GetFiles(id).ToList().ForEach(file =>
        //    {
        //        var result = _fileService.Get(Path.GetFileName(file));                
        //        var entry = zip.CreateEntry(result.fileName);
        //        using var fileStream = new MemoryStream(result.content);
        //        using var entryStream = entry.Open();
        //        fileStream.CopyTo(entryStream);
        //    });
        //}
        //return File(ms.ToArray(), "application/zip", zipName);

        _fileService.GetFiles(id).ToList().ForEach(file =>
        {
            var result = _fileService.Get(Path.GetFileName(file));
            items.Add(new FileDto { Content = result.content, FileName = result.fileName });
        });
        return items;
    }

    [HttpDelete("[action]/{id}/{imageType}")]
    public ApiResult DeleteImage(string id, string imageType)
    {
        _fileService.Delete($"{id}_{imageType}");
        return Ok();
    }

}
