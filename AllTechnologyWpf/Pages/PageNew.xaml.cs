using AllTechnologyWpf;
using AllTechnologyWpf.Models;
using AllTechnologyWpf.Windows;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using Brushes = System.Windows.Media.Brushes;

namespace AllTechnologyWpf.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageNew.xaml
    /// </summary>
    public partial class PageNew : Page
    {
        int startingTimer;
        DispatcherTimer timer;
        List<TreeViewItem> items;
        public PageNew()
        {
            InitializeComponent();
            var version = Assembly.GetEntryAssembly().GetName().Version;

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(2);

            VersionPkText.Text = version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString();
            startingTimer = 0;
            Refresh();
        }

        public class NewsItem
        {
            public string Title { get; set; }
            public string Link { get; set; }
            public string Description { get; set; }
            public DateTime PubDate { get; set; }
        }

        private void Refresh()
        {
            int x1 = 0;
            int y1 = 0;

            foreach (var item in App.DB.Grafik)
            {
                Line line = new Line();
                line.X1 = x1;
                line.X2 = x1 + 5;
                line.Y1 = -y1 + 100;
                line.Y2 = -item.Num + 100;
                line.Stroke = Brushes.Red;
                canvasDrive.Children.Add(line);
                x1 += 5;
                y1 = item.Num;
            }

            XDocument doc = XDocument.Load("https://westi-24.webnode.ru/rss/all.xml");
            var news = doc.Descendants("item").Select(x => new NewsItem
            {
                Title = (string)x.Element("title"),
                Link = (string)x.Element("link"),
                Description = (string)x.Element("description"),
                PubDate = DateTime.Parse((string)x.Element("pubDate"))
            }).ToList();
            DataNews.ItemsSource = news;

            var user = App.DB.User.FirstOrDefault();
            var lider = user.MaxUser;

            TreePanel.Items.Clear();
            items = new List<TreeViewItem>();

            TreeViewItem treeViewItem = new TreeViewItem();
            treeViewItem.Header = lider.FullName;
            TreePanel.Items.Add(treeViewItem);

            var users = App.DB.User.Where(x => x.LiderId == lider.Id).ToList();

            foreach (var item in users)
            {
                TreeViewItem treeViewItem1 = new TreeViewItem();
                treeViewItem1.Header = item.FullName;
                treeViewItem.Items.Add(treeViewItem1);
                items.Add(treeViewItem1);

                StartUser(item);
                items.Remove(treeViewItem1);
            }
        }

        private void StartUser(User lider)
        {
            var users = App.DB.User.Where(x => x.LiderId == lider.Id).ToList();

            foreach (var item in users)
            {
                TreeViewItem treeViewItem1 = new TreeViewItem();
                treeViewItem1.Header = item.FullName;
                items[items.Count - 1].Items.Add(treeViewItem1);
                items.Add(treeViewItem1);

                StartUser(item);
                items.Remove(treeViewItem1);
            }
        }

        private void SaveQr_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog() { Filter = "*.png; | *.png;" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var file = File.Create(dialog.FileName);

                var encoder = new JpegBitmapEncoder();
                BitmapSource bitmapSource = (BitmapSource)QrI.Source;
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(file);
                    file.Close();
                }
            }
        }

        private void SetQr_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog() { Filter = "*.png; | *.png;" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var imageBytes = File.ReadAllBytes(dialog.FileName);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = ms;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();

                    QrI.Source = image;
                    using (MemoryStream mss = new MemoryStream(imageBytes))
                    {
                        var decoder = new QRCodeDecoder();
                        Bitmap bitmap = new Bitmap(mss);
                        QrText.Text = decoder.Decode(new QRCodeBitmapImage(bitmap));
                    }
                }
            }
        }

        private void GotQr_Click(object sender, RoutedEventArgs e)
        {
            var encoder = new QRCodeEncoder();
            var bitmapImage = encoder.Encode(QrText.Text);

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Position = 0;
                bitmapImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();

                QrI.Source = image;
            }
        }

        private void StartTimer_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            startingTimer += 1;
            SetTimerText.Text = startingTimer.ToString();
            SystemSounds.Beep.Play();
        }

        private void OpenGrafik_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageGrafik());
        }

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void Vopros_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Ты любишь спать?", "Вопрос", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                Vopros.Background = Brushes.Green;
            else
                Vopros.Background = Brushes.Red;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OknoSend();
            dialog.ShowDialog();
        }
    }
}
