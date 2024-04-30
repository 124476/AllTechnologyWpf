using AllTechnologyWpf.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
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
using System.Xml;
using System.Xml.Serialization;

namespace AllTechnologyWpf.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageMain.xaml
    /// </summary>
    public partial class PageMain : Page
    {
        TextBlock textBlockLocal;
        int contextRot;
        public PageMain()
        {
            InitializeComponent();
            DownBtn.Content = "<";
            contextRot = 0;
        }

        private void GotPhotoGrid_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog() { Filter = "*.jpeg; | *.jpeg;" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var renderBitmap = new RenderTargetBitmap((int)GridCopy.ActualWidth, (int)GridCopy.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                renderBitmap.Render(GridCopy);
                var jpegEncoder = new JpegBitmapEncoder();
                jpegEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                using (var fileStream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    jpegEncoder.Save(fileStream);
                }
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
            var dialog = new OpenFileDialog() { Filter = "*.png; | *.png;", Multiselect=true };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var files = dialog.FileNames;
                foreach(var file in files)
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
                CenterX = 25,
                CenterY = 25
            };
            Photo.RenderTransform = transform;
            ListUsers.ItemsSource = App.DB.User.ToList();
            var user = App.DB.User.FirstOrDefault();
            DataContext = user;
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

                var users = JsonConvert.SerializeObject(App.DB.User.Select(x => new
                {
                    x.Id,
                    x.Name,
                }).ToList());

                File.WriteAllText(dialog.FileName, users);
            }
        }
        private void GotXmlBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog() { Filter = "*.xml; | *.xml;" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var file = File.Create(dialog.FileName);

                var xmlSer = new XmlSerializer(App.DB.User.ToList().GetType());

                xmlSer.Serialize(file, App.DB.User.ToList());
                file.Close();
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
