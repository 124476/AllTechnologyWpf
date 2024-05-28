using AllTechnologyWpf.Models;
using AllTechnologyWpf.Models.SerialisibleModels;
using AllTechnologyWpf.Properties;
using AllTechnologyWpf.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using Color = System.Windows.Media.Color;
using Path = System.IO.Path;

namespace AllTechnologyWpf.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageMain.xaml
    /// </summary>
    public partial class PageMain : Page
    {
        TextBlock textBlockLocal;
        int contextRot;
        bool IsGray;
        public PageMain()
        {
            DataContext = App.DB.User.FirstOrDefault();
            InitializeComponent();
            DownBtn.Content = "<";
            contextRot = 0;
            if (Settings.Default.savedText != null)
            {
                SavedText.Text = Settings.Default.savedText;
            }
            else
            {
                SavedText.Text = "";
            }
        }

        private void GotPhotoGrid_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog() { Filter = "*.jpeg; | *.jpeg;" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var render = new RenderTargetBitmap((int)GridCopy.ActualWidth, (int)GridCopy.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                render.Render(GridCopy);
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(render));

                var file = File.Create(dialog.FileName);
                encoder.Save(file);
                file.Close();
                System.Diagnostics.Process.Start(dialog.FileName);
            }
        }

        private void DragOne_Drop(object sender, DragEventArgs e)
        {
            if (StackMain.Children.IndexOf(sender as TextBlock) != StackMain.Children.IndexOf(textBlockLocal))
            {
                int index = StackMain.Children.IndexOf(sender as TextBlock);
                StackMain.Children.Remove(textBlockLocal);
                StackMain.Children.Insert(index, textBlockLocal);
            }
        }

        private void DragOne_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                textBlockLocal = textBlock;
                DragDrop.DoDragDrop(this, "", DragDropEffects.Copy);
            }
        }

        private void GotNewPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog() { Filter = "*.png; | *.png;", Multiselect = true };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var files = dialog.FileNames;
                foreach (var file in files)
                {
                    User user = new User();
                    user.Photo = File.ReadAllBytes(file);
                    user.Name = file;
                    App.DB.User.Add(user);
                }
                App.DB.SaveChanges();
                Refresh();
            }
        }

        private void Refresh()
        {
            Transform transform = new RotateTransform(contextRot)
            {
                CenterX = 50,
                CenterY = 25
            };
            Photo.RenderTransform = transform;
            ListUsers.ItemsSource = App.DB.User.ToList();
            var user = App.DB.User.FirstOrDefault();

            using (MemoryStream ms = new MemoryStream(user.Photo))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();

                using (MemoryStream mss = new MemoryStream())
                {
                    if (IsGray)
                    {
                        var imageColor = new FormatConvertedBitmap();
                        imageColor.BeginInit();
                        imageColor.Source = bitmapImage;
                        imageColor.DestinationFormat = PixelFormats.Gray32Float;
                        imageColor.EndInit();
                        Photo.Source = imageColor;
                    }
                    else
                    {
                        Photo.Source = bitmapImage;
                    }
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void GotJsonBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog() { Filter = "*.json; | *.json;" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var file = File.Create(dialog.FileName);
                file.Close();

                var data = JsonConvert.SerializeObject(App.DB.User.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.FullName,
                    Lider = x.User2.FullName
                }));

                File.WriteAllText(dialog.FileName, data);
                System.Diagnostics.Process.Start(dialog.FileName);
            }
        }

        private void GotXmlBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog() { Filter = "*.xml; | *.xml;" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var file = File.Create(dialog.FileName);

                var users = App.DB.User.ToList().Select(x => new XMLUser
                {
                    Name = x.Name,
                    FullName = x.FullName,
                    LiderId = x.LiderId,
                    Id = x.Id,
                }).ToList();

                var xmlEncoder = new XmlSerializer(users.GetType());
                xmlEncoder.Serialize(file, users);
                file.Close();
                System.Diagnostics.Process.Start(dialog.FileName);
            }
        }

        private void RandomBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Random random = new Random();
            string text = "";
            text += (char)random.Next('A', 'Z' + 1);
            text += (char)random.Next('0', '9' + 1);
            text += (char)random.Next('0', '9' + 1);
            text += (char)random.Next('0', '9' + 1);
            text += (char)random.Next('A', 'Z' + 1);
            text += (char)random.Next('A', 'Z' + 1);
            RandomText.Text = text;
        }

        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            contextRot -= 45;
            Refresh();
        }

        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            contextRot += 45;
            Refresh();
        }

        private void GotNewColor_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OknoColors();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                PoiskHex.Text = App.color.Hex;
                SearchColor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(App.color.Hex));
            }
        }

        private void PoiskHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SearchColor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(PoiskHex.Text));
            }
            catch
            {

            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/124476/AllTechnologyWpf");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.savedText = SavedText.Text;
            Settings.Default.Save();
        }

        private void NewPage_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageNew());
        }

        private void GrayDid_Click(object sender, RoutedEventArgs e)
        {
            IsGray = !IsGray;
            if (IsGray)
            {
                GrayDid.Content = "Cl";
                GrayDid.Background = Brushes.Green;
            }
            else
            {
                GrayDid.Content = "Dr";
                GrayDid.Background = Brushes.Gray;
            }
            Refresh();
        }

        private void SaveForPhotos_Click(object sender, RoutedEventArgs e)
        {
            var dbDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Photos");
            if (!Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }

            dbDir += "/";

            int txtFile = 0;
            foreach (var item in App.DB.User)
            {
                var dialogFile = dbDir + txtFile + ".png";

                var file = File.Create(dialogFile);
                file.Close();

                File.WriteAllBytes(dialogFile, item.Photo);
                txtFile += 1;
            }

            System.Diagnostics.Process.Start(dbDir);
        }

        private void GotCsvBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog() { Filter = "*.csv; | *.csv;" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var file = File.Create(dialog.FileName);
                file.Close();

                var text = "Id;Name;FullName;LiderId";

                var users = App.DB.User.ToList();
                foreach (var user in users)
                {
                    text += $"\n{user.Id};{user.Name};{user.FullName};{user.LiderId}";
                }

                File.WriteAllText(dialog.FileName, text);
                System.Diagnostics.Process.Start(dialog.FileName);
            }
        }
    }
}
