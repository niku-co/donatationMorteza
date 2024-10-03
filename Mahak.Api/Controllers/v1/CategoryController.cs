using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Entities;
using Entities.User;
using Mahak.Api.Hubs;
using Mahak.Api.Models;
using Mahak.Api.Models.Categories.Queries.Pagination;
using Mahak.Api.Models.Items.Queries.Pagination;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Services.DynamicAuthorization;
using Services.DynamicAuthorization.DTOs.Claims;
using Services.Services;
using System.Globalization;
using System.Text;
using WebFramework.Api;
using IMapper = AutoMapper.IMapper;
using Item = Entities.Item;
using Tag = Entities.Tag;

namespace Mahak.Api.Controllers.v1;

[ApiVersion("1")]
//[AllowAnonymous]
public class CategoryController : BaseController
{
    private readonly IRepository<Entities.Category> _repository;
    private readonly IRepository<Entities.CategoryPingLog> _cplRepository;
    private readonly IRepository<Entities.Item> _itemRepository;

    //private readonly IRepository<Category> Repository;
    private readonly IRepository<Language> _langRepository;
    private readonly IRepository<Setting> _settingRepository;
    private readonly IRepository<Tag> _tagRepository;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IRepository<CategoryTag> _catTagRepository;
    private readonly IMapper _mapper;

    //private readonly IMapper Mapper;

    //private readonly ISmsService _smsService;
    private readonly IFileService _fileService;
    private readonly ILogger<CategoryController> _logger;
    private readonly IMediator _mediator;
    private readonly IRepository<ProvinceSettting> _psRepository;
    private readonly IRepository<UserProvince> _upRepository;
    private readonly IRepository<UserTag> _utRepository;
    private readonly UserManager<User> _userManager;
    private readonly IHubContext<ChatHub> _hubContext;


    public CategoryController(IRepository<Entities.Category> repository,
                              IRepository<Language> langRepository,
                              IRepository<Setting> settingRepository,
                              IMapper mapper,
                              //ISmsService smsService,
                              IFileService fileService,
                              ILogger<CategoryController> logger,
                              IMediator mediator,
                              IRepository<Entities.CategoryPingLog> cplRepository,
                              IHubContext<ChatHub> hubcontext,
                              IRepository<CategoryTag> catTagRepository,
                              IRepository<UserTag> utRepository,
        IRepository<ProvinceSettting> psRepository
        , IRepository<UserProvince> upRepository, UserManager<User> userManager,
       IRepository<Tag> tagtingRepository, IRepository<Cart> cartRepository,
       IRepository<Item> itemRepository)
    {
        _repository = repository;
        _langRepository = langRepository;
        _settingRepository = settingRepository;
        _mapper = mapper;
        _fileService = fileService;
        _logger = logger;
        _mediator = mediator;
        _psRepository = psRepository;
        _upRepository = upRepository;
        _userManager = userManager;
        _hubContext = hubcontext;
        _tagRepository = tagtingRepository;
        _catTagRepository = catTagRepository;
        _cartRepository = cartRepository;
        _itemRepository = itemRepository;
        _cplRepository = cplRepository;
        _utRepository = utRepository;
    }
    [HttpGet]
    [DynamicAuthorization(KioskClaims.KioskList)]
    public virtual async Task<ActionResult<List<CategorySelectDto>>> Get(CancellationToken cancellationToken)
    {
        List<CategorySelectDto> list = null;
        var CList = _repository.TableNoTracking;
        var provinseSetting = _psRepository.TableNoTracking.FirstOrDefault();
        if (provinseSetting != null && provinseSetting.ApplyProvince)
        {
            var userName = HttpContext.User.Identity.GetUserName();
            var user = await _userManager.FindByNameAsync(userName);
            var userId = 0;
            if (user != null)
            {
                if (user.UserName == "admin")//|| user.UserName == "nikukiosk")
                {
                }
                else
                {
                    userId = user.Id;

                    var provinceIds = _upRepository.TableNoTracking.Where(c => c.UserId == userId).Select(c => c.ProvinceId).ToList();
                    if (provinceIds.Any())
                        CList = CList.Where(i => i.ProvinceId != null && provinceIds.Contains(i.ProvinceId.Value));
                }
            }

        }

        list = await CList.ProjectTo<CategorySelectDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);


