using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading;

namespace WeatherForecast
{
    public partial class MainPage : ContentPage
    {
        CancellationTokenSource cts;
        public MainPage()
        {
            InitializeComponent();

            GetCurrentLocation();
        }

        async Task GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    this.Content = new Label() { Text = location.Latitude + " " + location.Longitude };
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "Ok");              
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
