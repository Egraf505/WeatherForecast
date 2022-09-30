using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherForecast.UserControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeatherForHourControl : ContentView
    {
        readonly string hour;
        readonly ImageSource imageSource;
        readonly int temp;

        public WeatherForHourControl(string hour, ImageSource imageSource, int temp)
        {
            InitializeComponent();

            this.hour = hour;
            this.imageSource = imageSource;
            this.temp = temp;

            SetToValue();
        }

        private void SetToValue()
        {
            HourTimeL.Text = hour + ":00";
            HourImage.Source = imageSource;
            HourTempL.Text = " + " + temp.ToString();
        }
    }
}