        return Ok(list);
    }

    [HttpGet("{id}")]
    [DynamicAuthorization(KioskClaims.KioskList)]
    public virtual async Task<ApiResult<CategorySelectDto>> Get(int id, CancellationToken cancellationToken)
    {
        var dto = await _repository.TableNoTracking.ProjectTo<CategorySelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

        if (dto == null)
            return NotFound();

        return dto;
    }

    [HttpDelete("{id}")]
    [DynamicAuthorization(KioskClaims.DeleteKiosk)]
    public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
    {
        var model = await _repository.GetByIdAsync(cancellationToken, id);

        if (model == null)
            return NotFound();

        await _repository.DeleteAsync(model, cancellationToken);

        return Ok();
    }

    [HttpPost("[action]")]
    [DynamicAuthorization(KioskClaims.CreateKiosk)]
    public async Task<ApiResult<CategorySelectDto>> CreateCategory([FromForm] CategoryDto dto, CancellationToken cancellationToken)
    {
        var model = dto.ToEntity(_mapper);
        if (dto.ProvinceId == Guid.Empty)
        {
            model.ProvinceId = null;
            model.Province = null;
        }

        model.Setting = new Setting();
        var lang = _langRepository.Table.FirstOrDefault(i => i.CodeName.Equals("Fa"));
        model.Setting.Languages = new List<Language>
        {
            lang
        };
        //model.Active = true;

        var (count1, count2) = await CheckRepeatedAsync(model, cancellationToken);
        if (count1 > 0)
            return BadRequest("شناسه دستگاه نمی‌تواند تکراری باشد!");
        if (count2 > 0)
            return BadRequest("نام دستگاه نمی‌تواند تکراری باشد!");
        if (dto.Thumbnail is not null)
        {
            var image = await dto.Thumbnail.GetInputImageAsync(cancellationToken);
            model.ThumbnailPath = _fileService.Save(image, dto.Id.ToString(), ImageType.Thumbnail);
        }
        try
        {
            await _repository.AddAsync(model, cancellationToken);

            var oldCatTags = _catTagRepository.TableNoTracking.Where(c => c.CategoriesId == model.Id).ToList();
            await _catTagRepository.DeleteRangeAsync(oldCatTags, cancellationToken);

            if (dto.TagIds?.Length > 0)
            {
                var tagIds = System.Text.Json.JsonSerializer.Deserialize<Guid[]>(dto.TagIds);
                var catTags = _tagRepository.TableNoTracking.Where(t => tagIds.Contains(t.Id)).ToList().Select(c => new CategoryTag()
                {
                    CategoriesId = model.Id,
                    TagsId = c.Id
                });

                await _catTagRepository.AddRangeAsync(catTags, cancellationToken);
            }


        }
        catch (Exception ex)
        {

        }
        var resultDto = await _repository.TableNoTracking.ProjectTo<CategorySelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

        return resultDto;
    }

    [HttpGet("[action]")]
    [DynamicAuthorization(KioskClaims.KioskList)]
    public async Task<ApiResult<IList<CategoryMapData>>> GetMapData(CancellationToken cancellationToken)
    {
        var userName = HttpContext.User.Identity.GetUserName();
        var user = await _userManager.FindByNameAsync(userName);
        var userId = user.Id;

        var list = _repository.TableNoTracking;

        if (user.UserName != "admin")
        {
            var provinceIds = _upRepository.TableNoTracking.Where(c => c.UserId == userId).Select(c => c.ProvinceId).ToList();
            var tagsIds = _utRepository.TableNoTracking.Where(c => c.UserId == userId).Select(c => c.TagId).ToList();

            if (tagsIds.Any())
            {
                var cats = _catTagRepository.TableNoTracking.Where(c => tagsIds.Contains(c.TagsId)).Select(c => c.CategoriesId).ToList();
                list = list.Where(i => cats.Contains(i.Id));

            }

            if (provinceIds.Any())
            {
                list = list.Where(i => i.ProvinceId != null && provinceIds.Contains(i.ProvinceId.Value));

            }

        }

        var dto = list.Include(b => b.Setting).ProjectTo<CategorySelectDto>(_mapper.ConfigurationProvider);

        var groupItems = dto.GroupBy(i => new { i.Latitude, i.Longitude }).Select(i => new CategoryMapData
        {
            Latitude = i.Key.Latitude ?? 0,
            Longitude = i.Key.Longitude ?? 0,
            Count = i.Count(),
            CategoryMapDataItems = i.Select(j => new CategoryMapDataItem
            {
                Active = j.Active,
                State = j.State ?? false,
                LocName = j.LocName ?? "",
                DeviceId = j.DeviceId ?? "",
                Title = j.Title ?? "",
            }),
        });

        if (groupItems is not null)
        {
            var result = await groupItems.ToListAsync(cancellationToken);
            return result;
        }
        return NotFound();
    }

    [HttpPut("Update")]
    [DynamicAuthorization(KioskClaims.EditKiosk)]
    public async Task<ApiResult<CategorySelectDto>> Update(CategoryUpdateDto dto, CancellationToken cancellationToken)
    {
        var model = await _repository.GetByIdAsync(cancellationToken, dto.Id);
        if (model == null)
            return NotFound();
        model = dto.ToEntity(_mapper, model);
        if (dto.ProvinceId == Guid.Empty)
        {
            model.ProvinceId = null;
            model.Province = null;
        }
        var (count1, count2) = await CheckRepeatedAsync(model, cancellationToken);
        if (count1 > 1)
            return BadRequest("شناسه دستگاه نمی‌تواند تکراری باشد!");
        if (count2 > 1)
            return BadRequest("نام دستگاه نمی‌تواند تکراری باشد!");
        try
        {
            await _repository.UpdateAsync(model, cancellationToken);

            var oldCatTags = _catTagRepository.TableNoTracking.Where(c => c.CategoriesId == model.Id).ToList();
            await _catTagRepository.DeleteRangeAsync(oldCatTags, cancellationToken);

            if (dto.TagIds?.Length > 0)
            {
                var catTags = _tagRepository.TableNoTracking.Where(t => dto.TagIds.Contains(t.Id)).ToList().Select(c => new CategoryTag()
                {
                    CategoriesId = model.Id,
                    TagsId = c.Id
                });

                await _catTagRepository.AddRangeAsync(catTags, cancellationToken);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }


        var resultDto = await _repository.TableNoTracking.ProjectTo<CategorySelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

        return resultDto;
    }
    [HttpPut("[action]")]
    [DynamicAuthorization(KioskClaims.EditKiosk)]
    public async Task<ApiResult<CategorySelectDto>> UpdateImage([FromForm] CategoryThumbnail categoryThumbnail, CancellationToken cancellationToken)
    {
        var model = await _repository.GetByIdAsync(cancellationToken, categoryThumbnail.Id);
        if (model == null)
            return NotFound();

        if (categoryThumbnail.File is not null)
        {
            var image = await categoryThumbnail.File.GetInputImageAsync(cancellationToken);
            model.ThumbnailPath = _fileService.Save(image, categoryThumbnail.Id.ToString(), ImageType.Thumbnail);
        }
        else
        {
            _fileService.Delete($"{categoryThumbnail.Id}_{ImageType.Thumbnail}");
            model.ThumbnailPath = null;
        }
        await _repository.UpdateAsync(model, cancellationToken);

        var resultDto = await _repository.TableNoTracking.ProjectTo<CategorySelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

        return resultDto;
    }
    [HttpDelete("[action]/{id}")]
    [DynamicAuthorization(KioskClaims.EditKiosk)]
    public async Task<ApiResult<CategorySelectDto>> DeleteImageAsync(int id, CancellationToken cancellationToken)
    {
        var model = await _repository.GetByIdAsync(cancellationToken, id);
        if (model == null)
            return NotFound();

        _fileService.Delete($"{id}_{ImageType.Thumbnail}");
        model.ThumbnailPath = null;

        await _repository.UpdateAsync(model, cancellationToken);

        var resultDto = await _repository.TableNoTracking.ProjectTo<CategorySelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

        return resultDto;
    }
    async Task<(int, int)> CheckRepeatedAsync(Entities.Category model, CancellationToken cancellationToken)
    {
        try
        {
            var cats = await _repository.TableNoTracking.ToListAsync(cancellationToken);
            var count1 = cats?.Count(i => i.DeviceId.Equals(model.DeviceId, StringComparison.OrdinalIgnoreCase)) ?? 0;
            var count2 = cats?.Count(i => i.Title.Equals(model.Title, StringComparison.OrdinalIgnoreCase)) ?? 0;
            return (count1, count2);
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    [HttpGet("[action]")]
    [DynamicAuthorization(KioskClaims.KioskList)]
    public async Task<CategoryPaginationDto> GetBySearch([FromQuery] GetCategoryPaginationQuery paginationQuery, CancellationToken cancellationToken)
    {
        var userName = HttpContext.User.Identity.GetUserName();
        paginationQuery.Username = userName;
        var result = await _mediator.Send(paginationQuery, cancellationToken);
        return result;
    }

    [HttpGet("[action]/{deviceId}")]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public virtual async Task<ApiResult<CategorySelectDto>> GetItems(string deviceId, CancellationToken cancellationToken)
    {
        //deviceId = "12345";
        var dto = await _repository.TableNoTracking
            .ProjectTo<CategorySelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.DeviceId.Equals(deviceId), cancellationToken);
        if (dto == null)
        {
            _logger.LogInformation("Device Not Found!");
            return NotFound();
        }

        var model = await _repository.GetByIdAsync(cancellationToken, dto.Id);
        model.LastRequest = DateTime.Now;
        _logger.LogInformation($"DateTime.Now: {DateTime.Now}");
        await _repository.UpdateAsync(model, cancellationToken);
        _logger.LogInformation($"LastRequest: {model.LastRequest}");

        return dto;
    }

    [HttpPost("[action]")]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public virtual async Task<ApiResult<CategorySelectDto>> PostErrorMessage(CategoryErrorDto dto, CancellationToken cancellationToken)
    {
        var model = await _repository.Table
            .SingleOrDefaultAsync(p => p.DeviceId.Equals(dto.DeviceId), cancellationToken);
        if (model == null)
            return NotFound();
        model.ErrorMessage = dto.ErrorMessage;
        await _repository.UpdateAsync(model, cancellationToken);


        var resultDto = await _repository.TableNoTracking.ProjectTo<CategorySelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

        return resultDto;
    }

    [HttpGet("[action]/{deviceId}")]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public async Task<ApiResult<List<LanguageKioskDto>>> GetLangs(string deviceId, CancellationToken cancellationToken)
    {
        var setting = await _settingRepository.TableNoTracking.Include(i => i.Category).Include(i => i.Languages)
            //.ProjectTo<CategorySelectDto>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(i => i.Category.DeviceId.Equals(deviceId), cancellationToken);
        if (setting == null)
            return NotFound();
        var result = setting.Languages.Adapt<List<LanguageKioskDto>>();
        return result;
    }

    [HttpGet("[action]")]
    [DynamicAuthorization(KioskClaims.KioskList)]
    public async Task<IActionResult> Csv([FromQuery] GetCategoryPaginationQuery paginationQuery, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(paginationQuery, cancellationToken);
        var builder = new StringBuilder();
        builder.AppendLine("آیدی,نام مجازی دستگاه,نام دستگاه, ip دستگاه, شماره ترمینال, وضعیت");
        foreach (var category in result.Items)
        {
            builder.AppendLine($"{category.Id},{category.Title},{category.DeviceId},{category.Ip},{category.TerminalNo},{category.Active}");
        }

        return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "Categories.csv");
    }

    //[HttpGet("[action]")]
    //public async Task<IActionResult> Excel([FromQuery] GetCategoryExcelQuery exportQuery, CancellationToken cancellationToken)
    //{
    //    var content = await _mediator.Send(exportQuery, cancellationToken);

    //    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Categories.xlsx");
    //}

    [HttpGet("[action]")]
    [DynamicAuthorization(KioskClaims.KioskList)]
    public async Task<ApiResult<IList<CategoryPieChartData>>> GetPieChartData(CancellationToken cancellationToken)
    {
        try
        {
            var userName = HttpContext.User.Identity.GetUserName();
            var user = await _userManager.FindByNameAsync(userName);
            var userId = user.Id;

            var list = _repository.TableNoTracking;

            if (user.UserName != "admin")
            {
                var provinceIds = _upRepository.TableNoTracking.Where(c => c.UserId == userId).Select(c => c.ProvinceId).ToList();
                var tagsIds = _utRepository.TableNoTracking.Where(c => c.UserId == userId).Select(c => c.TagId).ToList();

                if (tagsIds.Any())
                {
                    var cats = _catTagRepository.TableNoTracking.Where(c => tagsIds.Contains(c.TagsId)).Select(c => c.CategoriesId).ToList();
                    list = list.Where(i => cats.Contains(i.Id));

                }

                if (provinceIds.Any())
                {
                    list = list.Where(i => i.ProvinceId != null && provinceIds.Contains(i.ProvinceId.Value));

                }

            }

            var dto = list.ProjectTo<CategorySelectDto>(_mapper.ConfigurationProvider); ;

            var groupItems = dto.Select(i => new
            {
                i.Active,
                i.State,
                Count = 1

            }).ToList().GroupBy(i => new { i.Active, i.State }).Select(i => new CategoryPieChartData
            {
                Active = i.Key.Active,
                State = i.Key.State ?? false,
                Count = i.Count()

            });

            if (groupItems is not null)
            {
                var result = groupItems.ToList();
                return result;
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    [HttpGet("[action]")]
    [DynamicAuthorization(KioskClaims.KioskList)]
    public async Task<ApiResult<CartLineChartData>> GetLineChartData(int chartType, CancellationToken cancellationToken)
    {

        try
        {
            var item = new CartLineChartData();
            List<string> labels = new List<string>();
            List<long> data = new List<long>();
            List<int> count = new List<int>();

            var userName = HttpContext.User.Identity.GetUserName();
            var user = await _userManager.FindByNameAsync(userName);
            var userId = user.Id;

            var list = _repository.TableNoTracking;

            if (user.UserName != "admin")
            {
                var provinceIds = _upRepository.TableNoTracking.Where(c => c.UserId == userId).Select(c => c.ProvinceId).ToList();
                var tagsIds = _utRepository.TableNoTracking.Where(c => c.UserId == userId).Select(c => c.TagId).ToList();

                if (tagsIds.Any())
                {
                    var cats = _catTagRepository.TableNoTracking.Where(c => tagsIds.Contains(c.TagsId)).Select(c => c.CategoriesId).ToList();
                    list = list.Where(i => cats.Contains(i.Id));

                }

                if (provinceIds.Any())
                {
                    list = list.Where(i => i.ProvinceId != null && provinceIds.Contains(i.ProvinceId.Value));

                }
            }
            var catIds = list.Select(c => c.Id);

            if (chartType == 0)
            {
                var datetime = DateTime.Now.AddDays(-6);
                var index = 1;
                while (index <= 7)
                {
                    labels.Add(PersionDayOfWeek(datetime) + "\n" + datetime.Date.ToPersianShortDateStringFormat());

                    var startDate = getLongDate(datetime.Date);
                    var endtDate = getLongDate(datetime.Date.AddMinutes(23 * 60 + 59));

                    var dto = _cartRepository.TableNoTracking.Include(c => c.Category)
                        .Where(c => catIds.Contains(c.Category.Id) && c.DateTime >= startDate && c.DateTime <= endtDate);

                    var groupItems = dto.GroupBy(i => i.RefNum).Select(i => i.Sum(x => x.TotalPrice));
                    data.Add(groupItems.Sum(x => x));

                    var groupItems1 = dto.GroupBy(i => i.RefNum).Select(i => i.Count());
                    count.Add(groupItems1.Sum(x => x));

                    index++;
                    datetime = datetime.AddDays(1);

                }

                item.Label = labels.ToArray();
                item.Data = data.ToArray();
                item.Count = count.ToArray();
            }
            else if (chartType == 1)
            {
                var datetime = DateTime.Now.AddMonths(-1);
                var index = 0;
                while (index <= 30)
                {
                    labels.Add(datetime.Date.ToPersianShortDateStringFormat());

                    var startDate = getLongDate(datetime.Date);
                    var endtDate = getLongDate(datetime.Date.AddMinutes(23 * 60 + 59));

                    var dto = _cartRepository.TableNoTracking.Include(c => c.Category)
                        .Where(c => catIds.Contains(c.Category.Id) && c.DateTime >= startDate && c.DateTime <= endtDate);

                    var groupItems = dto.GroupBy(i => i.RefNum).Select(i => i.Sum(x => x.TotalPrice));
                    data.Add(groupItems.Sum(x => x));

                    var groupItems1 = dto.GroupBy(i => i.RefNum).Select(i => i.Count());
                    count.Add(groupItems1.Sum(x => x));

                    index++;
                    datetime = datetime.AddDays(1);

                }

                item.Label = labels.ToArray();
                item.Data = data.ToArray();
                item.Count = count.ToArray();
            }
            if (chartType == 2)
            {
                var datetime = DateTime.Now.AddMonths(-11);
                var index = 1;
                while (index <= 12)
                {
                    labels.Add(datetime.Date.ToPersianShortMonthDateStringFormat());

                    var p = new PersianCalendar();
                    var dateTime = datetime.Date;

                    DateTime dt = new DateTime(p.GetYear(dateTime), p.GetMonth(dateTime), 1, p);

                    var startDate = getLongDate(dt.Date);
                    var days = p.GetDaysInMonth(p.GetYear(dateTime), p.GetMonth(dateTime));

                    dt = new DateTime(p.GetYear(dateTime), p.GetMonth(dateTime), days, p);

                    var endtDate = getLongDate(dt.Date.AddMinutes(23 * 60 + 59));

                    var dto = _cartRepository.TableNoTracking.Include(c => c.Category)
                        .Where(c => catIds.Contains(c.Category.Id) && c.DateTime >= startDate && c.DateTime <= endtDate);

                    var groupItems = dto.GroupBy(i => i.RefNum).Select(i => i.Sum(x => x.TotalPrice));
                    data.Add(groupItems.Sum(x => x));

                    var groupItems1 = dto.GroupBy(i => i.RefNum).Select(i => i.Count());
                    count.Add(groupItems1.Sum(x => x));

                    index++;
                    datetime = datetime.AddMonths(1);

                }

                item.Label = labels.ToArray();
                item.Data = data.ToArray();
                item.Count = count.ToArray();
            }

            return item;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    [NonAction]

    string add0(int digit)
    {
        return digit < 10 ? ("0" + digit) : (digit + "");
    }
    [NonAction]
    long getLongDate(DateTime date)
    {
        return long.Parse((date.Year - 2000) + "" + add0(date.Month) + "" + add0(date.Day) + "" + add0(date.Hour) + "" + add0(date.Minute) + "" + add0(date.Second));
    }

    [HttpGet("[action]")]
    [DynamicAuthorization(KioskClaims.KioskList)]
    public async Task<ApiResult<DashboardData>> GetDashboardData(CancellationToken cancellationToken)
    {
        var data = new DashboardData();
        var userName = HttpContext.User.Identity.GetUserName();
        try
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userId = user.Id;

            var datetime = DateTime.Now;

            var startDate = getLongDate(datetime.Date);
            var endtDate = getLongDate(datetime.Date.AddMinutes(23 * 60 + 59));

            var list = _repository.TableNoTracking;

            if (user.UserName != "admin")
            {
                var provinceIds = _upRepository.TableNoTracking.Where(c => c.UserId == userId).Select(c => c.ProvinceId).ToList();
                var tagsIds = _utRepository.TableNoTracking.Where(c => c.UserId == userId).Select(c => c.TagId).ToList();

                if (tagsIds.Any())
                {
                    var cats = _catTagRepository.TableNoTracking.Where(c => tagsIds.Contains(c.TagsId)).Select(c => c.CategoriesId).ToList();
                    list = list.Where(i => cats.Contains(i.Id));

                }

                if (provinceIds.Any())
                {
                    list = list.Where(i => i.ProvinceId != null && provinceIds.Contains(i.ProvinceId.Value));
                }

            }

            data.TotalActiveCategory = list.Count();

            var catIds = list.Select(i => i.Id);

            var dto = _cartRepository.TableNoTracking.Include(c => c.Category)
                .Where(c => c.DateTime >= startDate && c.DateTime <= endtDate && catIds.Contains(c.Category.Id));

            var groupItems = dto.GroupBy(i => i.RefNum).Select(i => i.Count());
            data.DailyTransaction = groupItems.Sum(x => x);

            var groupItems2 = dto.GroupBy(i => i.RefNum).Select(i => i.Sum(c => c.TotalPrice));
            data.DailyAmountTransaction = groupItems2.Sum(x => x);


            data.TotalItems = _itemRepository.TableNoTracking.Include(c => c.Categories).Where(c => (c.Active ?? false) && c.Categories.Any()).Count();

            return data;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    [NonAction]

    public string PersionDayOfWeek(DateTime date)
    {
        switch (date.DayOfWeek)
        {
            case DayOfWeek.Saturday:
                return "شنبه";
            case DayOfWeek.Sunday:
                return "یک شنبه";
            case DayOfWeek.Monday:
                return "دو شنبه";
            case DayOfWeek.Tuesday:
                return "سه شنبه";
            case DayOfWeek.Wednesday:
                return "چهارشنبه";
            case DayOfWeek.Thursday:
                return "پنج شنبه";
            case DayOfWeek.Friday:
                return "جمعه";
            default:
                throw new Exception();
        }
    }

    //[HttpGet("[action]/{id}")]
    //[DynamicAuthorization(KioskClaims.RestartKiosk)]
    //public async Task<bool> RestartKiosk(int id, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var model = await _repository.GetByIdAsync(cancellationToken, id);

    //        if (model == null)
    //            return false;

    //        await _hubContext.Clients.All.SendAsync("SendRestart", model.DeviceId, cancellationToken);
    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }

    //}

    [HttpPost("[action]")]
    [DynamicAuthorization(ReportClaims.ReportLogs)]
    public async Task<CategoryLogPaginationDto> GeCategoryLogs([FromBody] GetPosLogPaginationQuery request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result;
    }


    [HttpPost("[action]")]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public virtual async Task<ApiResult> PingCategory(CategoryPingDto dto, CancellationToken cancellationToken)
    {
        var model = await _repository.Table
            .SingleOrDefaultAsync(p => p.DeviceId.Equals(dto.DeviceId), cancellationToken);

        if (model == null)
            return NotFound();

        model.LastRequest = DateTime.Now;
        await _repository.UpdateAsync(model, cancellationToken);

        var item = _cplRepository.TableNoTracking
            .Where(c => c.CategoryId == model.Id)
            .OrderByDescending(c => c.Date)
            .ToList()
            .FirstOrDefault(c => c.Date.Date == dto.Date.Date);

        item ??= new CategoryPingLog
        {
            Id = Guid.Empty
        };

        item.DeviceId = dto.DeviceId;
        item.CategoryId = model.Id;
        item.TurnOnHours = dto.TurnOnHours;
        item.TotalRequestCounter = dto.TotalCounter;
        item.PosSuccessCounter = dto.PosSuccessCounter;
        item.PosTotalRequestCounter = dto.PosTotalCounter;
        item.SuccessCounter = dto.SuccessCounter;
        item.InsertTime = NativeDateTime.Now();

        //TimeZoneInfo iranTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tehran");
        //DateTime iranTime = TimeZoneInfo.ConvertTime(dto.DateTime, iranTimeZone);

        item.Date = dto.Date.DateTime;

        _logger.LogInformation("InsertTime: {0}", item.InsertTime);
        _logger.LogInformation("Date: {0}", item.Date);
        _logger.LogInformation("dto: {0}", JsonConvert.SerializeObject(dto));


        if (item.Id != Guid.Empty)
        {
            await _cplRepository.UpdateAsync(item, cancellationToken);
        }
        else
            await _cplRepository.AddAsync(item, cancellationToken);


        return Ok();
    }

}