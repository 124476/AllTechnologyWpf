using LiveCharts;
using LiveCharts.Wpf;
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
    /// Логика взаимодействия для OknoLiveCharts.xaml
    /// </summary>
    public partial class OknoLiveCharts : Window
    {
        public OknoLiveCharts()
        {
            InitializeComponent();

            ChartValues<double> doubles = new ChartValues<double>();
            foreach (var item in App.DB.Grafik)
            {
                doubles.Add(item.Num);
            }

            var series = new SeriesCollection()
            {
                new LineSeries
                {
                    Values = doubles,
                    Fill = null
                }
            };

            Cartes.Series = series;
        }
    }
}
