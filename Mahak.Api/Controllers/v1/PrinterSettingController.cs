using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contracts;
using Entities;
using Mahak.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Services.DynamicAuthorization.DTOs.Claims;
using Services.DynamicAuthorization;
using Services.Services;
using WebFramework.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Common.Utilities;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using static Mahak.Api.Models.SettingSelectDto;

namespace Mahak.Api.Controllers.v1
{
    [ApiVersion("1")]
    public class PrinterSettingController : BaseController
    {
        private readonly IRepository<PrinterSetting> _repository;
        private readonly IFileService _fileService;

        private readonly IRepository<Language> langTransRepo;
        private readonly IMapper _mapper;

        public PrinterSettingController(IRepository<PrinterSetting> repository,
                             IFileService fileService,
                             IRepository<Language> langTransRepo,
                             IMapper mapper)
        {
            _repository = repository;
            _fileService = fileService;
            this.langTransRepo = langTransRepo;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        [DynamicAuthorization(PrinterSettingClaims.EditPrinterSetting)]
        public virtual async Task<ApiResult<PrinterSettingDto>> GetPrinterSetting(CancellationToken cancellationToken)
        {
            try
            {
                var dto = _repository.TableNoTracking.ProjectTo<PrinterSettingDto>(_mapper.ConfigurationProvider)
                .FirstOrDefault();

                if (dto == null)
                    return NotFound();

                var logoInfo = _fileService.GetFiles(dto.Id.ToString()).FirstOrDefault();
                if (logoInfo != null)
                {
                    var result = _fileService.Get(Path.GetFileName(logoInfo));
                    dto.logoValue = (new FileDto { Content = result.content, FileName = result.fileName });
                }


                return dto;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        [HttpPost("[action]")]
        [DynamicAuthorization(PrinterSettingClaims.EditPrinterSetting)]
        public virtual async Task<bool> EditPrintSetting([FromForm] PrinterSettingDto entity, CancellationToken cancellationToken)
        {
            var dto = await _repository.TableNoTracking.SingleOrDefaultAsync(c => c.Id == entity.Id, cancellationToken);

            if (dto == null)
            {
                dto = new PrinterSetting();
                dto.Id = Guid.Empty;
            }

            dto.ActivePrinter = entity.ActivePrinter;
            dto.PrintText = entity.PrintText;
            dto.QRCodeUrl = entity.QRCodeUrl;

            if (dto.Id == Guid.Empty)
            {
                await _repository.AddAsync(dto, cancellationToken);
            }
            else
            {
                await _repository.UpdateAsync(dto, cancellationToken);
            }

            if (dto.Id != Guid.Empty)
            {
                if (entity.LogoImageFile is not null)
                {
                    var inputImage = await entity.LogoImageFile.GetInputImageAsync(cancellationToken);
                    dto.LogoPhysicalPath = _fileService.Save(inputImage, dto.Id.ToString(), ImageType.printLogo);
                    await _repository.UpdateAsync(dto, cancellationToken);
                }
            }
            return true;
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = "BasicAuthentication")]
        public virtual async Task<ApiResult<PrinterSettingDto>> GetPrinterSettingItem(CancellationToken cancellationToken)
        {
            try
            {
                var dto = _repository.TableNoTracking.ProjectTo<PrinterSettingDto>(_mapper.ConfigurationProvider)
                .FirstOrDefault();

                if (dto == null)
                    return NotFound();

                var logoInfo = _fileService.GetFiles(dto.Id.ToString()).FirstOrDefault();
                if (logoInfo != null)
                {
                    var result = _fileService.Get(Path.GetFileName(logoInfo));
                    dto.logoValue = (new FileDto { Content = result.content, FileName = result.fileName });
                }

                return dto;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        [HttpDelete("[action]/{id}")]
        public ApiResult DeleteImage(string id)
        {
            _fileService.Delete($"{id}_{ImageType.printLogo}");
            return Ok();
        }

    }
}
