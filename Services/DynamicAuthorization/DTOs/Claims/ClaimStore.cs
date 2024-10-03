using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;

namespace Services.DynamicAuthorization.DTOs.Claims;

public static class ClaimStore
{
    public const string UserAccess = nameof(UserAccess);

    public static ImmutableHashSet<ClaimCollection> AllClaims =>
        ImmutableHashSet.CreateRange(new List<ClaimCollection>
        {
                new()
                {
                    Title="مدیریت کیوسک‌ها و تنظیمات",
                    ClaimInfoCollection=new List<ClaimInfo>
                    {
                        new()
                        {
                            Name = KioskClaims.KioskList,
                            DisplayName="مشاهده کیوسک ها"
                        },
                        new()
                        {
                            Name=KioskClaims.CreateKiosk,
                            DisplayName="ایجاد کیوسک"
                        },
                        new()
                        {
                            Name=KioskClaims.EditKiosk,
                            DisplayName="ویرایش کیوسک ها و تنظیمات"
                        },
                        new()
                        {
                            Name=KioskClaims.DeleteKiosk,
                            DisplayName="حذف کیوسک"
                        },
                        new()
                        {
                            Name=KioskClaims.RestartKiosk,
                            DisplayName="خاموش و روشن کردن کیوسک"
                        },
                    }
                },
                 new()
                {
                    Title="مدیریت کاربران",
                    ClaimInfoCollection=new List<ClaimInfo>
                    {
                        new()
                        {
                            Name=UserClaims.UserList,
                            DisplayName="مشاهده کاربران"
                        },
                        new()
                        {
                            Name=UserClaims.CreateEditUser,
                            DisplayName="ویرایش کاربران"
                        },
                        new()
                        {
                            Name=UserClaims.DeleteUser,
                            DisplayName="حذف کاربران"
                        }
                    }
                },
                new()
                {
                    Title="مدیریت خدمات",
                    ClaimInfoCollection=new List<ClaimInfo>
                    {
                        new()
                        {
                            Name=ServiceClaims.ServiceList,
                            DisplayName="مشاهده خدمات"
                        },
                        new()
                        {
                            Name=ServiceClaims.CreateService,
                            DisplayName="ایجاد خدمت"
                        },
                        new()
                        {
                            Name=ServiceClaims.EditService,
                            DisplayName="ویرایش خدمات"
                        },
                        new()
                        {
                            Name=ServiceClaims.DeleteService,
                            DisplayName="حذف خدمت"
                        }
                    }
                },
                new()
                {
                    Title="گزارش‌گیری",
                    ClaimInfoCollection=new List<ClaimInfo>
                    {
                        new()
                        {
                            Name=ReportClaims.Report,
                            DisplayName="مشاهده گزارش‌ها"
                        },
                    }
                },
                new()
                {
                    Title="گزارش‌ لاگ پوز",
                    ClaimInfoCollection=new List<ClaimInfo>
                    {
                        new()
                        {
                            Name=ReportClaims.ReportLogs,
                            DisplayName="مشاهده گزارش‌ لاگ پوز"
                        },
                    }
                },
                new()
                {
                    Title="تنظیمات پنل پیامکی",
                    ClaimInfoCollection=new List<ClaimInfo>
                    {

                        new()
                        {
                            Name=SmsServiceClaims.CreateEditSmsService,
                            DisplayName="ویرایش پنل"
                        },
                    }
                },
                 new()
                {
                    Title="تنظیمات چاپگر",
                    ClaimInfoCollection=new List<ClaimInfo>
                    {

                        new()
                        {
                            Name=PrinterSettingClaims.EditPrinterSetting,
                            DisplayName="ویرایش تنظیمات چاپگر"
                        },
                    }
                },
                  new()
                {
                    Title=" مدیریت داده",
                    ClaimInfoCollection=new List<ClaimInfo>
                    {

                        new()
                        {
                            Name=DataMngClaims.EditDataMng,
                            DisplayName="مدیریت داده"
                        },
                    }
                },
                   new()
                {
                    Title="  مدیریت تگ ها",
                    ClaimInfoCollection=new List<ClaimInfo>
                    {

                        new()
                        {
                            Name=TagClaims.CreateEditTages,
                            DisplayName="مدیریت تگ ها"
                        },
                    }
                },
                   new()
                {
                    Title="  مدیریت استان ها",
                    ClaimInfoCollection=new List<ClaimInfo>
                    {

                        new()
                        {
                            Name=ProvinceClaims.ProvinceList,
                            DisplayName="مدیریت استان ها"
                        },
                    }
                },
                   new()
                {
                    Title=" مدیریت عامل ها",
                    ClaimInfoCollection=new List<ClaimInfo>
                    {

                        new()
                        {
                            Name=AgentClaims.AgentList,
                            DisplayName="لیست عامل"
                        },
                        new()
                        {
                            Name=AgentClaims.DeleteAgent,
                            DisplayName="حذف عامل"
                        },
                        new()
                        {
                            Name=AgentClaims.AgentEdit,
                            DisplayName="ویرایش عامل"
                        },
                    }
                },

        });

    public static IEnumerable<ClaimCollection> GetClaimCollections(IEnumerable<Claim> userClaims)
    {
        var claims = AllClaims.ToList();
        var claimValues = userClaims.Select(x => x.Value).ToList();
        claims.SelectMany(x => x.ClaimInfoCollection).Where(x => claimValues.Contains(x.Name)).ToList().ForEach(x => x.Selected = true);
        return claims;
    }

}
