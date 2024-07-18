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

        public static int GetSelectedImageCount(InkCanvas inkCanvas)
        {
            int count = 0;
            foreach (UIElement element in inkCanvas.GetSelectedElements())
            {
                if (element is Image selectedImage)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
