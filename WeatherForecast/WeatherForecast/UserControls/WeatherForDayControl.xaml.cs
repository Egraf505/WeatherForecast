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
    public partial class WeatherForDayControl : ContentView
    {        

        public WeatherForDayControl(string Date, ImageSource icon, int tempD, int tempN)
        {
            InitializeComponent();

            DateOfDayL.Text = Date;

            DayOfIconimage.Source = icon;

            DayOfTemp.Text = "+ " + tempD.ToString();

            NightOfTemp.Text = "+ " + tempN.ToString();
        }
    }
}