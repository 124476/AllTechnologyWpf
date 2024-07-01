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
    /// Логика взаимодействия для OknoAnglChart.xaml
    /// </summary>
    public partial class OknoAnglChart : Window
    {
        public OknoAnglChart()
        {
            InitializeComponent();
            var series = new SeriesCollection();
            
            foreach (var item in App.DB.Grafik)
            {
                series.Add(new ColumnSeries
                {
                    Title = item.Id.ToString(),
                    Values = new ChartValues<double>() { item.Num }
                });
            }

            Cartes.Series = series;
        }
    }
}
