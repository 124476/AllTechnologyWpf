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
    /// Логика взаимодействия для OknoColors.xaml
    /// </summary>
    public partial class OknoColors : Window
    {
        public OknoColors()
        {
            InitializeComponent();
            ListColors.ItemsSource = App.DB.Color.ToList();
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Models.Color color = (sender as TextBlock).DataContext as Models.Color;
            if (color != null)
            {
                App.color = color;
                DialogResult = true;
            }
        }
    }
}
