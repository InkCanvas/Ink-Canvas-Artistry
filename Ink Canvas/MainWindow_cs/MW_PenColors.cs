using Ink_Canvas.Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        int inkColor = 1;

        private void InkWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isLoaded) return;
            if (sender == BoardInkWidthSlider) InkWidthSlider.Value = ((Slider)sender).Value;
            if (sender == InkWidthSlider) BoardInkWidthSlider.Value = ((Slider)sender).Value;
            Settings.Canvas.InkWidth = ((Slider)sender).Value / 2;
            if (inkColor > 100)
            {
                drawingAttributes.Height = 30 + ((Slider)sender).Value;
                drawingAttributes.Width = 30 + ((Slider)sender).Value;
            }
            else
            {
                drawingAttributes.Height = ((Slider)sender).Value / 2;
                drawingAttributes.Width = ((Slider)sender).Value / 2;
            }
            SaveSettingsToFile();
        }

        private void ColorSwitchCheck()
        {
            forceEraser = false;
            HideSubPanels("color");
            if (Main_Grid.Background == Brushes.Transparent)
            {
                if (currentMode == 1)
                {
                    currentMode = 0;
                    GridBackgroundCover.Visibility = Visibility.Collapsed;
                    AnimationsHelper.HideWithSlideAndFade(BlackboardLeftSide);
                    AnimationsHelper.HideWithSlideAndFade(BlackboardCenterSide);
                    AnimationsHelper.HideWithSlideAndFade(BlackboardRightSide);
                }
                BtnHideInkCanvas_Click(null, null);
            }

            StrokeCollection strokes = inkCanvas.GetSelectedStrokes();
            if (strokes.Count != 0)
            {
                foreach (Stroke stroke in strokes)
                {
                    try
                    {
                        stroke.DrawingAttributes.Color = inkCanvas.DefaultDrawingAttributes.Color;
                    }
                    catch { }
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
            else
            {
                inkCanvas.IsManipulationEnabled = true;
                drawingShapeMode = 0;
                inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                CancelSingleFingerDragMode();
                forceEraser = false;
                CheckColorTheme();
            }

            isLongPressSelected = false;
        }

        bool isUselightThemeColor = false, isDesktopUselightThemeColor = false;
        int lastDesktopInkColor = 1, lastBoardInkColor = 5;
        Dictionary<int, Color> inkColorLightThemeMapping = new Dictionary<int, Color>
        {
            { 0, Colors.Black }, // Black
            { 1, Color.FromRgb(239, 68, 68) }, // Red
            { 2, Color.FromRgb(34, 197, 94) }, // Green
            { 3, Color.FromRgb(59, 130, 246) }, // Blue
            { 4, Color.FromRgb(250, 204, 21) }, // Yellow
            { 5, Colors.White }, // White
            { 6, Color.FromRgb(236, 72, 153) }, // Pink
            { 7, Color.FromRgb(20, 184, 166) }, // Teal
            { 8, Color.FromRgb(249, 115, 22) }, // Orange
        };
        Dictionary<int, Color> inkColorDarkThemeMapping = new Dictionary<int, Color>
        {
            { 0, Colors.Black }, // Black
            { 1, Color.FromRgb(220, 38, 38) }, // Red
            { 2, Color.FromRgb(22, 163, 74) }, // Green
            { 3, Color.FromRgb(37, 99, 235) }, // Blue
            { 4, Color.FromRgb(234, 179, 8) }, // Yellow
            { 5, Colors.White }, // White
            { 6, Color.FromRgb(147, 51, 234) }, // Pink (Purple)
            { 7, Color.FromRgb(13, 148, 136) }, // Teal
            { 8, Color.FromRgb(234, 88, 12) }  // Orange
        };

        private void CheckColorTheme(bool changeColorTheme = false)
        {
            if (changeColorTheme)
            {
                if (currentMode != 0)
                {
                    if (Settings.Canvas.UsingWhiteboard)
                    {
                        GridBackgroundCover.Background = new SolidColorBrush(StringToColor("#FFF2F2F2"));
                        isUselightThemeColor = false;
                    }
                    else
                    {
                        GridBackgroundCover.Background = new SolidColorBrush(StringToColor("#FF1F1F1F"));
                        isUselightThemeColor = true;
                    }
                }
            }

            if (currentMode == 0)
            {
                isUselightThemeColor = isDesktopUselightThemeColor;
                inkColor = lastDesktopInkColor;
            }
            else
            {
                inkColor = lastBoardInkColor;
            }

            if (inkColor == 101)
            { // Highlighter Red
                inkCanvas.DefaultDrawingAttributes.Color = Color.FromRgb(220, 38, 38);
            }
            else if (inkColor == 102)
            { // Highlighter Orange
                inkCanvas.DefaultDrawingAttributes.Color = Color.FromRgb(234, 88, 12);
            }
            else if (inkColor == 103)
            { // Highlighter Yellow
                inkCanvas.DefaultDrawingAttributes.Color = Color.FromRgb(234, 179, 8);
            }
            else if (inkColor == 104)
            { // Highlighter Teal
                inkCanvas.DefaultDrawingAttributes.Color = Color.FromRgb(13, 148, 136);
            }
            else if (inkColor == 105)
            { // Highlighter Blue
                inkCanvas.DefaultDrawingAttributes.Color = Color.FromRgb(37, 99, 235);
            }
            else if (inkColor == 106)
            { // Highlighter Purple
                inkCanvas.DefaultDrawingAttributes.Color = Color.FromRgb(147, 51, 234);
            }
            else if (isUselightThemeColor)
            {
                inkCanvas.DefaultDrawingAttributes.Color = inkColorLightThemeMapping[inkColor];
            }
            else
            {
                inkCanvas.DefaultDrawingAttributes.Color = inkColorDarkThemeMapping[inkColor];
            }
            if (isUselightThemeColor)
            { // 亮系
                // Red
                BorderPenColorRed.Background = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                BoardBorderPenColorRed.Background = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                // Green
                BorderPenColorGreen.Background = new SolidColorBrush(Color.FromRgb(34, 197, 94));
                BoardBorderPenColorGreen.Background = new SolidColorBrush(Color.FromRgb(34, 197, 94));
                // Blue
                BorderPenColorBlue.Background = new SolidColorBrush(Color.FromRgb(59, 130, 246));
                BoardBorderPenColorBlue.Background = new SolidColorBrush(Color.FromRgb(59, 130, 246));
                // Yellow
                BorderPenColorYellow.Background = new SolidColorBrush(Color.FromRgb(250, 204, 21));
                BoardBorderPenColorYellow.Background = new SolidColorBrush(Color.FromRgb(250, 204, 21));
                // Pink ( Purple )
                BorderPenColorPink.Background = new SolidColorBrush(Color.FromRgb(236, 72, 153));
                BoardBorderPenColorPink.Background = new SolidColorBrush(Color.FromRgb(236, 72, 153));
                // Teal
                BorderPenColorTeal.Background = new SolidColorBrush(Color.FromRgb(20, 184, 166));
                BoardBorderPenColorTeal.Background = new SolidColorBrush(Color.FromRgb(20, 184, 166));
                // Orange
                BorderPenColorOrange.Background = new SolidColorBrush(Color.FromRgb(249, 115, 22));
                BoardBorderPenColorOrange.Background = new SolidColorBrush(Color.FromRgb(249, 115, 22));

                ColorThemeSwitchIcon.Glyph = "\uE708";
                BoardColorThemeSwitchIcon.Glyph = "\uE708";
                ColorThemeSwitchTextBlock.Text = "暗系";
            }
            else
            { // 暗系
                // Red
                BorderPenColorRed.Background = new SolidColorBrush(Color.FromRgb(220, 38, 38));
                BoardBorderPenColorRed.Background = new SolidColorBrush(Color.FromRgb(220, 38, 38));
                // Green
                BorderPenColorGreen.Background = new SolidColorBrush(Color.FromRgb(22, 163, 74));
                BoardBorderPenColorGreen.Background = new SolidColorBrush(Color.FromRgb(22, 163, 74));
                // Blue
                BorderPenColorBlue.Background = new SolidColorBrush(Color.FromRgb(37, 99, 235));
                BoardBorderPenColorBlue.Background = new SolidColorBrush(Color.FromRgb(37, 99, 235));
                // Yellow
                BorderPenColorYellow.Background = new SolidColorBrush(Color.FromRgb(234, 179, 8));
                BoardBorderPenColorYellow.Background = new SolidColorBrush(Color.FromRgb(234, 179, 8));
                // Pink ( Purple )
                BorderPenColorPink.Background = new SolidColorBrush(Color.FromRgb(147, 51, 234));
                BoardBorderPenColorPink.Background = new SolidColorBrush(Color.FromRgb(147, 51, 234));
                // Teal
                BorderPenColorTeal.Background = new SolidColorBrush(Color.FromRgb(13, 148, 136));
                BoardBorderPenColorTeal.Background = new SolidColorBrush(Color.FromRgb(13, 148, 136));
                // Orange
                BorderPenColorOrange.Background = new SolidColorBrush(Color.FromRgb(234, 88, 12));
                BoardBorderPenColorOrange.Background = new SolidColorBrush(Color.FromRgb(234, 88, 12));


                ColorThemeSwitchIcon.Glyph = "\uE706";
                BoardColorThemeSwitchIcon.Glyph = "\uE706";
                ColorThemeSwitchTextBlock.Text = "亮系";
            }
            if (drawingAttributes != null && isLoaded)
            {
                if (inkColor > 100)
                {
                    drawingAttributes.Height = 30 + Settings.Canvas.InkWidth;
                    drawingAttributes.Width = 30 + Settings.Canvas.InkWidth;
                    byte NowR = drawingAttributes.Color.R;
                    byte NowG = drawingAttributes.Color.G;
                    byte NowB = drawingAttributes.Color.B;
                    drawingAttributes.Color = Color.FromArgb((byte)Settings.Canvas.InkAlpha, NowR, NowG, NowB);
                    drawingAttributes.IsHighlighter = true;
                }
                else
                {
                    drawingAttributes.Height = Settings.Canvas.InkWidth;
                    drawingAttributes.Width = Settings.Canvas.InkWidth;
                    drawingAttributes.IsHighlighter = false;
                }
            }

            // 改变选中提示
            ViewboxBtnColorBlackContent.Visibility = Visibility.Collapsed;
            ViewboxBtnColorBlueContent.Visibility = Visibility.Collapsed;
            ViewboxBtnColorGreenContent.Visibility = Visibility.Collapsed;
            ViewboxBtnColorRedContent.Visibility = Visibility.Collapsed;
            ViewboxBtnColorYellowContent.Visibility = Visibility.Collapsed;
            ViewboxBtnColorWhiteContent.Visibility = Visibility.Collapsed;
            ViewboxBtnColorPinkContent.Visibility = Visibility.Collapsed;
            ViewboxBtnColorTealContent.Visibility = Visibility.Collapsed;
            ViewboxBtnColorOrangeContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnColorBlackContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnColorBlueContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnColorGreenContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnColorRedContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnColorYellowContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnColorWhiteContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnColorPinkContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnColorTealContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnColorOrangeContent.Visibility = Visibility.Collapsed;

            ViewboxBtnHighlighterColorRedContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnHighlighterColorRedContent.Visibility = Visibility.Collapsed;
            ViewboxBtnHighlighterColorOrangeContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnHighlighterColorOrangeContent.Visibility = Visibility.Collapsed;
            ViewboxBtnHighlighterColorYellowContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnHighlighterColorYellowContent.Visibility = Visibility.Collapsed;
            ViewboxBtnHighlighterColorTealContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnHighlighterColorTealContent.Visibility = Visibility.Collapsed;
            ViewboxBtnHighlighterColorBlueContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnHighlighterColorBlueContent.Visibility = Visibility.Collapsed;
            ViewboxBtnHighlighterColorPurpleContent.Visibility = Visibility.Collapsed;
            BoardViewboxBtnHighlighterColorPurpleContent.Visibility = Visibility.Collapsed;
            switch (inkColor)
            {
                case 0:
                    ViewboxBtnColorBlackContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnColorBlackContent.Visibility = Visibility.Visible;
                    break;
                case 1:
                    ViewboxBtnColorRedContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnColorRedContent.Visibility = Visibility.Visible;
                    break;
                case 2:
                    ViewboxBtnColorGreenContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnColorGreenContent.Visibility = Visibility.Visible;
                    break;
                case 3:
                    ViewboxBtnColorBlueContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnColorBlueContent.Visibility = Visibility.Visible;
                    break;
                case 4:
                    ViewboxBtnColorYellowContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnColorYellowContent.Visibility = Visibility.Visible;
                    break;
                case 5:
                    ViewboxBtnColorWhiteContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnColorWhiteContent.Visibility = Visibility.Visible;
                    break;
                case 6:
                    ViewboxBtnColorPinkContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnColorPinkContent.Visibility = Visibility.Visible;
                    break;
                case 7:
                    ViewboxBtnColorTealContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnColorTealContent.Visibility = Visibility.Visible;
                    break;
                case 8:
                    ViewboxBtnColorOrangeContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnColorOrangeContent.Visibility = Visibility.Visible;
                    break;
                case 101:
                    ViewboxBtnHighlighterColorRedContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnHighlighterColorRedContent.Visibility = Visibility.Visible;
                    break;
                case 102:
                    ViewboxBtnHighlighterColorOrangeContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnHighlighterColorOrangeContent.Visibility = Visibility.Visible;
                    break;
                case 103:
                    ViewboxBtnHighlighterColorYellowContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnHighlighterColorYellowContent.Visibility = Visibility.Visible;
                    break;
                case 104:
                    ViewboxBtnHighlighterColorTealContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnHighlighterColorTealContent.Visibility = Visibility.Visible;
                    break;
                case 105:
                    ViewboxBtnHighlighterColorBlueContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnHighlighterColorBlueContent.Visibility = Visibility.Visible;
                    break;
                case 106:
                    ViewboxBtnHighlighterColorPurpleContent.Visibility = Visibility.Visible;
                    BoardViewboxBtnHighlighterColorPurpleContent.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void CheckLastColor(int inkColor)
        {
            StrokeCollection strokes = inkCanvas.GetSelectedStrokes();
            if (strokes.Count > 0)
            {
                Color targetedColor = inkColorLightThemeMapping[inkColor];
                if (!isUselightThemeColor)
                {
                    inkCanvas.DefaultDrawingAttributes.Color = inkColorDarkThemeMapping[inkColor];
                }
                foreach (Stroke stroke in strokes)
                {
                    stroke.DrawingAttributes.Color = targetedColor;
                }
            }
            else
            {
                if (currentMode == 0)
                {
                    lastDesktopInkColor = inkColor;
                }
                else
                {
                    lastBoardInkColor = inkColor;
                }
                ColorSwitchCheck();
            }
        }

        private void BtnColorBlack_Click(object sender, RoutedEventArgs e)
        {
            CheckLastColor(0);
        }

        private void BtnColorRed_Click(object sender, RoutedEventArgs e)
        {
            CheckLastColor(1);
        }

        private void BtnColorGreen_Click(object sender, RoutedEventArgs e)
        {
            CheckLastColor(2);
        }

        private void BtnColorBlue_Click(object sender, RoutedEventArgs e)
        {
            CheckLastColor(3);
        }

        private void BtnColorYellow_Click(object sender, RoutedEventArgs e)
        {
            CheckLastColor(4);
        }

        private void BtnColorWhite_Click(object sender, RoutedEventArgs e)
        {
            CheckLastColor(5);
        }

        private void BtnColorPink_Click(object sender, RoutedEventArgs e)
        {
            CheckLastColor(6);
        }

        private void BtnColorTeal_Click(object sender, RoutedEventArgs e)
        {
            CheckLastColor(7);
        }

        private void BtnColorOrange_Click(object sender, RoutedEventArgs e)
        {
            CheckLastColor(8);
        }

        private Color StringToColor(string colorStr)
        {
            Byte[] argb = new Byte[4];
            for (int i = 0; i < 4; i++)
            {
                char[] charArray = colorStr.Substring(i * 2 + 1, 2).ToCharArray();
                Byte b1 = toByte(charArray[0]);
                Byte b2 = toByte(charArray[1]);
                argb[i] = (Byte)(b2 | (b1 << 4));
            }
            return Color.FromArgb(argb[0], argb[1], argb[2], argb[3]);//#FFFFFFFF
        }

        private static byte toByte(char c)
        {
            byte b = (byte)"0123456789ABCDEF".IndexOf(c);
            return b;
        }
    }
}