using AllTechnologyWpf.Models;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Office.Interop.Excel;
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
            ChartValues<double> doubles = new ChartValues<double>();
            foreach (var item in App.DB.Grafik)
            {
                doubles.Add(item.Num);
            }

            var seriaes = new LiveCharts.SeriesCollection()
            {
                new LineSeries
                {
                    Title = "Grafik",
                    Values = doubles,
                    Fill = null
                }
            };

            Cartes.Series = seriaes;
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
    }
}
