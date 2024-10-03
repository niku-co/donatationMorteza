using Entities;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Mahak.Api.Models.DataMng
{
    public class EmdadService
    {
        private readonly DataMNGSetting _setting;
        public EmdadService(DataMNGSetting setting)
        {
            _setting=setting;
        }
        public async Task<EmdadBulkResponse> SendAsync(List<DataDetailItemDto> listData)
        {
            
            if (_setting == null)
                throw new Exception("setting not found.");

            var token = await getTokenAsync(_setting);
            if (string.IsNullOrEmpty(token)) throw new Exception("token not found.");

            var _client = new HttpClient();
            _client.BaseAddress = new Uri("https://apim.emdad.ir:8243");

            string url = "/api/mdm/import_data/bulk_import_data";

            var bulkData = new BulkData();
            bulkData.data_object2 = new Data_Object()
            {
                data_file = listData.ToArray(),

            };
            bulkData.count_of_records = listData.Count.ToString();
            bulkData.import_type_code = "0";
            bulkData.entity_code = "62";

            var jsonParam = JsonConvert.SerializeObject(bulkData);
            HttpContent inContent = new StringContent(jsonParam, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsync(url, inContent);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var postResult = JsonConvert.DeserializeObject<EmdadBulkResponse>(content);

                return postResult;
            }
            else
            {
                throw new Exception("sending bulk data failed.");
            }
        }

        public async Task<EmdadBulkTrace> TraceAsync(string traceId)
        {

            if (_setting == null)
                throw new Exception("setting not found.");

            var token = await getTokenAsync(_setting);
            if (string.IsNullOrEmpty(token)) throw new Exception("token not found.");

            var _client = new HttpClient();
            _client.BaseAddress = new Uri("https://apim.emdad.ir:8243");

            string url = "/api/mdm/import_data /trace_bulk_import_data?trace_id="+ traceId;
                        
           
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var postResult = JsonConvert.DeserializeObject<EmdadBulkTrace>(content);

                return postResult;
            }
            else
            {
                throw new Exception("trace bulk data failed.");
            }
        }

        private async Task<string> getTokenAsync(DataMNGSetting setting)
        {
            var _client = new HttpClient();
            _client.BaseAddress = new Uri("https://apim.emdad.ir:9443");

            string url = "/oauth2/token";
            var item = new EmdadTokenInDto();
            item.grant_type = "password";
            item.username = setting.Username;
            item.password = setting.Password;

            var jsonParam = JsonConvert.SerializeObject(item);
            HttpContent inContent = new StringContent(jsonParam, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(url, inContent);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var postResult = JsonConvert.DeserializeObject<EmdadTokenOutDto>(content);
                if (!string.IsNullOrEmpty(postResult.access_token))
                {
                    return postResult.access_token;
                }
            }
            else
            {
                return "";
            }

            return "";
        }



    }
}
