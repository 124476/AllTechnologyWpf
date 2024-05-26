using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace AllTechnologyWpf.Windows
{
    /// <summary>
    /// Логика взаимодействия для OknoSend.xaml
    /// </summary>
    public partial class OknoSend : Window
    {
        byte[] data;
        string filename;

        public OknoSend()
        {
            InitializeComponent();
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MailMessage message = new MailMessage(App.EmailName, EmailTo.Text, "All texnology", "You got it!");

                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(App.EmailName, App.EmailPassword),
                    EnableSsl = true
                };

                try
                {
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        var data = new Attachment(ms, filename);
                        message.Attachments.Add(data);
                        client.Send(message);
                    }
                }
                catch
                {
                    client.Send(message);
                }

                MessageBox.Show("Отправлено!");
            }
            catch
            {
                MessageBox.Show("Ошибка отправки!");
            }
        }

        private void SetFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                data = File.ReadAllBytes(dialog.FileName);
                var list = dialog.FileName.Split('\\');
                filename = list[list.Length - 1];
                FileDoc.Text = filename;
            }
        }

        private void GetFile_Click(object sender, RoutedEventArgs e)
        {
            if (data == null || filename == null)
            {
                MessageBox.Show("Не указан файл!");
                return;
            }

            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var filePath = dialog.SelectedPath + '\\' + filename;
                var file = File.Create(filePath);
                file.Close();

                File.WriteAllBytes(filePath, data);
            }
        }
    }
}
