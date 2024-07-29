using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        private void BtnImageInsert_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(filePath, UriKind.Absolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                /*
                bitmapImage.DecodePixelWidth = 200;
                bitmapImage.DecodePixelHeight = 200;
                */
                bitmapImage.EndInit();

                Image image = new Image();
                image.Source = bitmapImage;
                image.Width = bitmapImage.PixelWidth;
                image.Height = bitmapImage.PixelHeight;

                string timestamp = "img_" + DateTime.Now.ToString("ddHHmmssfff");
                image.Name = timestamp;

                InkCanvas.SetLeft(image, 0);
                InkCanvas.SetTop(image, 0);
                inkCanvas.Children.Add(image);

                timeMachine.CommitImageInsertHistory(image);
            }
        }
    }
}