using Ink_Canvas.Helpers;
using System.Windows;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        private Point GetMatrixTransformCenterPoint(Point gestureOperationCenterPoint, FrameworkElement fe)
        {
            Point canvasCenterPoint = new Point(fe.ActualWidth / 2, fe.ActualHeight / 2);
            if (!isLoaded) return canvasCenterPoint;
            if (Settings.Gesture.MatrixTransformCenterPoint == MatrixTransformCenterPointOptions.CanvasCenterPoint)
            {
                return canvasCenterPoint;
            }
            else if (Settings.Gesture.MatrixTransformCenterPoint == MatrixTransformCenterPointOptions.GestureOperationCenterPoint)
            {
                return gestureOperationCenterPoint;
            }
            else if (Settings.Gesture.MatrixTransformCenterPoint == MatrixTransformCenterPointOptions.SelectedElementsCenterPoint)
            {
                return InkCanvasElementHelper.GetAllElementsBoundsCenterPoint(inkCanvas);
            }
            return canvasCenterPoint;
        }
    }
}
