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

namespace WeatherForecast
{
    public partial class MainPage : ContentPage
    {
        CancellationTokenSource cts;

        YandexForecast _yandexForecast;

        Location _location;

        public MainPage()
        {            
            InitializeComponent();

            _location = Task.Run(() => GetCurrentLocation()).Result;

            _yandexForecast = new YandexForecast(_location);

            if (_yandexForecast != null)
            {
                DegreesL.Text = _yandexForecast?.YandexAPI?.fact?.temp.ToString();
            }
            
        }


        async Task<Location> GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    return location;
                }

                return null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
                return null;
            }
        }

        protected override void OnDisappearing()
        {
            if(cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            base.OnDisappearing();
        }
    }
}
