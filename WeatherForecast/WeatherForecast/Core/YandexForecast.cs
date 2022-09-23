using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherForecast.Core.YandexApiCore;
using Newtonsoft.Json;

namespace WeatherForecast.Core
{
    internal class YandexForecast
    {
        readonly YandexAPI _yandexAPI;
        public YandexAPI YandexAPI { get { return _yandexAPI; } }

        private const string _key = "26317ec5-5ae0-4eb1-87c3-da2e32439ef2";
        private const string _header = "X-Yandex-API-Key";

        private string _lat;
        private string _lang;

        public YandexForecast(string lat, string lang)
        {
            _lat = lat;
            _lang = lang;

            _yandexAPI = GetYandexAPI().Result;
        }

        private async Task<YandexAPI> GetYandexAPI()
        {
            HttpResponseMessage httpResponse;

            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage httpRequest = new HttpRequestMessage();
                httpRequest.Method = HttpMethod.Get;
                httpRequest.RequestUri = new Uri($@"https://api.weather.yandex.ru/v2/forecast?lat={_lat}&lon={_lang}&lang=ru_RU&limit=7&hours=true&extra=true");
                httpRequest.Headers.Add(_header, _key);

                httpResponse = await httpClient.SendAsync(httpRequest);
            }

            string json = await  httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<YandexAPI>(json);            
        }
    }
}
