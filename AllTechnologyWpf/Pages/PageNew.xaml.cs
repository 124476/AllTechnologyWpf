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
using Brushes = System.Windows.Media.Brushes;

namespace AllTechnologyWpf.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageNew.xaml
    /// </summary>
    public partial class PageNew : Page
    {
        int startingTimer;
        public PageNew()
        {
            InitializeComponent();
            var version = Assembly.GetEntryAssembly().GetName().Version;

            VersionPkText.Text = version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString();
            startingTimer = 0;
            Refresh();
        }

        private void Refresh()
        {
            int x1 = 0;
            int y1 = 0;

            foreach(var item in App.DB.Grafik)
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
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(5);
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
    }
}
