using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Mahak.Api.ViewModels
{
    public class UploadImageViewModel
    {
        [Required]
        [Display(Name = "Image")]
        public IFormFile ItemPicture { get; set; }
    }
}
