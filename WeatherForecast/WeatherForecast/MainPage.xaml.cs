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
using System.Windows.Input;

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

            RefreshForecast.IsRefreshing = true;

            RefreshForecast.Command = new Command(async () =>
            {
                await ContentLoading();
                RefreshForecast.IsRefreshing = false;
            });
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
            await ContentLoading();   
        }

        private async Task ContentLoading()
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

                RefreshForecast.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void ContentpPlacement(YandexAPI yandexAPI)
        {

            // Город и дата
            Forecast forecast = yandexAPI.forecasts.First();
            DateTime dateOfForecast = GetDateForString(forecast.date);
            CityL.Text = yandexAPI.geo_object.locality.name + " ";
            DateL.Text = dateOfForecast.ToString("dd") + " " + dateOfForecast.ToString("MMMM");

            // Иконка
            ImageSource imageSource = GetSVGIcon(yandexAPI.fact.icon);
            WeatherImage.Source = imageSource;           

            // Температура
            DegreesL.Text = "+ " + yandexAPI?.fact?.temp.ToString();

            // Температура ощущается
            WeatherLikeL.Text = "Ощущается как + " + yandexAPI.fact.feels_like.ToString();

            // Вывод почасовой погоды
            await GetHourForDay(yandexAPI, WeatherHoursStack.Children);

            // Вывод прогноза на 7 дней
            await GetForecastForDay(yandexAPI.forecasts, DayOfWeekStack.Children);
        }

        private Task GetHourForDay(YandexAPI yandexAPI, IList<View> children)
        {             
            if (children == null || yandexAPI == null)
                return Task.CompletedTask;

            children.Clear();

            Forecast forecast = yandexAPI.forecasts.First();

            List<Hour> hours = forecast.hours.Where(x => int.Parse(x.hour) >= DateTime.Now.Hour).ToList();

            foreach (var hour in hours)
            {                
                    children.Add(new WeatherForHourControl(hour.hour, GetSVGIcon(hour.icon), hour.temp));
            }

            if (children.Count < 24)
            {
                List<Hour> forecastNextHours = yandexAPI.forecasts.First(forec => forec.date_ts != forecast.date_ts).hours;

                foreach (var hour in forecastNextHours)
                {
                    if (children.Count == 24)                    
                        break;

                    children.Add(new WeatherForHourControl(hour.hour, GetSVGIcon(hour.icon), hour.temp));
                }
            }

            return Task.CompletedTask;
        }

        private DateTime GetDateForString(string date)
        {
            // Format dateStr 0000-00-00

            DateTime result;

            if(DateTime.TryParse(date,out result))
            {
                return result;
            }

            return DateTime.Now;
        }

        private Task GetForecastForDay(IList<Forecast> forecasts, IList<View> children)
        {
            if (forecasts == null || children == null)
                return Task.CompletedTask;

            children.Clear();

            foreach (var forecast in forecasts)
            {
                DateTime date = GetDateForString(forecast.date);
                children.Add(new WeatherForDayControl(date.ToString("dd") + " " + date.ToString("MMMM"), GetSVGIcon(forecast.parts.day_short.icon), forecast.parts.day_short.temp, forecast.parts.night_short.temp));
            }

            return Task.CompletedTask;
        }

        private ImageSource GetSVGIcon(string icon)
        {         
            return SvgImageSource.FromUri(new Uri($"https://yastatic.net/weather/i/icons/funky/dark/{icon}.svg")).ImageSource;
        }
    }
}
