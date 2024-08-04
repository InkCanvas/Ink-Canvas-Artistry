using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ink_Canvas.Helpers;
using Microsoft.Win32;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        private async void BtnImageInsert_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                byte[] imageBytes = await Task.Run(() => File.ReadAllBytes(filePath));

                Image image = await CreateAndCompressImageAsync(imageBytes);

                if (image != null)
                {
                    string timestamp = "img_" + DateTime.Now.ToString("ddHHmmssfff");
                    image.Name = timestamp;

                    CenterAndScaleImage(image);

                    InkCanvas.SetLeft(image, 0);
                    InkCanvas.SetTop(image, 0);
                    inkCanvas.Children.Add(image);

                    timeMachine.CommitImageInsertHistory(image);
                }
            }
        }

        private void CenterAndScaleImage(Image image)
        {
            double maxWidth = SystemParameters.PrimaryScreenWidth / 2;
            double maxHeight = SystemParameters.PrimaryScreenHeight / 2;
            
            double scaleX = maxWidth / image.Width;
            double scaleY = maxHeight / image.Height;
            double scale = Math.Min(scaleX, scaleY);

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(scale, scale));

            double canvasWidth = inkCanvas.ActualWidth;
            double canvasHeight = inkCanvas.ActualHeight;
            double centerX = (canvasWidth - image.Width * scale) / 2;
            double centerY = (canvasHeight - image.Height * scale) / 2;

            transformGroup.Children.Add(new TranslateTransform(centerX, centerY));

            image.RenderTransform = transformGroup;
        }

        private async Task<Image> CreateAndCompressImageAsync(byte[] imageBytes)
        {
            return await Dispatcher.InvokeAsync(() =>
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }

                int width = bitmapImage.PixelWidth;
                int height = bitmapImage.PixelHeight;

                if (isLoaded && Settings.Canvas.IsCompressPicturesUploaded && (width > 1920 || height > 1080))
                {
                    double scaleX = 1920.0 / width;
                    double scaleY = 1080.0 / height;
                    double scale = Math.Min(scaleX, scaleY);

                    TransformedBitmap transformedBitmap = new TransformedBitmap(bitmapImage, new System.Windows.Media.ScaleTransform(scale, scale));

                    Image image = new Image();
                    image.Source = transformedBitmap;
                    image.Width = transformedBitmap.PixelWidth;
                    image.Height = transformedBitmap.PixelHeight;

                    return image;
                }
                else
                {
                    Image image = new Image();
                    image.Source = bitmapImage;
                    image.Width = width;
                    image.Height = height;

                    return image;
                }
            });
        }
    }
}
