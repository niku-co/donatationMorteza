using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Utilities;
using Data.Contracts;
using Entities;
using Mahak.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services.Services;

namespace Mahak.Api.HostServices;

public class CheckErrorService : BackgroundService
{
    private readonly IRepository<Category> _repository;
    private readonly IMapper _mapper;
    private readonly IRepository<CategoryLog> _logRepository;
    private readonly IOptionsSnapshot<SiteSettings> _settings;
    private readonly SmsService _smsService;

    public CheckErrorService(IRepository<Category> repository,
                             IRepository<CategoryLog> logCepository,
                             IMapper mapper,
                             SmsService smsService,
                             IOptionsSnapshot<SiteSettings> settings)
    {
        _repository = repository;
        _mapper = mapper;
        _smsService = smsService;
        _settings = settings;
        _logRepository = logCepository;
    }
    public async Task CheckingAsync(object state)
    {
        try
        {
            if (state is null)
                return;

            var (errorList, mobileList, dicError) = await CheckCategoryStateAsync((CancellationToken)state);
            int index = 0;
            foreach (var error in errorList ?? Enumerable.Empty<string>())
            {
                if (!string.IsNullOrEmpty(mobileList[index]))
                    await _smsService.SendAsync(mobileList[index], error);
                index++;
            }

            if (dicError != null)
            {
                foreach (var error in dicError)
                {
                    if (!string.IsNullOrEmpty(error.Key))
                        await _smsService.SendAsync(error.Key, error.Value);
                }
            }

        }
        catch (Exception)
        {

            throw;
        }

    }
    async Task<(IList<string>, IList<string>, Dictionary<string, string>)> CheckCategoryStateAsync(CancellationToken cancellationToken)
    {
        var categories = await _repository.TableNoTracking
        .ProjectTo<CategorySelectDto>(_mapper.ConfigurationProvider)
        .ToListAsync(cancellationToken);

        var dicError = new Dictionary<string, string>();
        var deactiveCategories = categories.Where(i => !(i?.State ?? false));
        var errorsCategories = categories.Where(i => !string.IsNullOrEmpty(i.ErrorMessage));

        List<string> catsErrors = new();
        List<string> catsMobile = new();
        foreach (var category in deactiveCategories)
        {
            string msg = $"کیوسک {category.Title} غیر فعال شد.";

            var log = await _logRepository.TableNoTracking
                .Where(c => c.CategoryId == category.Id && c.LogType == (int)CategoryLogType.InactiveCategoryLog)
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            var diff_now = DateTime.Now - category.LastRequest;

            if (log == null && category.Setting != null && diff_now.TotalHours >= category.Setting.Hour)
            {
                await addLog(category, cancellationToken);
                addToSmsDic(dicError, category, msg);
                continue;
            }
            
            if (log == null)
                continue;

            var diff = log.InsertTime - category.LastRequest;

            if (diff_now.TotalHours >= category.Setting.Hour && diff.TotalHours < 0)
            {
                await addLog(category, cancellationToken);
                addToSmsDic(dicError, category, msg);
            }

        }

        foreach (var category in errorsCategories)
        {
            string msg = $"خطای کیوسک {category.Title}:{category.ErrorMessage}";
            catsErrors.Add(msg);
            var phoneNumber = category.CategoryAgent?.PhoneNumber;
            catsMobile.Add(phoneNumber ?? "");
        }
        return (catsErrors, catsMobile, dicError);
    }

    private static void addToSmsDic(Dictionary<string, string> dicError, CategorySelectDto category, string msg)
    {
        var phoneNumber = category.CategoryAgent?.PhoneNumber;
        if (dicError.ContainsKey(phoneNumber ?? ""))
        {
            dicError[phoneNumber ?? ""] += msg + "\n";
        }
        else
        {
            dicError.Add(phoneNumber ?? "", msg + "\n");
        }
    }

    async Task addLog(CategorySelectDto category, CancellationToken cancellationToken)
    {
        string msg = $"کیوسک {category.Title} غیر فعال است.";
        var item = new CategoryLog();
        item.InsertTime = NativeDateTime.Now();
        item.CategoryId = category.Id;
        item.LogType = (int)CategoryLogType.InactiveCategoryLog;
        item.Message = msg;
        await _logRepository.AddAsync(item, cancellationToken);

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckingAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromHours(_settings.Value.SmsSettings.CheckInterval), stoppingToken);
        }
    }
}