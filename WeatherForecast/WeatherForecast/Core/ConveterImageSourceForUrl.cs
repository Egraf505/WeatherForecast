using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherForecast.Core
{
    public class ConveterImageSourceForUrl
    {
        public async Task<byte[]> GetByteArrayFromApi(string icon)
        {
            HttpResponseMessage httpResponse;

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Get;
                    request.RequestUri = new Uri($"https://yastatic.net/weather/i/icons/funky/dark/{icon}.svg");

                    httpResponse = await httpClient.SendAsync(request);
                }               
            }

            return await httpResponse.Content.ReadAsByteArrayAsync();
        }
    }
}
