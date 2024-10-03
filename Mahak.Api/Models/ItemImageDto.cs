using System;
using Entities;
using Microsoft.AspNetCore.Http;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class ItemImageDto : BaseDto<ItemImageDto, ItemImage, Guid>
    {
        public int ItemId { get; set; }
        public IFormFile ImageFile { get; set; }

    }
    public class ItemImageSelectDto : BaseDto<ItemImageSelectDto, ItemImage, Guid>
    {
        public string Name { get; set; }
        public string FileType { get; set; }
        public string PhysicalPath { get; set; }
        public string FileContent { get; set; }
        public int ItemId { get; set; }
    }
}
