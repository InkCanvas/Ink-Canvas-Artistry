using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;

namespace Ink_Canvas.Helpers
{
    public static class InkCanvasElementHelper
    {
        public static Rect GetAllElementsBounds(InkCanvas inkCanvas)
        {
            Rect totalBounds = Rect.Empty;
            foreach (Stroke stroke in inkCanvas.Strokes)
            {
                Rect strokeBounds = stroke.GetBounds();
                totalBounds.Union(strokeBounds);
            }
            foreach (UIElement child in inkCanvas.Children)
            {
                Rect childBounds = VisualTreeHelper.GetDescendantBounds(child);
                Point childPosition = child.TransformToAncestor(inkCanvas).Transform(new Point(0, 0));
                childBounds.Offset(childPosition.X, childPosition.Y);
                totalBounds.Union(childBounds);
            }
            return totalBounds;
        }
    }
}
