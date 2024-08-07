using Ink_Canvas.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        #region Floating Control

        object lastBorderMouseDownObject;

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastBorderMouseDownObject = sender;
        }

        bool isStrokeSelectionCloneOn = false;

        private void BorderStrokeSelectionClone_Click(object sender, RoutedEventArgs e)
        {
            if (isStrokeSelectionCloneOn)
            {
                IconStrokeSelectionClone.SetResourceReference(TextBlock.ForegroundProperty, "FloatBarForeground");
                isStrokeSelectionCloneOn = false;
            }
            else
            {
                IconStrokeSelectionClone.SetResourceReference(TextBlock.ForegroundProperty, "FloatBarBackground");
                isStrokeSelectionCloneOn = true;
            }
        }

        private void BorderStrokeSelectionCloneToBoardOrNewPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentMode == 0)
            {
                StrokeCollection strokes = inkCanvas.GetSelectedStrokes();
                List<UIElement> elements = InkCanvasElementsHelper.GetSelectedElementsCloned(inkCanvas);
                inkCanvas.Select(new StrokeCollection());
                strokes = strokes.Clone();
                ImageBlackboard_Click(null, null);
                inkCanvas.Strokes.Add(strokes);
                InkCanvasElementsHelper.AddElements(inkCanvas, elements, timeMachine);
            }
            else
            {
                StrokeCollection strokes = inkCanvas.GetSelectedStrokes();
                List<UIElement> elements = InkCanvasElementsHelper.GetSelectedElementsCloned(inkCanvas);
                inkCanvas.Select(new StrokeCollection());
                strokes = strokes.Clone();
                BtnWhiteBoardAdd_Click(null, null);
                inkCanvas.Strokes.Add(strokes);
                InkCanvasElementsHelper.AddElements(inkCanvas, elements, timeMachine);
            }
        }

        private void GridPenWidthDecrease_Click(object sender, RoutedEventArgs e)
        {
            ChangeSelectedStrokeThickness(0.8);
        }

        private void GridPenWidthIncrease_Click(object sender, RoutedEventArgs e)
        {
            ChangeSelectedStrokeThickness(1.25);
        }

        private void GridPenWidthRestore_Click(object sender, RoutedEventArgs e)
        {
            foreach (Stroke stroke in inkCanvas.GetSelectedStrokes())
            {
                stroke.DrawingAttributes.Width = inkCanvas.DefaultDrawingAttributes.Width;
                stroke.DrawingAttributes.Height = inkCanvas.DefaultDrawingAttributes.Height;
            }
        }

        private void BorderStrokeSelectionDelete_Click(object sender, RoutedEventArgs e)
        {
            SymbolIconDelete_MouseUp(sender, e);
        }

        private void BtnStrokeSelectionSaveToImage_Click(object sender, RoutedEventArgs e)
        {
            StrokeCollection selectedStrokes = inkCanvas.GetSelectedStrokes();
            var selectedElements = inkCanvas.GetSelectedElements();

            if (selectedStrokes.Count > 0 || selectedElements.Count > 0)
            {
                Rect bounds = inkCanvas.GetSelectionBounds();

                double width = bounds.Width + 10;
                double height = bounds.Height + 10;
                RenderTargetBitmap renderTarget = new RenderTargetBitmap(
                    (int)Math.Ceiling(width), (int)Math.Ceiling(height),
                    96, 96, PixelFormats.Pbgra32);

                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.PushTransform(new TranslateTransform(-bounds.X, -bounds.Y));

                    foreach (Stroke stroke in selectedStrokes)
                    {
                        stroke.Draw(drawingContext);
                    }

                    foreach (UIElement element in selectedElements)
                    {
                        VisualBrush vb = new VisualBrush(element);
                        Rect elementBounds = new Rect(element.RenderSize);

                        Transform renderTransform = element.RenderTransform;
                        if (renderTransform != null)
                        {
                            drawingContext.PushTransform(renderTransform);
                            drawingContext.DrawRectangle(vb, null, elementBounds);
                            drawingContext.Pop();
                        }
                        else
                        {
                            drawingContext.DrawRectangle(vb, null, elementBounds);
                        }
                    }
                }

                renderTarget.Render(drawingVisual);

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PNG Images|*.png",
                    Title = "Save Selected Ink as PNG",
                    FileName = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-fff")
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                    using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }
            }
        }

        private void ChangeSelectedStrokeThickness(double multipler)
        {
            foreach (Stroke stroke in inkCanvas.GetSelectedStrokes())
            {
                var newWidth = stroke.DrawingAttributes.Width * multipler;
                var newHeight = stroke.DrawingAttributes.Height * multipler;
                if (newWidth >= DrawingAttributes.MinWidth && newWidth <= DrawingAttributes.MaxWidth
                    && newHeight >= DrawingAttributes.MinHeight && newHeight <= DrawingAttributes.MaxHeight)
                {
                    stroke.DrawingAttributes.Width = newWidth;
                    stroke.DrawingAttributes.Height = newHeight;
                }
            }
            if (DrawingAttributesHistory.Count > 0)
            {
                timeMachine.CommitStrokeDrawingAttributesHistory(DrawingAttributesHistory);
                DrawingAttributesHistory = new Dictionary<Stroke, Tuple<DrawingAttributes, DrawingAttributes>>();
                foreach (var item in DrawingAttributesHistoryFlag)
                {
                    item.Value.Clear();
                }
            }
        }

        private void MatrixTransform(int type)
        {
            Matrix m = new Matrix();
            Rect bounds = inkCanvas.GetSelectionBounds();
            Point center = new Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);

            switch (type)
            {
                case 1: // Flip Horizontal
                    m.ScaleAt(-1, 1, center.X, center.Y);
                    break;
                case 2: // Flip Vertical
                    m.ScaleAt(1, -1, center.X, center.Y);
                    break;
                default: // Rotate
                    m.RotateAt(type, center.X, center.Y);
                    break;
            }

            StrokeCollection targetStrokes = inkCanvas.GetSelectedStrokes();
            foreach (Stroke stroke in targetStrokes)
            {
                stroke.Transform(m, false);
            }

            List<UIElement> selectedElements = InkCanvasElementsHelper.GetSelectedElements(inkCanvas);
            foreach (UIElement element in selectedElements)
            {
                ApplyElementMatrixTransform(element, m);
            }

            if (DrawingAttributesHistory.Count > 0)
            {
                timeMachine.CommitStrokeDrawingAttributesHistory(DrawingAttributesHistory);
                DrawingAttributesHistory = new Dictionary<Stroke, Tuple<DrawingAttributes, DrawingAttributes>>();
                foreach (var item in DrawingAttributesHistoryFlag)
                {
                    item.Value.Clear();
                }
            }
            ToCommitStrokeManipulationHistoryAfterMouseUp();
        }

        private void ApplyElementMatrixTransform(UIElement element, Matrix matrix)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;
            TransformGroup transformGroup = frameworkElement.RenderTransform as TransformGroup;
            if (transformGroup == null)
            {
                transformGroup = new TransformGroup();
                frameworkElement.RenderTransform = transformGroup;
            }

            if (!ElementsInitialHistory.ContainsKey(frameworkElement.Name))
            {
                ElementsInitialHistory[frameworkElement.Name] = transformGroup.Clone();
            }

            TransformGroup centeredTransformGroup = new TransformGroup();
            centeredTransformGroup.Children.Add(new MatrixTransform(matrix));
            transformGroup.Children.Add(centeredTransformGroup);

            if (ElementsManipulationHistory == null)
            {
                ElementsManipulationHistory = new Dictionary<string, Tuple<object, TransformGroup>>();
            }
            ElementsManipulationHistory[frameworkElement.Name] =
                new Tuple<object, TransformGroup>(ElementsInitialHistory[frameworkElement.Name], transformGroup.Clone());
        }

        private void BtnFlipHorizontal_Click(object sender, RoutedEventArgs e)
        {
            MatrixTransform(1);
        }

        private void BtnFlipVertical_Click(object sender, RoutedEventArgs e)
        {
            MatrixTransform(2);
        }

        private void BtnAnticlockwiseRotate15_Click(object sender, RoutedEventArgs e)
        {
            MatrixTransform(-15);
        }

        private void BtnAnticlockwiseRotate45_Click(object sender, RoutedEventArgs e)
        {
            MatrixTransform(-45);
        }

        private void BtnAnticlockwiseRotate90_Click(object sender, RoutedEventArgs e)
        {
            MatrixTransform(-90);
        }

        private void BtnClockwiseRotate15_Click(object sender, RoutedEventArgs e)
        {
            MatrixTransform(15);
        }

        private void BtnClockwiseRotate45_Click(object sender, RoutedEventArgs e)
        {
            MatrixTransform(45);
        }

        private void BtnClockwiseRotate90_Click(object sender, RoutedEventArgs e)
        {
            MatrixTransform(90);
        }

        #endregion

        bool isGridInkCanvasSelectionCoverMouseDown = false;
        private Point lastMousePoint;

        private void GridInkCanvasSelectionCover_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastMousePoint = e.GetPosition(inkCanvas);
            isGridInkCanvasSelectionCoverMouseDown = true;
            if (isStrokeSelectionCloneOn)
            {
                _currentCommitType = CommitReason.CodeInput;
                StrokeCollection strokes = inkCanvas.GetSelectedStrokes();
                List<UIElement> elementList = InkCanvasElementsHelper.GetSelectedElements(inkCanvas);
                isProgramChangeStrokeSelection = true;
                ElementsSelectionClone = InkCanvasElementsHelper.CloneSelectedElements(inkCanvas, ref ElementsInitialHistory);
                inkCanvas.Select(new StrokeCollection());
                StrokesSelectionClone = strokes.Clone();
                inkCanvas.Strokes.Add(StrokesSelectionClone);
                inkCanvas.Select(strokes, elementList);
                isProgramChangeStrokeSelection = false;
                _currentCommitType = CommitReason.UserInput;
            }
            else if (lastMousePoint.X < inkCanvas.GetSelectionBounds().Left ||
            lastMousePoint.Y < inkCanvas.GetSelectionBounds().Top ||
            lastMousePoint.X > inkCanvas.GetSelectionBounds().Right ||
            lastMousePoint.Y > inkCanvas.GetSelectionBounds().Bottom)
            {
                isGridInkCanvasSelectionCoverMouseDown = false;
                inkCanvas.Select(new StrokeCollection());
                StrokesSelectionClone = new StrokeCollection();
                ElementsSelectionClone = new List<UIElement>();
            }
        }

        private void GridInkCanvasSelectionCover_MouseMove(object sender, MouseEventArgs e)
        {
            if (isGridInkCanvasSelectionCoverMouseDown == false) return;
            Point mousePoint = e.GetPosition(inkCanvas);
            Vector trans = new Vector(mousePoint.X - lastMousePoint.X, mousePoint.Y - lastMousePoint.Y);
            lastMousePoint = mousePoint;
            Matrix m = new Matrix();
            // add Translate
            m.Translate(trans.X, trans.Y);
            // handle UIElement
            List<UIElement> elements = new List<UIElement>();
            if (ElementsSelectionClone.Count != 0)
            {
                elements = ElementsSelectionClone;
            }
            else
            {
                elements = InkCanvasElementsHelper.GetSelectedElements(inkCanvas);
            }
            foreach (UIElement element in elements)
            {
                ApplyElementMatrixTransform(element, m);
            }
            // handle strokes
            StrokeCollection strokes = inkCanvas.GetSelectedStrokes();
            if (StrokesSelectionClone.Count != 0)
            {
                strokes = StrokesSelectionClone;
            }
            foreach (Stroke stroke in strokes)
            {
                stroke.Transform(m, false);
            }
            updateBorderStrokeSelectionControlLocation();
        }

        private void GridInkCanvasSelectionCover_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ToCommitStrokeManipulationHistoryAfterMouseUp();
            isGridInkCanvasSelectionCoverMouseDown = false;
            if (InkCanvasElementsHelper.IsNotCanvasElementSelected(inkCanvas))
            {
                GridInkCanvasSelectionCover.Visibility = Visibility.Collapsed;
                StrokesSelectionClone = new StrokeCollection();
                ElementsSelectionClone = new List<UIElement>();
            }
            else
            {
                if (currentMode == 0)
                {
                    TextSelectionCloneToNewBoard.Text = "衍至画板";
                }
                else
                {
                    TextSelectionCloneToNewBoard.Text = "衍至新页";
                }
                GridInkCanvasSelectionCover.Visibility = Visibility.Visible;
                StrokesSelectionClone = new StrokeCollection();
                ElementsSelectionClone = new List<UIElement>();
            }
        }

        private void GridInkCanvasSelectionCover_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scale = e.Delta > 0 ? 1.1 : 0.9;
            Point center = InkCanvasElementsHelper.GetAllElementsBoundsCenterPoint(inkCanvas);
            Matrix m = new Matrix();
            m.ScaleAt(scale, scale, center.X, center.Y);

            StrokeCollection strokes = inkCanvas.GetSelectedStrokes();
            List<UIElement> elements = InkCanvasElementsHelper.GetSelectedElements(inkCanvas);
            // handle UIElement
            foreach (UIElement element in elements)
            {
                ApplyElementMatrixTransform(element, m);
            }
            // handle strokes
            foreach (Stroke stroke in strokes)
            {
                stroke.Transform(m, false);
                try
                {
                    stroke.DrawingAttributes.Width *= scale;
                    stroke.DrawingAttributes.Height *= scale;
                }
                catch { }
            }
            updateBorderStrokeSelectionControlLocation();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            forceEraser = true;
            drawingShapeMode = 0;
            inkCanvas.IsManipulationEnabled = false;
            if (inkCanvas.EditingMode == InkCanvasEditingMode.Select)
            {
                if (inkCanvas.GetSelectedStrokes().Count == inkCanvas.Strokes.Count
                    && inkCanvas.GetSelectedElements().Count == inkCanvas.Children.Count)
                {
                    inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                    inkCanvas.EditingMode = InkCanvasEditingMode.Select;
                }
                else
                {
                    StrokeCollection selectedStrokes = new StrokeCollection();
                    foreach (Stroke stroke in inkCanvas.Strokes)
                    {
                        if (stroke.GetBounds().Width > 0 && stroke.GetBounds().Height > 0)
                        {
                            selectedStrokes.Add(stroke);
                        }
                    }
                    List<UIElement> selectedElements = InkCanvasElementsHelper.GetAllElements(inkCanvas);
                    inkCanvas.Select(selectedStrokes, selectedElements);
                }
            }
            else
            {
                inkCanvas.EditingMode = InkCanvasEditingMode.Select;
            }
        }
        bool isProgramChangeStrokeSelection = false;

        private void inkCanvas_SelectionChanged(object sender, EventArgs e)
        {
            if (isProgramChangeStrokeSelection) return;
            if (InkCanvasElementsHelper.IsNotCanvasElementSelected(inkCanvas))
            {
                GridInkCanvasSelectionCover.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (currentMode == 0)
                {
                    TextSelectionCloneToNewBoard.Text = "衍至画板";
                }
                else
                {
                    TextSelectionCloneToNewBoard.Text = "衍至新页";
                }
                GridInkCanvasSelectionCover.Visibility = Visibility.Visible;
                IconStrokeSelectionClone.SetResourceReference(TextBlock.ForegroundProperty, "FloatBarForeground");
                ToggleButtonStrokeSelectionClone.IsChecked = false;
                isStrokeSelectionCloneOn = false;
                updateBorderStrokeSelectionControlLocation();
            }
        }
        double BorderStrokeSelectionControlWidth = 695;
        double BorderStrokeSelectionControlHeight = 104;

        private void updateBorderStrokeSelectionControlLocation()
        {
            Rect selectionBounds = inkCanvas.GetSelectionBounds();
            double borderLeft = (selectionBounds.Left + selectionBounds.Right - BorderStrokeSelectionControlWidth) / 2;
            double borderTop = selectionBounds.Bottom + 15;

            // ensure the border is inside the window
            borderLeft = Math.Max(0, borderLeft);
            borderTop = Math.Max(0, borderTop);
            borderLeft = Math.Min(Width - BorderStrokeSelectionControlWidth, borderLeft);
            borderTop = Math.Min(Height - BorderStrokeSelectionControlHeight, borderTop);

            double borderBottom = borderTop + BorderStrokeSelectionControlHeight;
            double borderRight = borderLeft + BorderStrokeSelectionControlWidth;

            double viewboxTop = ViewboxFloatingBar.Margin.Top;
            double viewboxLeft = ViewboxFloatingBar.Margin.Left;
            double viewboxBottom = viewboxTop + ViewboxFloatingBar.ActualHeight;
            double viewboxRight = viewboxLeft + ViewboxFloatingBar.ActualWidth;

            if (currentMode == 0)
            {
                bool isHorizontalOverlap = (borderLeft < viewboxRight && borderRight > viewboxLeft);
                bool isVerticalOverlap = (borderTop < viewboxBottom && borderBottom > viewboxTop);
                if (isHorizontalOverlap && isVerticalOverlap)
                {
                    double belowViewboxMargin = viewboxBottom + 5;
                    double maxBottomPositionMargin = Height - BorderStrokeSelectionControlHeight;
                    borderTop = belowViewboxMargin > maxBottomPositionMargin
                        ? viewboxTop - BorderStrokeSelectionControlHeight - 5
                        : belowViewboxMargin;
                }
            }
            else
            {
                borderTop = Math.Min(Height - BorderStrokeSelectionControlHeight - 60, borderTop);
            }
            if (!double.IsNaN(borderLeft) && !double.IsNaN(borderTop))
            {
                BorderStrokeSelectionControl.Margin = new Thickness(borderLeft, borderTop, 0, 0);
            }
            else
            {
                BorderStrokeSelectionControl.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        private void GridInkCanvasSelectionCover_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.Mode = ManipulationModes.All;
        }

        private void GridInkCanvasSelectionCover_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (StrokeManipulationHistory?.Count > 0)
            {
                timeMachine.CommitStrokeManipulationHistory(StrokeManipulationHistory, ElementsManipulationHistory);
                foreach (var item in StrokeManipulationHistory)
                {
                    StrokeInitialHistory[item.Key] = item.Value.Item2;
                }
                StrokeManipulationHistory = null;
            }
            if (DrawingAttributesHistory.Count > 0)
            {
                timeMachine.CommitStrokeDrawingAttributesHistory(DrawingAttributesHistory);
                DrawingAttributesHistory = new Dictionary<Stroke, Tuple<DrawingAttributes, DrawingAttributes>>();
                foreach (var item in DrawingAttributesHistoryFlag)
                {
                    item.Value.Clear();
                }
            }
        }

        StrokeCollection StrokesSelectionClone = new StrokeCollection();
        List<UIElement> ElementsSelectionClone = new List<UIElement>();

        private void GridInkCanvasSelectionCover_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            try
            {
                if (dec.Count >= 1)
                {
                    ManipulationDelta md = e.DeltaManipulation;
                    Vector trans = md.Translation;
                    double rotate = md.Rotation;
                    Vector scale = md.Scale;
                    Point center = GetMatrixTransformCenterPoint(e.ManipulationOrigin, e.Source as FrameworkElement);
                    Matrix m = new Matrix();
                    // add Scale
                    m.ScaleAt(scale.X, scale.Y, center.X, center.Y);
                    StrokeCollection strokes = inkCanvas.GetSelectedStrokes();
                    if (StrokesSelectionClone.Count != 0)
                    {
                        strokes = StrokesSelectionClone;
                    }
                    else if (Settings.Gesture.IsEnableTwoFingerRotationOnSelection)
                    {
                        // add Rotate
                        m.RotateAt(rotate, center.X, center.Y);
                    }
                    // add Translate
                    m.Translate(trans.X, trans.Y);
                    List<UIElement> elements = new List<UIElement>();
                    if (ElementsSelectionClone.Count != 0)
                    {
                        elements = ElementsSelectionClone;
                    }
                    else
                    {
                        elements = InkCanvasElementsHelper.GetSelectedElements(inkCanvas);
                    }
                    // handle UIElements
                    foreach (UIElement element in elements)
                    {
                        ApplyElementMatrixTransform(element, m);
                    }
                    // handle strokes
                    foreach (Stroke stroke in strokes)
                    {
                        stroke.Transform(m, false);
                        try
                        {
                            stroke.DrawingAttributes.Width *= md.Scale.X;
                            stroke.DrawingAttributes.Height *= md.Scale.Y;
                        }
                        catch { }
                    }
                    updateBorderStrokeSelectionControlLocation();
                }
            }
            catch { }
        }

        Point lastTouchPointOnGridInkCanvasCover = new Point(0, 0);
        private void GridInkCanvasSelectionCover_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dec.Add(e.TouchDevice.Id);
            //设备1个的时候，记录中心点
            if (dec.Count == 1)
            {
                TouchPoint touchPoint = e.GetTouchPoint(null);
                centerPoint = touchPoint.Position;
                lastTouchPointOnGridInkCanvasCover = touchPoint.Position;

                if (isStrokeSelectionCloneOn)
                {
                    StrokeCollection strokes = inkCanvas.GetSelectedStrokes();
                    List<UIElement> elementsList = InkCanvasElementsHelper.GetSelectedElements(inkCanvas);
                    isProgramChangeStrokeSelection = true;
                    ElementsSelectionClone = InkCanvasElementsHelper.CloneSelectedElements(inkCanvas, ref ElementsInitialHistory);
                    inkCanvas.Select(new StrokeCollection());
                    StrokesSelectionClone = strokes.Clone();
                    inkCanvas.Strokes.Add(StrokesSelectionClone);
                    inkCanvas.Select(strokes, elementsList);
                    isProgramChangeStrokeSelection = false;
                }
            }
        }

        private void GridInkCanvasSelectionCover_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            dec.Remove(e.TouchDevice.Id);
            if (dec.Count >= 1) return;
            isProgramChangeStrokeSelection = false;
            if (lastTouchPointOnGridInkCanvasCover == e.GetTouchPoint(null).Position)
            {
                if (lastTouchPointOnGridInkCanvasCover.X < inkCanvas.GetSelectionBounds().Left ||
                    lastTouchPointOnGridInkCanvasCover.Y < inkCanvas.GetSelectionBounds().Top ||
                    lastTouchPointOnGridInkCanvasCover.X > inkCanvas.GetSelectionBounds().Right ||
                    lastTouchPointOnGridInkCanvasCover.Y > inkCanvas.GetSelectionBounds().Bottom)
                {
                    inkCanvas.Select(new StrokeCollection());
                    StrokesSelectionClone = new StrokeCollection();
                    ElementsSelectionClone = new List<UIElement>();
                }
            }
            else if (InkCanvasElementsHelper.IsNotCanvasElementSelected(inkCanvas))
            {
                GridInkCanvasSelectionCover.Visibility = Visibility.Collapsed;
                StrokesSelectionClone = new StrokeCollection();
                ElementsSelectionClone = new List<UIElement>();
            }
            else
            {
                if (currentMode == 0)
                {
                    TextSelectionCloneToNewBoard.Text = "衍至画板";
                }
                else
                {
                    TextSelectionCloneToNewBoard.Text = "衍至新页";
                }
                GridInkCanvasSelectionCover.Visibility = Visibility.Visible;
                StrokesSelectionClone = new StrokeCollection();
                ElementsSelectionClone = new List<UIElement>();
            }
        }
    }
}
