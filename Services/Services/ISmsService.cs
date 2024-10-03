using Common;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface ISmsService : IScopedDependency
    {
        Task<short> SendAsync(string message = "پیام تست از طرف محک",string phoneNumber="");
    }
}