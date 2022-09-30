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
using FFImageLoading.Svg.Forms;
using WeatherForecast.UserControls;

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

        private void ContentpPlacement(YandexAPI yandexAPI)
        {
            CityL.Text = yandexAPI.geo_object.locality.name;

            ImageSource imageSource = GetSVGIcon(yandexAPI.fact.icon);

            WeatherImage.Source = imageSource;           

            DegreesL.Text = "+ " + yandexAPI?.fact?.temp.ToString();

            WeatherLikeL.Text = "Ощущается как + " + yandexAPI.fact.feels_like.ToString();

            Forecast forecast = yandexAPI.forecasts.First();
           
            WeatherHoursStack.Children.Clear();

            foreach (var hour in forecast.hours)
            {               
                WeatherHoursStack.Children.Add(new WeatherForHourControl(hour.hour, GetSVGIcon(hour.icon), hour.temp));                
            }
        }

        private ImageSource GetSVGIcon(string icon)
        {         
            return SvgImageSource.FromUri(new Uri($"https://yastatic.net/weather/i/icons/funky/dark/{icon}.svg")).ImageSource;
        }
    }
}
