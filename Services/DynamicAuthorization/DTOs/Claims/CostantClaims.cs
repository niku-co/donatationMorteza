namespace Services.DynamicAuthorization.DTOs.Claims;

public static class KioskClaims
{
    public const string KioskList = nameof(KioskList);
    public const string CreateKiosk = nameof(CreateKiosk);
    public const string EditKiosk = nameof(EditKiosk);
    public const string DeleteKiosk = nameof(DeleteKiosk);
    public const string RestartKiosk = nameof(RestartKiosk);
}
public static class ServiceClaims
{
    public const string ServiceList = nameof(ServiceList);
    public const string CreateService = nameof(CreateService);
    public const string EditService = nameof(EditService);
    public const string DeleteService = nameof(DeleteService);
}
public static class AgentClaims
{
    public const string AgentList = nameof(AgentList);
    public const string DeleteAgent = nameof(DeleteAgent);
    public const string AgentEdit = nameof(AgentEdit);
}

public static class UserClaims
{
    public const string UserList = nameof(UserList);
    public const string CreateEditUser = nameof(CreateEditUser);
    public const string DeleteUser = nameof(DeleteUser);
}

public static class ReportClaims
{
    public const string Report = nameof(Report);
    public const string ReportLogs = nameof(ReportLogs);
}
public static class LanguageClaims
{
    public const string Language = nameof(Language);
}
public static class SmsServiceClaims
{
    //public const string SMSServiceList = nameof(SMSServiceList);
    public const string CreateEditSmsService = nameof(CreateEditSmsService);
}

public static class PrinterSettingClaims
{
    public const string EditPrinterSetting = nameof(EditPrinterSetting);
}

public static class ProvincegClaims
{
    public const string EditProvince = nameof(EditProvince);
    public const string ProvinceList = nameof(ProvinceList);
}

public static class DataMngClaims
{
    public const string EditDataMng = nameof(EditDataMng);
}
public static class TagClaims
{
    public const string CreateEditTages = nameof(CreateEditTages);
}

public static class ProvinceClaims
{
    public const string ProvinceList = nameof(ProvinceList);
}