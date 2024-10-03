using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public enum PriceUnitType
    {
        [Display(Name ="ریال")]
        Rial,
        [Display(Name ="تومان")]
        Toman
    }
    public enum SortType
    {
        [Display(Name = "صعودی")]
        Asc,
        [Display(Name = "نزولی")]
        Desc
    }

    public enum CategoryLogType
    {
         posLog,
         InactiveCategoryLog
         
    }

}
