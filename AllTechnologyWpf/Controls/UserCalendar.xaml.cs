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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AllTechnologyWpf.Controls
{
    /// <summary>
    /// Логика взаимодействия для UserCalendar.xaml
    /// </summary>
    public partial class UserCalendar : UserControl
    {
        DateTime dateNow;
        public UserCalendar()
        {
            InitializeComponent();
            dateNow = DateTime.Now;
            Refresh();
        }

        private void Refresh()
        {
            DataGrid.Children.Clear();

            var dateStart = new DateTime(dateNow.Year, dateNow.Month, 1);
            var dateEnd = dateStart.AddMonths(1);
            int dat = (int)dateStart.DayOfWeek - 1;
            if (dat == -1)
                dat = 5;

            var date = new DateTime();

            for (int i = 0; i < 7; i++)
            {
                var textDate = new TextBlock();
                textDate.Text = date.AddDays(i).DayOfWeek.ToString().Substring(0, 2);
                DataGrid.Children.Add(textDate);
                Grid.SetColumn(textDate, i);
            }

            while (dateStart < dateEnd)
            {
                var textDate = new TextBlock();
                textDate.Text = dateStart.Day.ToString();
                textDate.HorizontalAlignment = HorizontalAlignment.Center;
                textDate.VerticalAlignment = VerticalAlignment.Center;

                Border border = new Border();
                border.CornerRadius = new CornerRadius(50);
                border.Height = 20;
                border.Width = 20;
                border.Child = textDate;

                StackPanel stackPanel = new StackPanel();
                stackPanel.Children.Add(border);

                var dateOfWeek = (int)dateStart.DayOfWeek;
                if (dateOfWeek == 0 || dateOfWeek == 6)
                    border.Background = Brushes.Red;

                if (DateTime.Now.Date == dateStart)
                {
                    var textNow = new TextBlock();
                    textNow.Text = "🤚";
                    textNow.HorizontalAlignment = HorizontalAlignment.Center;
                    stackPanel.Children.Add(textNow);
                }

                DataGrid.Children.Add(stackPanel);
                var dateN = dateStart.Day + dat - 1;
                Grid.SetColumn(stackPanel, dateN % 7);
                Grid.SetRow(stackPanel, dateN / 7 + 1);

                dateStart = dateStart.AddDays(1);
            }
            TextDate.Text = dateNow.ToString("MMMM yyyy");
        }

        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            dateNow = dateNow.AddMonths(-1);
            Refresh();
        }

        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            dateNow = dateNow.AddMonths(1);
            Refresh();
        }
    }
}
