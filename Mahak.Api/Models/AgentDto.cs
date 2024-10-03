using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class AgentDto : BaseDto<AgentDto, CategoryAgent, Guid>
    {
        public string FullName { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}
