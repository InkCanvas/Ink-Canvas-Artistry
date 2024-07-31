using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;

namespace Ink_Canvas.Helpers
{
    public static class InkCanvasElementHelper
    {
        public static Point GetAllElementsBoundsCenterPoint(InkCanvas inkCanvas)
        {
            Rect bounds = inkCanvas.GetSelectionBounds();
            return new Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
        }
    }
}
