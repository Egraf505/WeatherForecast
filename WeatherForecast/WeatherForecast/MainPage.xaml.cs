using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading;
using WeatherForecast.Core;
using WeatherForecast.Core.YandexApiCore;
using System.Net.Http;
using System.IO;

namespace WeatherForecast
{
    public partial class MainPage : ContentPage
    {
        CancellationTokenSource cts;

        YandexAPI _yandexAPI;

        Location _location;

        public MainPage()
        {
            InitializeComponent();            
        }


        async Task<Location> GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                Location location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    return location;
                }

                return await Geolocation.GetLastKnownLocationAsync(); ;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
                return await Geolocation.GetLastKnownLocationAsync(); ;
            }
        }

        protected override void OnDisappearing()
        {
            if(cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            base.OnDisappearing();
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            try
            {
                _location = await GetCurrentLocation();

                YandexForecast yandexForecast = new YandexForecast(_location);

                _yandexAPI = await yandexForecast.GetYandexAPI();

                if (_yandexAPI != null)
                {
                    ContentpPlacement(_yandexAPI);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка",ex.Message,"Ок");                
            }
            
        }

        private async void ContentpPlacement(YandexAPI yandexAPI)
        {
            CityL.Text = _yandexAPI.geo_object.locality.name;

            WeatherImage.Source = await GetSVGIcon(_yandexAPI.fact.icon);

            DegreesL.Text = "+ " + _yandexAPI?.fact?.temp.ToString();

            WeatherLikeL.Text = "Ощущается как + " + _yandexAPI.fact.feels_like.ToString();
        }

        private async Task<ImageSource> GetSVGIcon(string icon)
        {
            ConveterImageSourceForUrl conveter = new ConveterImageSourceForUrl();

            byte[] image = await conveter.GetByteArrayFromApi(icon);

            MemoryStream memoryStream = new MemoryStream(image);

            return ImageSource.FromStream(() => memoryStream);
        }
    }
}
