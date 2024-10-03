namespace Mahak.Api.Models
{
    public class EmdadTokenInDto
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class EmdadTokenOutDto
    {
        public string access_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }

    }

    public class BulkData
    {
        public Data_Object data_object2 { get; set; }
        public string entity_code { get; set; }

        public string import_type_code { get; set; }
        public string count_of_records { get; set; }

    }
    public class Data_Object
    {
        public DataDetailItemDto[] data_file { get; set; }
    }

    public class EmdadBulkResponse
    {
        public int response_code { get; set; }
        public EmdadBulkResponseContent content { get; set; }
    }

    public class EmdadBulkResponseContent
    {
        public string trace_id { get; set; }
    }

    public class EmdadBulkTraceContentResult
    {
        public long create { get; set; }
        public long ignore { get; set; }
        public long specific_update { get; set; }
        public long ignore_proiority { get; set; }
        public long missed { get; set; }
    }
    public class EmdadBulkTraceContent
    {
        public long response_code { get; set; }
        public string start_date { get; set; }
        public string finish_date { get; set; }
        public int status { get; set; }
        public EmdadBulkTraceContentResult summary_result { get; set; }

    }

    public class EmdadBulkTrace
    {
        public long response_code { get; set; }
        public EmdadBulkTraceContent content { get; set; }

    }
}
