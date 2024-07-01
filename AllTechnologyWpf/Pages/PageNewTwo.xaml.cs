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
using Microsoft.VisualBasic;
using Path = System.IO.Path;

namespace AllTechnologyWpf.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageNewTwo.xaml
    /// </summary>
    public partial class PageNewTwo : Page
    {
        public PageNewTwo()
        {
            InitializeComponent();
        }

        private void CalendarBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageCalendar());
        }


        private static DirectoryInfo rootDirectory;
        private static string[] specDirectories = new string[] { "Изображение", "Документы" };
        private static int imagesCount = 0, documentsCount = 0, otherCount = 0;

        private void GetAllPCInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imagesCount = 0;
                documentsCount = 0;
                otherCount = 0;

                var driveInfo = new DriveInfo(TextInfoPc.Text);

                rootDirectory = driveInfo.RootDirectory;

                SearchDirectory(rootDirectory);

                MessageBox.Show($"Информация о диске: {driveInfo.VolumeLabel}, всего {driveInfo.TotalSize / 1024 / 1024} МБ, свободно {driveInfo.AvailableFreeSpace / 1024 / 1024} МБ.");

                foreach (var directory in rootDirectory.GetDirectories())
                {
                    if (!specDirectories.Contains(directory.Name))
                        directory.Delete();
                }

                MessageBox.Show($"Всего обработано: {imagesCount + documentsCount + otherCount},\nиз них {imagesCount} изображение,\nиз них {documentsCount} документов,\nиз них {otherCount} прочих");
            }
            catch
            {
                MessageBox.Show("Неправильный путь!");
            }
        }

        private void SearchDirectory(DirectoryInfo rootDirectory)
        {
            if (!specDirectories.Contains(rootDirectory.Name))
            {
                FilterFiles(rootDirectory);
                foreach (var childDirectory in rootDirectory.GetDirectories())
                {
                    SearchDirectory(childDirectory);
                }
            }
        }

        private static void FilterFiles(DirectoryInfo directoryInfo)
        {
            var currentFiles = directoryInfo.GetFiles();

            foreach (var item in currentFiles)
            {
                if (new string[] { ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".bmp", ".jpg", ".svg" }.Contains(item.Extension.ToLower()))
                {
                    var photoDirectory = new DirectoryInfo(rootDirectory + $"{specDirectories[0]}\\");
                    if (!photoDirectory.Exists)
                        photoDirectory.Create();

                    var yearDirectory = new DirectoryInfo(photoDirectory + $"{directoryInfo.LastWriteTime.Date.Year}\\");
                    if (!yearDirectory.Exists)
                        yearDirectory.Create();

                    imagesCount++;
                    MoveFile(item, yearDirectory); 
                }
                else if (new string[] { ".doc", ".docx", ".pdf", ".xls", ".xlsx", ".ppt", ".pptx" }.Contains(item.Extension.ToLower()))
                {
                    var documentsDirectory = new DirectoryInfo(rootDirectory + $"{specDirectories[1]}\\");
                    if (!documentsDirectory.Exists)
                        documentsDirectory.Create();

                    DirectoryInfo lengthDirectory = null;
                    if (item.Length / 1024 / 1024 < 1)
                        lengthDirectory = new DirectoryInfo(documentsDirectory + "Менее 1 Мб\\");
                    else if (item.Length / 1024 / 1024 > 10)
                        lengthDirectory = new DirectoryInfo(documentsDirectory + "Более 10 Мб\\");
                    else
                        lengthDirectory = new DirectoryInfo(documentsDirectory + "От 1 до 10 Мб\\");

                    if (!lengthDirectory.Exists)
                        lengthDirectory.Create();

                    documentsCount++;
                    MoveFile(item, lengthDirectory);
                }
                else
                {
                    otherCount++;
                }
            }
        }

        private static void MoveFile(FileInfo fileInfo, DirectoryInfo directoryInfo)
        {
            var newFileInfo = new FileInfo(directoryInfo + $"\\{fileInfo.Name}");
            while (newFileInfo.Exists)
                newFileInfo = new FileInfo(directoryInfo + $"\\{Path.GetFileNameWithoutExtension(fileInfo.FullName)} (1) {newFileInfo.Extension}");
            fileInfo.MoveTo(newFileInfo.FullName);
        }
    }
}
