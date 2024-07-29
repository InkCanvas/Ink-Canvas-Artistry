using Ink_Canvas.Helpers;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Markup;
using System.Windows.Media;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        private void SymbolIconSaveStrokes_Click(object sender, RoutedEventArgs e)
        {
            if (inkCanvas.Visibility != Visibility.Visible) return;
            AnimationsHelper.HideWithSlideAndFade(BorderTools);
            AnimationsHelper.HideWithSlideAndFade(BoardBorderTools);
            GridNotifications.Visibility = Visibility.Collapsed;
            SaveInkCanvasStrokes(true, true);
        }

        private void SaveInkCanvasStrokes(bool newNotice = true, bool saveByUser = false)
        {
            try
            {
                string savePath = Settings.Automation.AutoSavedStrokesLocation
                    + (saveByUser ? @"\User Saved - " : @"\Auto Saved - ")
                    + (currentMode == 0 ? "Annotation Strokes" : "BlackBoard Strokes");
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                string savePathWithName = savePath + @"\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-fff")
                    + (currentMode != 0 ? " Page-" + CurrentWhiteboardIndex + " StrokesCount-" + inkCanvas.Strokes.Count + ".icart" : ".icart");

                using (FileStream fs = new FileStream(savePathWithName, FileMode.Create))
                using (var archive = new System.IO.Compression.ZipArchive(fs, System.IO.Compression.ZipArchiveMode.Create))
                {
                    // save strokes
                    var strokesEntry = archive.CreateEntry("strokes.icstk");
                    using (var strokesStream = strokesEntry.Open())
                    {
                        inkCanvas.Strokes.Save(strokesStream);
                    }

                    // save UI elements
                    var elementsEntry = archive.CreateEntry("elements.xaml");
                    using (var elementsStream = elementsEntry.Open())
                    {
                        XamlWriter.Save(inkCanvas, elementsStream);
                    }

                    if (newNotice)
                    {
                        ShowNotificationAsync("墨迹及元素成功保存至 " + savePathWithName);
                    }
                }
            }
            catch (Exception Ex)
            {
                ShowNotificationAsync("墨迹及元素保存失败！");
                LogHelper.WriteLogToFile("墨迹及元素保存失败 | " + Ex.ToString(), LogHelper.LogType.Error);
            }
        }

        private void SymbolIconOpenStrokes_Click(object sender, RoutedEventArgs e)
        {
            AnimationsHelper.HideWithSlideAndFade(BorderTools);
            AnimationsHelper.HideWithSlideAndFade(BoardBorderTools);

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Settings.Automation.AutoSavedStrokesLocation,
                Title = "打开墨迹文件",
                Filter = "Ink Canvas Files (*.icart;*.icstk)|*.icart;*.icstk|Ink Canvas Artistry Files (*.icart)|*.icart|Ink Canvas Stroke Files (*.icstk)|*.icstk"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LogHelper.WriteLogToFile($"Strokes Insert: Name: {openFileDialog.FileName}", LogHelper.LogType.Event);

                try
                {
                    string extension = Path.GetExtension(openFileDialog.FileName).ToLower();
                    using (var fs = new FileStream(openFileDialog.FileName, FileMode.Open))
                    {
                        if (extension == ".icart")
                        {
                            using (var archive = new System.IO.Compression.ZipArchive(fs, System.IO.Compression.ZipArchiveMode.Read))
                            {
                                // strokes
                                var strokesEntry = archive.GetEntry("strokes.icstk");
                                if (strokesEntry != null)
                                {
                                    using (var strokesStream = strokesEntry.Open())
                                    {
                                        var strokes = new StrokeCollection(strokesStream);
                                        ClearStrokes(true);
                                        timeMachine.ClearStrokeHistory();
                                        inkCanvas.Strokes.Add(strokes);
                                        LogHelper.NewLog($"Strokes Insert: Strokes Count: {inkCanvas.Strokes.Count}");
                                    }
                                }

                                // UI Elements
                                var elementsEntry = archive.GetEntry("elements.xaml");
                                using (var elementsStream = elementsEntry.Open())
                                {
                                    try
                                    {
                                        var loadedCanvas = XamlReader.Load(elementsStream) as InkCanvas;
                                        if (loadedCanvas != null)
                                        {
                                            inkCanvas.Children.Clear();
                                            foreach (UIElement child in loadedCanvas.Children)
                                            {
                                                UIElement clonedChild = CloneUIElement(child);
                                                // 设置克隆子元素的 RenderTransform 和其他属性
                                                if (child is FrameworkElement frameworkElement)
                                                {
                                                    if (frameworkElement.RenderTransform != null)
                                                    {
                                                        clonedChild.SetValue(UIElement.RenderTransformProperty, CloneTransform(frameworkElement.RenderTransform));
                                                    }
                                                    clonedChild.SetValue(UIElement.OpacityProperty, frameworkElement.Opacity);
                                                }
                                                inkCanvas.Children.Add(clonedChild);
                                            }
                                            LogHelper.NewLog($"Elements Insert: Elements Count: {inkCanvas.Children.Count}");
                                        }
                                    }
                                    catch (XamlParseException xamlEx)
                                    {
                                        LogHelper.WriteLogToFile($"XAML 解析错误: {xamlEx.Message}", LogHelper.LogType.Error);
                                        ShowNotificationAsync("加载 UI 元素时出现 XAML 解析错误");
                                    }
                                    catch (Exception ex)
                                    {
                                        LogHelper.WriteLogToFile($"加载 UI 元素失败: {ex.Message}", LogHelper.LogType.Error);
                                        ShowNotificationAsync("加载 UI 元素失败");
                                    }
                                }
                            }
                        }
                        else if (extension == ".icstk")
                        {
                            // 直接加载 .icstk 文件中的墨迹
                            using (var strokesStream = new MemoryStream())
                            {
                                fs.CopyTo(strokesStream);
                                strokesStream.Seek(0, SeekOrigin.Begin);
                                var strokes = new StrokeCollection(strokesStream);
                                ClearStrokes(true);
                                timeMachine.ClearStrokeHistory();
                                inkCanvas.Strokes.Add(strokes);
                                LogHelper.NewLog($"Strokes Insert: Strokes Count: {inkCanvas.Strokes.Count}");
                            }
                        }
                        else
                        {
                            ShowNotificationAsync("不支持的文件格式。");
                        }

                        if (inkCanvas.Visibility != Visibility.Visible)
                        {
                            SymbolIconCursor_Click(sender, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowNotificationAsync("墨迹或元素打开失败");
                    LogHelper.WriteLogToFile($"打开墨迹或元素失败: {ex.Message}\n{ex.StackTrace}", LogHelper.LogType.Error);
                }
            }
        }

        private static UIElement CloneUIElement(UIElement element)
        {
            // 使用 XAML 克隆元素
            var xaml = XamlWriter.Save(element);
            return (UIElement)XamlReader.Parse(xaml);
        }

        private static Transform CloneTransform(Transform transform)
        {
            // 克隆 Transform 对象
            if (transform is MatrixTransform matrixTransform)
            {
                return new MatrixTransform(matrixTransform.Matrix);
            }
            else if (transform is TransformGroup transformGroup)
            {
                var clonedGroup = new TransformGroup();
                foreach (var t in transformGroup.Children)
                {
                    clonedGroup.Children.Add(CloneTransform(t));
                }
                return clonedGroup;
            }
            // 支持更多的 Transform 类型
            return transform.Clone();
        }
    }
}
