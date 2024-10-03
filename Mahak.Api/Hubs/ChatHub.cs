using Common.Utilities;
using Data.Contracts;
using Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Mahak.Api.Hubs;

public class ChatHub : Hub
{
    private readonly IRepository<Category> _repository;
    private readonly IRepository<CategoryLog> _repositoryCatLog;

    public ChatHub(IRepository<Category> repository, IRepository<CategoryLog> repositoryCatLog)
    {
        _repository = repository;
        _repositoryCatLog = repositoryCatLog;
    }
    public async Task SendMessage(string deviceId)
    {
        await Clients.Group(deviceId).SendAsync("ReceiveMessage", deviceId);
    }

    public async Task OpenRustDesk(string deviceId)
    {
        await Clients.Group(deviceId).SendAsync("ReceiveOpenRustdesk", deviceId);
    }


    public async Task SendRestart(string deviceId)
    {
        await Clients.Group(deviceId).SendAsync("ReceiveRestart", deviceId);
    }

    public async Task GetMessageFromAndroid(string deviceId)
    {
        var model = await _repository.Entities.FirstOrDefaultAsync(i => i.DeviceId == deviceId);
        if (model == null)
        {
            //model = new Category()
            //{
            //    Title = Guid.NewGuid().ToString(),
            //    DeviceId = deviceId,
            //    ConnectionId = Context.ConnectionId,
            //    Setting = new Setting() { }
            //};
            //await _repository.AddAsync(model, default);
        }
        else if (model.ConnectionId is null)
        {
            model.ConnectionId = Context.ConnectionId;
            await _repository.UpdateAsync(model, default);
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
    }

    public async Task GetErrorMsgFromAndroid(string deviceId, string message)
    {
        var model = await _repository.Entities.FirstOrDefaultAsync(i => i.DeviceId == deviceId);
        if (model == null)
        {
            //model = new Category()
            //{
            //    Title = Guid.NewGuid().ToString(),
            //    DeviceId = deviceId,
            //    ConnectionId = Context.ConnectionId,
            //    Setting = new Setting() { }
            //};
            //await _repository.AddAsync(model, default);
        }
        else
        {
            var catLog = new CategoryLog();
            catLog.InsertTime = NativeDateTime.Now();
            catLog.Message = message;
            catLog.DeviceId = deviceId;
            catLog.CategoryId = model.Id;
            catLog.LogType = (int)CategoryLogType.posLog;

            //model.ConnectionId = Context.ConnectionId;
            await _repositoryCatLog.AddAsync(catLog, default);
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
    }
}
