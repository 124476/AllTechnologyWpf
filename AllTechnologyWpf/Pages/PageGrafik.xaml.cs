using AllTechnologyWpf.Models;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Office.Interop.Excel;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Page = System.Windows.Controls.Page;
using ChartArea = System.Windows.Forms.DataVisualization.Charting.ChartArea;
using Series = System.Windows.Forms.DataVisualization.Charting.Series;
using AllTechnologyWpf.Windows;

namespace AllTechnologyWpf.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageGrafik.xaml
    /// </summary>
    public partial class PageGrafik : Page
    {
        public PageGrafik()
        {
            InitializeComponent();

            ChartPayments.ChartAreas.Add(new ChartArea("Main"));

            var currentSeries = new Series("Payments")
            {
                IsValueShownAsLabel = true
            };
            ChartPayments.Series.Add(currentSeries);

            ComboGraficks.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
            ComboGraficks.SelectedIndex = 0;
            Refresh();
        }

        private void SetGrafikExcel_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog() { Filter = "*.xlsx; | *.xlsx;" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var grafiks = new List<Grafik>();

                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = excelApp.Workbooks.Open(dialog.FileName);
                Worksheet worksheet = workbook.Worksheets[1];
                Range objects = worksheet.UsedRange;

                int i = 2;
                while (objects[1][i].Value != null)
                {
                    grafiks.Add(new Grafik()
                    {
                        Num = int.Parse(objects[2][i].Value.ToString())
                    });
                    i++;
                }

                foreach (var item in App.DB.Grafik)
                {
                    App.DB.Grafik.Remove(item);
                }
                foreach(var item in grafiks)
                {
                    App.DB.Grafik.Add(item);
                }
                App.DB.SaveChanges();
            }

            Refresh();
        }

        private void Refresh()
        {
            var currentType = (SeriesChartType)ComboGraficks.SelectedItem;

            Series currentSeries = ChartPayments.Series.FirstOrDefault();
            currentSeries.ChartType = currentType;
            currentSeries.Points.Clear();

            foreach (var item in App.DB.Grafik)
            {
                currentSeries.Points.AddXY(item.Num, item.Id);
            };
        }

        private void GotGrafikExcel_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog() { Filter = "*.xlsx; | *.xlsx;" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                Worksheet worksheet = workbook.Worksheets[1];
                worksheet.Name = "Grafik";

                int i = 2;
                worksheet.Range["A1"].Value = "Id";
                worksheet.Range["B1"].Value = "Num";

                foreach (var item in App.DB.Grafik)
                {
                    worksheet.Range["A" + i.ToString()].Value = i - 1;
                    worksheet.Range["B" + i.ToString()].Value = item.Num;
                    i++;
                }
                workbook.SaveAs(dialog.FileName);
            }
        }

        private void ClosePage_Click(object sender, RoutedEventArgs e)
        {
            ColumnOne.Width = new GridLength(0);
            OpenPage.Visibility = Visibility.Visible;
        }

        private void OpenPage_Click(object sender, RoutedEventArgs e)
        {
            ColumnOne.Width = GridLength.Auto;
            OpenPage.Visibility = Visibility.Hidden;
        }

        private void ComboGraficks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void LiveCharts_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OknoLiveCharts();
            dialog.ShowDialog();
        }

        private void PieChart_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OknoPieChart();
            dialog.ShowDialog();
        }

        private void TabChart_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OknoTabChart();
            dialog.ShowDialog();
        }

        private void AnglChart_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OknoAnglChart();
            dialog.ShowDialog();
        }
    }
}
