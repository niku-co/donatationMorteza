using AutoMapper;
using Entities;
using WebFramework.Api;
using Common.Utilities;

namespace Mahak.Api.Models
{
    public class CategoryLogDto : BaseDto<CategoryLogDto, CategoryLog, Guid>
    {
        public string DeviceId { get; set; }
        public int CategoryId { get; set; }
        public string Message { get; set; }
        public DateTime InsertTime { get; set; }
        public string PerDateTime { get; set; }
        public int LogType { get; set; }

        public CategoryDto Category { get; set; }
        public override void CustomMappings(IMappingExpression<CategoryLog, CategoryLogDto> mapping)
        {
            mapping.ForMember(
                dest => dest.PerDateTime,
                config => config.MapFrom(i => i.InsertTime.ToPersianDateStringFormat()));
        }
    }

    public class CategoryLogPaginationDto : BaseDto<CategoryLogPaginationDto, CategoryLog, Guid>
    {
        public ICollection<CategoryLogDto> Items { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }


}
