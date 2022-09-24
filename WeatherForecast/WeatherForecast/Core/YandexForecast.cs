using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherForecast.Core.YandexApiCore;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Collections.Generic;

namespace WeatherForecast.Core
{
    internal class YandexForecast
    {
        readonly YandexAPI _yandexAPI;
        public YandexAPI YandexAPI { get { return _yandexAPI; } }

        Location _location;

        private const string _key = "26317ec5-5ae0-4eb1-87c3-da2e32439ef2";
        private const string _header = "X-Yandex-API-Key";
         

        public YandexForecast(Location location)
        {
            _location = location;

            _yandexAPI = Task.Run(() => GetYandexAPI()).Result;
        }

        private async Task<YandexAPI> GetYandexAPI()
        {
            HttpResponseMessage httpResponse;

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpRequestMessage httpRequest = new HttpRequestMessage())
                {

                    httpRequest.Method = HttpMethod.Get;

                    string url = "https://api.weather.yandex.ru/v2/forecast";
                    var builder = new UriBuilder(url);
                    builder.Query = $"lat="+_location.Latitude.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + "&lon=" + _location.Longitude.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + "&lang=ru_RU&limit=7&hours=true&extra=true";     

                    httpRequest.RequestUri = builder.Uri;                   

                    httpRequest.Headers.Add(_header, _key);

                    httpResponse = await httpClient.SendAsync(httpRequest);
                }
            }

            if (httpResponse != null && httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string json = await httpResponse.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<YandexAPI>(json);
            }           

            return null;
        }
    }
}
