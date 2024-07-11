using Ink_Canvas.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Application = System.Windows.Application;
using System.Diagnostics;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        private void BoardChangeBackgroundColorBtn_MouseUp(object sender, RoutedEventArgs e)
        {
            if (!isLoaded) return;
            Settings.Canvas.UsingWhiteboard = !Settings.Canvas.UsingWhiteboard;
            SaveSettingsToFile();
            if (Settings.Canvas.UsingWhiteboard)
            {
                if (inkColor == 5) lastBoardInkColor = 0;
            }
            else
            {
                if (inkColor == 0) lastBoardInkColor = 5;
            }
            ComboBoxTheme_SelectionChanged(null, null);
            CheckColorTheme(true);
            if (BoardPen.Opacity == 1)
            {
                BoardPen.Background = (Brush)Application.Current.FindResource("BoardBarBackground");
            }
            if (BoardEraser.Opacity == 1)
            {
                BoardEraser.Background = (Brush)Application.Current.FindResource("BoardBarBackground");
            }
            if (BoardSelect.Opacity == 1)
            {
                BoardSelect.Background = (Brush)Application.Current.FindResource("BoardBarBackground");
            }
            if (BoardEraserByStrokes.Opacity == 1)
            {
                BoardEraserByStrokes.Background = (Brush)Application.Current.FindResource("BoardBarBackground");
            }
        }

        private void BoardEraserIcon_Click(object sender, RoutedEventArgs e)
        {
            if (BoardEraser.Opacity != 1)
            {
                AnimationsHelper.ShowWithSlideFromBottomAndFade(BoardDeleteIcon);
            }
            else
            {
                forceEraser = true;
                forcePointEraser = true;
                double k = 1;
                switch (Settings.Canvas.EraserSize)
                {
                    case 0:
                        k = 0.5;
                        break;
                    case 1:
                        k = 0.8;
                        break;
                    case 3:
                        k = 1.25;
                        break;
                    case 4:
                        k = 1.8;
                        break;
                }
                inkCanvas.EraserShape = new EllipseStylusShape(k * 90, k * 90);
                inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                drawingShapeMode = 0;

                inkCanvas_EditingModeChanged(inkCanvas, null);
                CancelSingleFingerDragMode();

                HideSubPanels("eraser");
            }
        }

        private void BoardEraserIconByStrokes_Click(object sender, RoutedEventArgs e)
        {
            if (BoardEraserByStrokes.Opacity != 1)
            {
                AnimationsHelper.ShowWithSlideFromBottomAndFade(BoardDeleteIcon);
            }
            else
            {
                forceEraser = true;
                forcePointEraser = false;

                inkCanvas.EraserShape = new EllipseStylusShape(5, 5);
                inkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
                drawingShapeMode = 0;

                inkCanvas_EditingModeChanged(inkCanvas, null);
                CancelSingleFingerDragMode();

                HideSubPanels("eraserByStrokes");
            }
        }

        private void BoardSymbolIconDelete_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PenIcon_Click(null, null);
            SymbolIconDelete_MouseUp(sender, e);
        }

        private void BoardLaunchEasiCamera_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ImageBlackboard_Click(null, null);
            SoftwareLauncher.LaunchEasiCamera("希沃视频展台");
        }

        private void BoardLaunchDesmos_MouseUp(object sender, MouseButtonEventArgs e)
        {
            HideSubPanelsImmediately();
            ImageBlackboard_Click(null, null);
            Process.Start("https://www.desmos.com/calculator?lang=zh-CN");
        }

    }
}