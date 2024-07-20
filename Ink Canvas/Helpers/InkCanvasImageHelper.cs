using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ink_Canvas.Helpers
{
    public static class InkCanvasImageHelper
    {
        public static List<Image> GetSelectedImages(InkCanvas inkCanvas)
        {
            List<Image> selectedImages = new List<Image>();
            foreach (UIElement element in inkCanvas.GetSelectedElements())
            {
                if (element is Image selectedImage)
                {
                    selectedImages.Add(selectedImage);
                }
            }
            return selectedImages;
        }

        private static Image CloneImage(Image originalImage)
        {
            Image clonedImage = new Image
            {
                Source = originalImage.Source,
                Width = originalImage.Width,
                Height = originalImage.Height,
                Stretch = originalImage.Stretch,
                Opacity = originalImage.Opacity,
                RenderTransform = originalImage.RenderTransform.Clone()
            };
            return clonedImage;
        }

        public static List<Image> CloneSelectedImages(InkCanvas inkCanvas)
        {
            List<Image> clonedImages = new List<Image>();
            foreach (UIElement element in inkCanvas.GetSelectedElements())
            {
                if (element is Image originalImage)
                {
                    Image clonedImage = CloneImage(originalImage);
                    InkCanvas.SetLeft(clonedImage, InkCanvas.GetLeft(originalImage));
                    InkCanvas.SetTop(clonedImage, InkCanvas.GetTop(originalImage));
                    inkCanvas.Children.Add(clonedImage);
                    clonedImages.Add(clonedImage);
                }
            }
            return clonedImages;
        }
    }
}
