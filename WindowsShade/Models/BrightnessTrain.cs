using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WindowsShade.Models
{
    public class BrightnessTrain
    {
        private HttpClient _httpClient = new HttpClient();

        public async Task<bool> HealthCheck()
        {
            var uri = new Uri($"{Common.Config.ServerUrl}/api/Brightness/HealthCheck");
            try
            {
                return await this._httpClient.GetStringAsync(uri) == "Ok";
            }
            catch { }
            return false;
        }

        public async Task<List<BrightnessData>> GetBrightnessData(IEnumerable<BrightnessData> src)
        {
            var uri = new Uri($"{Common.Config.ServerUrl}/api/Brightness/Items");
            var brightnessContent = new StringContent(JsonConvert.SerializeObject(src), System.Text.Encoding.UTF8, "application/json");

            var d = null as List<BrightnessData>;
            try
            {
                var r = await this._httpClient.PostAsync(uri, brightnessContent);

                if (r.IsSuccessStatusCode)
                {
                    var r1 = r.Content.ReadAsStringAsync().Result;
                    d = JsonConvert.DeserializeObject<List<BrightnessData>>(r1);
                }
            }
            catch { }

            return d;
        }
    }
}
