using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AllTechnologyWpf.Windows
{
    /// <summary>
    /// Логика взаимодействия для OknoTabChart.xaml
    /// </summary>
    public partial class OknoTabChart : Window
    {
        public OknoTabChart()
        {
            InitializeComponent();

            var series = new SeriesCollection();

            foreach (var item in App.DB.Grafik)
            {
                var n = item.Num - 147;

                series.Add(new HeatSeries
                {
                    Title = item.Num.ToString(),
                    Values = new ChartValues<double> { n / 4, n % 4, 345 }
                });
            };

            Cartes.Series = series;
        }
    }
}
