using Ink_Canvas.Helpers;
using Microsoft.Office.Interop.PowerPoint;
using iNKORE.UI.WPF.Modern;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Application = System.Windows.Application;
using File = System.IO.File;
using MessageBox = System.Windows.MessageBox;
using Microsoft.Office.Core;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        public static Microsoft.Office.Interop.PowerPoint.Application pptApplication = null;
        public static Microsoft.Office.Interop.PowerPoint.Presentation presentation = null;
        public static Microsoft.Office.Interop.PowerPoint.Slides slides = null;
        public static Microsoft.Office.Interop.PowerPoint.Slide slide = null;
        public static int slidescount = 0;

        /*
        private void BtnCheckPPT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pptApplication = (Microsoft.Office.Interop.PowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
                //pptApplication.SlideShowWindows[1].View.Next();
                if (pptApplication != null)
                {
                    //获得演示文稿对象
                    presentation = pptApplication.ActivePresentation;
                    pptApplication.SlideShowBegin += PptApplication_SlideShowBegin;
                    pptApplication.SlideShowNextSlide += PptApplication_SlideShowNextSlide;
                    pptApplication.SlideShowEnd += PptApplication_SlideShowEnd;
                    // 获得幻灯片对象集合
                    slides = presentation.Slides;
                    // 获得幻灯片的数量
                    slidescount = slides.Count;
                    memoryStreams = new MemoryStream[slidescount + 2];
                    // 获得当前选中的幻灯片
                    try
                    {
                        // 在普通视图下这种方式可以获得当前选中的幻灯片对象
                        // 然而在阅读模式下，这种方式会出现异常
                        slide = slides[pptApplication.ActiveWindow.Selection.SlideRange.SlideNumber];
                    }
                    catch
                    {
                        // 在阅读模式下出现异常时，通过下面的方式来获得当前选中的幻灯片对象
                        slide = pptApplication.SlideShowWindows[1].View.Slide;
                    }
                }

                if (pptApplication == null) throw new Exception();
                //BtnCheckPPT.Visibility = Visibility.Collapsed;
                BtnPPTSlideShowEnd.Visibility = Visibility.Visible;
            }
            catch
            {
                //BtnCheckPPT.Visibility = Visibility.Visible;
                BtnPPTSlideShowEnd.Visibility = Visibility.Collapsed;
                PPTNavigationBottomLeft.Visibility = Visibility.Collapsed;
                PPTNavigationBottomRight.Visibility = Visibility.Collapsed;
                PPTNavigationSidesLeft.Visibility = Visibility.Collapsed;
                PPTNavigationSidesRight.Visibility = Visibility.Collapsed;
                MessageBox.Show("未找到幻灯片");
            }
        }
        */

        private void ToggleSwitchSupportWPS_Toggled(object sender, RoutedEventArgs e)
        {
            if (!isLoaded) return;

            Settings.PowerPointSettings.IsSupportWPS = ToggleSwitchSupportWPS.IsOn;
            SaveSettingsToFile();
        }

        public static bool isWPSSupportOn => Settings.PowerPointSettings.IsSupportWPS;

        public static bool IsShowingRestoreHiddenSlidesWindow = false;

        private void TimerCheckPPT_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsShowingRestoreHiddenSlidesWindow) return;
            try
            {
                Process[] processes = Process.GetProcessesByName("wpp");
                if (processes.Length > 0 && !isWPSSupportOn)
                {
                    return;
                }

                //使用下方提前创建 PowerPoint 实例，将导致 PowerPoint 不再有启动界面
                //pptApplication = (Microsoft.Office.Interop.PowerPoint.Application)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("91493441-5A91-11CF-8700-00AA0060263B")));
                //new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowBegin").AddEventHandler(pptApplication, new EApplication_SlideShowBeginEventHandler(this.PptApplication_SlideShowBegin));
                //new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowEnd").AddEventHandler(pptApplication, new EApplication_SlideShowEndEventHandler(this.PptApplication_SlideShowEnd));
                //new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").AddEventHandler(pptApplication, new EApplication_SlideShowNextSlideEventHandler(this.PptApplication_SlideShowNextSlide));
                //ConfigHelper.Instance.IsInitApplicationSuccessful = true;

                pptApplication = (Microsoft.Office.Interop.PowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");

                if (pptApplication != null)
                {
                    timerCheckPPT.Stop();
                    //获得演示文稿对象
                    presentation = pptApplication.ActivePresentation;
                    pptApplication.PresentationClose += PptApplication_PresentationClose;
                    pptApplication.SlideShowBegin += PptApplication_SlideShowBegin;
                    pptApplication.SlideShowNextSlide += PptApplication_SlideShowNextSlide;
                    pptApplication.SlideShowEnd += PptApplication_SlideShowEnd;
                    // 获得幻灯片对象集合
                    slides = presentation.Slides;

                    // 获得幻灯片的数量
                    slidescount = slides.Count;
                    memoryStreams = new MemoryStream[slidescount + 2];
                    // 获得当前选中的幻灯片
                    try
                    {
                        // 在普通视图下这种方式可以获得当前选中的幻灯片对象
                        // 然而在阅读模式下，这种方式会出现异常
                        slide = slides[pptApplication.ActiveWindow.Selection.SlideRange.SlideNumber];
                    }
                    catch
                    {
                        // 在阅读模式下出现异常时，通过下面的方式来获得当前选中的幻灯片对象
                        slide = pptApplication.SlideShowWindows[1].View.Slide;
                    }
                }

                if (pptApplication == null) return;

                // 跳转到上次播放页
                if (Settings.PowerPointSettings.IsNotifyPreviousPage)
                {
                    Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        string folderPath = Settings.Automation.AutoSavedStrokesLocation + @"\Auto Saved - Presentations\" + presentation.Name + "_" + presentation.Slides.Count;
                        try
                        {
                            if (File.Exists(folderPath + "/Position"))
                            {
                                if (int.TryParse(File.ReadAllText(folderPath + "/Position"), out var page))
                                {
                                    if (page <= 0) return;
                                    new YesOrNoNotificationWindow($"上次播放到了第 {page} 页, 是否立即跳转", () =>
                                    {
                                        if (pptApplication.SlideShowWindows.Count >= 1)
                                        {
                                            // 如果已经播放了的话, 跳转
                                            presentation.SlideShowWindow.View.GotoSlide(page);
                                        }
                                        else
                                        {
                                            presentation.Windows[1].View.GotoSlide(page);
                                        }
                                    }).ShowDialog();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLogToFile(ex.ToString(), LogHelper.LogType.Error);
                        }
                    }));
                }

                //检查是否有隐藏幻灯片
                if (Settings.PowerPointSettings.IsNotifyHiddenPage)
                {
                    bool isHaveHiddenSlide = false;
                    foreach (Slide slide in slides)
                    {
                        if (slide.SlideShowTransition.Hidden == Microsoft.Office.Core.MsoTriState.msoTrue)
                        {
                            isHaveHiddenSlide = true;
                            break;
                        }
                    }

                    Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        if (isHaveHiddenSlide && !IsShowingRestoreHiddenSlidesWindow)
                        {
                            IsShowingRestoreHiddenSlidesWindow = true;
                            new YesOrNoNotificationWindow("检测到此演示文档中包含隐藏的幻灯片，是否取消隐藏？",
                                () =>
                                {
                                    foreach (Slide slide in slides)
                                    {
                                        if (slide.SlideShowTransition.Hidden == Microsoft.Office.Core.MsoTriState.msoTrue)
                                        {
                                            slide.SlideShowTransition.Hidden = Microsoft.Office.Core.MsoTriState.msoFalse;
                                        }
                                    }
                                }).ShowDialog();
                        }

                        // BtnPPTSlideShow.Visibility = Visibility.Visible;
                    }));
                }

                //检测是否有自动播放
                if (Settings.PowerPointSettings.IsNotifyAutoPlayPresentation
                    // && presentation.SlideShowSettings.AdvanceMode == PpSlideShowAdvanceMode.ppSlideShowUseSlideTimings
                    && BtnPPTSlideShowEnd.Visibility != Visibility.Visible)
                {
                    bool hasSlideTimings = false;
                    foreach (Slide slide in presentation.Slides)
                    {
                        if (slide.SlideShowTransition.AdvanceOnTime == MsoTriState.msoTrue && slide.SlideShowTransition.AdvanceTime > 0)
                        {
                            hasSlideTimings = true;
                            break;
                        }
                    }
                    if (hasSlideTimings)
                    {
                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            new YesOrNoNotificationWindow("检测到此演示文档中自动播放或排练计时已经启用，可能导致幻灯片自动翻页，是否取消？",
                                () =>
                                {
                                    presentation.SlideShowSettings.AdvanceMode = PpSlideShowAdvanceMode.ppSlideShowManualAdvance;
                                }).ShowDialog();
                        }));
                        presentation.SlideShowSettings.AdvanceMode = PpSlideShowAdvanceMode.ppSlideShowManualAdvance;
                    }
                }

                //如果检测到已经开始放映，则立即进入画板模式
                if (pptApplication.SlideShowWindows.Count >= 1)
                {
                    PptApplication_SlideShowBegin(pptApplication.SlideShowWindows[1]);
                }
            }
            catch
            {
                /*
                Application.Current.Dispatcher.Invoke(() =>
                {
                    BtnPPTSlideShow.Visibility = Visibility.Collapsed;
                });
                */
                timerCheckPPT.Start();
            }
        }

        private void PptApplication_PresentationClose(Presentation Pres)
        {
            pptApplication.PresentationClose -= PptApplication_PresentationClose;
            pptApplication.SlideShowBegin -= PptApplication_SlideShowBegin;
            pptApplication.SlideShowNextSlide -= PptApplication_SlideShowNextSlide;
            pptApplication.SlideShowEnd -= PptApplication_SlideShowEnd;
            pptApplication = null;
            timerCheckPPT.Start();
            
            Application.Current.Dispatcher.Invoke(() =>
            {
                //BtnPPTSlideShow.Visibility = Visibility.Collapsed;
                BtnPPTSlideShowEnd.Visibility = Visibility.Collapsed;
            });
            
        }

        bool isPresentationHaveBlackSpace = false;


        private string pptName = null;
        private void PptApplication_SlideShowBegin(SlideShowWindow Wn)
        {
            if (Settings.Automation.IsAutoFoldInPPTSlideShow && !isFloatingBarFolded)
            {
                FoldFloatingBar_Click(null, null);
            }
            else if (isFloatingBarFolded)
            {
                UnFoldFloatingBar_MouseUp(null, null);
            }

            LogHelper.WriteLogToFile("PowerPoint Application Slide Show Begin", LogHelper.LogType.Event);
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (currentMode != 0)
                {
                    ImageBlackboard_Click(null, null);
                }

                //调整颜色
                double screenRatio = SystemParameters.PrimaryScreenWidth / SystemParameters.PrimaryScreenHeight;
                if (Math.Abs(screenRatio - 16.0 / 9) <= -0.01)
                {
                    if (Wn.Presentation.PageSetup.SlideWidth / Wn.Presentation.PageSetup.SlideHeight < 1.65)
                    {
                        isPresentationHaveBlackSpace = true;
                    }
                }
                else if (screenRatio == -256 / 135)
                {

                }
                lastDesktopInkColor = 1;


                slidescount = Wn.Presentation.Slides.Count;
                previousSlideID = 0;
                memoryStreams = new MemoryStream[slidescount + 2];

                pptName = Wn.Presentation.Name;
                LogHelper.NewLog("Name: " + Wn.Presentation.Name);
                LogHelper.NewLog("Slides Count: " + slidescount.ToString());

                //检查是否有已有墨迹，并加载
                if (Settings.PowerPointSettings.IsAutoSaveStrokesInPowerPoint)
                {
                    if (Directory.Exists(Settings.Automation.AutoSavedStrokesLocation + @"\Auto Saved - Presentations\" + Wn.Presentation.Name + "_" + Wn.Presentation.Slides.Count))
                    {
                        LogHelper.WriteLogToFile("Found saved strokes", LogHelper.LogType.Trace);
                        FileInfo[] files = new DirectoryInfo(Settings.Automation.AutoSavedStrokesLocation + @"\Auto Saved - Presentations\" + Wn.Presentation.Name + "_" + Wn.Presentation.Slides.Count).GetFiles();
                        int count = 0;
                        foreach (FileInfo file in files)
                        {
                            if (file.Name != "Position")
                            {
                                int i = -1;
                                try
                                {
                                    i = int.Parse(System.IO.Path.GetFileNameWithoutExtension(file.Name));
                                    memoryStreams[i] = new MemoryStream(File.ReadAllBytes(file.FullName));
                                    memoryStreams[i].Position = 0;
                                    count++;
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.WriteLogToFile(string.Format("Failed to load strokes on Slide {0}\n{1}", i, ex.ToString()), LogHelper.LogType.Error);
                                }
                            }
                        }
                        LogHelper.WriteLogToFile(string.Format("Loaded {0} saved strokes", count.ToString()));
                    }
                }

                BtnPPTSlideShowEnd.Visibility = Visibility.Visible;

                if (Settings.PowerPointSettings.IsShowBottomPPTNavigationPanel)
                {
                    AnimationsHelper.ShowWithScaleFromBottom(PPTNavigationBottomLeft);
                    AnimationsHelper.ShowWithScaleFromBottom(PPTNavigationBottomRight);
                }
                else
                {
                    PPTNavigationBottomLeft.Visibility = Visibility.Collapsed;
                    PPTNavigationBottomRight.Visibility = Visibility.Collapsed;
                }
                if (Settings.PowerPointSettings.IsShowSidePPTNavigationPanel)
                {
                    AnimationsHelper.ShowWithScaleFromLeft(PPTNavigationSidesLeft);
                    AnimationsHelper.ShowWithScaleFromRight(PPTNavigationSidesRight);
                }
                else
                {
                    PPTNavigationSidesLeft.Visibility = Visibility.Collapsed;
                    PPTNavigationSidesRight.Visibility = Visibility.Collapsed;
                }

                //BtnPPTSlideShow.Visibility = Visibility.Collapsed;
                BtnPPTSlideShowEnd.Visibility = Visibility.Visible;

                if (Settings.Appearance.IsColorfulViewboxFloatingBar)
                {
                    ViewboxFloatingBar.Opacity = 0.8;
                }
                else
                {
                    ViewboxFloatingBar.Opacity = 0.75;
                }

                if (Settings.PowerPointSettings.IsShowCanvasAtNewSlideShow && Main_Grid.Background == Brushes.Transparent)
                {
                    if (currentMode != 0)
                    {
                        currentMode = 0;
                        GridBackgroundCover.Visibility = Visibility.Collapsed;
                        AnimationsHelper.HideWithSlideAndFade(BlackboardLeftSide);
                        AnimationsHelper.HideWithSlideAndFade(BlackboardCenterSide);
                        AnimationsHelper.HideWithSlideAndFade(BlackboardRightSide);

                        //SaveStrokes();
                        ClearStrokes(true);
                    }
                    BtnHideInkCanvas_Click(null, null);
                }

                ClearStrokes(true);

                BorderFloatingBarMainControls.Visibility = Visibility.Visible;

                if (Settings.PowerPointSettings.IsShowCanvasAtNewSlideShow)
                {
                    BtnColorRed_Click(null, null);
                }

                isEnteredSlideShowEndEvent = false;
                PptNavigationTextBlockBottom.Text = $"{Wn.View.CurrentShowPosition}/{Wn.Presentation.Slides.Count}";
                LogHelper.NewLog("PowerPoint Slide Show Loading process complete");

                new Thread(new ThreadStart(() =>
                {
                    Thread.Sleep(100);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ViewboxFloatingBarMarginAnimation(60);
                    });
                })).Start();
            });
        }

        bool isEnteredSlideShowEndEvent = false; //防止重复调用本函数导致墨迹保存失效
        private async void PptApplication_SlideShowEnd(Presentation Pres)
        {
            if (isFloatingBarFolded) UnFoldFloatingBar_MouseUp(null, null);

            LogHelper.WriteLogToFile(string.Format("PowerPoint Slide Show End"), LogHelper.LogType.Event);
            if (isEnteredSlideShowEndEvent)
            {
                LogHelper.WriteLogToFile("Detected previous entrance, returning");
                return;
            }
            isEnteredSlideShowEndEvent = true;
            if (Settings.PowerPointSettings.IsAutoSaveStrokesInPowerPoint)
            {
                string folderPath = Settings.Automation.AutoSavedStrokesLocation + @"\Auto Saved - Presentations\" + Pres.Name + "_" + Pres.Slides.Count;
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                try
                {
                    File.WriteAllText(folderPath + "/Position", previousSlideID.ToString());
                }
                catch { }
                for (int i = 1; i <= Pres.Slides.Count; i++)
                {
                    if (memoryStreams[i] != null)
                    {
                        try
                        {
                            if (memoryStreams[i].Length > 8)
                            {
                                byte[] srcBuf = new Byte[memoryStreams[i].Length];
                                int byteLength = memoryStreams[i].Read(srcBuf, 0, srcBuf.Length);
                                File.WriteAllBytes(folderPath + @"\" + i.ToString("0000") + ".icstk", srcBuf);
                                LogHelper.WriteLogToFile(string.Format("Saved strokes for Slide {0}, size={1}, byteLength={2}", i.ToString(), memoryStreams[i].Length, byteLength));
                            }
                            else
                            {
                                File.Delete(folderPath + @"\" + i.ToString("0000") + ".icstk");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLogToFile(string.Format("Failed to save strokes for Slide {0}\n{1}", i, ex.ToString()), LogHelper.LogType.Error);
                            File.Delete(folderPath + @"\" + i.ToString("0000") + ".icstk");
                        }
                    }
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                isPresentationHaveBlackSpace = false;

                //BtnPPTSlideShow.Visibility = Visibility.Visible;
                BtnPPTSlideShowEnd.Visibility = Visibility.Collapsed;
                BtnPPTSlideShowEnd.Visibility = Visibility.Collapsed;
                PPTNavigationBottomLeft.Visibility = Visibility.Collapsed;
                PPTNavigationBottomRight.Visibility = Visibility.Collapsed;
                PPTNavigationSidesLeft.Visibility = Visibility.Collapsed;
                PPTNavigationSidesRight.Visibility = Visibility.Collapsed;

                if (currentMode != 0)
                {
                    ImageBlackboard_Click(null, null);
                }

                ClearStrokes(true);

                if (Main_Grid.Background != Brushes.Transparent)
                {
                    BtnHideInkCanvas_Click(null, null);
                }

                if (Settings.Appearance.IsColorfulViewboxFloatingBar)
                {
                    ViewboxFloatingBar.Opacity = 0.95;
                }
                else
                {
                    ViewboxFloatingBar.Opacity = 1;
                }
            });

            await Task.Delay(150);
            ViewboxFloatingBarMarginAnimation(100);
        }

        int previousSlideID = 0;
        MemoryStream[] memoryStreams = new MemoryStream[50];

        private void PptApplication_SlideShowNextSlide(SlideShowWindow Wn)
        {
            LogHelper.WriteLogToFile(string.Format("PowerPoint Next Slide (Slide {0})", Wn.View.CurrentShowPosition), LogHelper.LogType.Event);
            if (Wn.View.CurrentShowPosition != previousSlideID)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MemoryStream ms = new MemoryStream();
                    inkCanvas.Strokes.Save(ms);
                    ms.Position = 0;
                    memoryStreams[previousSlideID] = ms;

                    if (inkCanvas.Strokes.Count > Settings.Automation.MinimumAutomationStrokeNumber && Settings.PowerPointSettings.IsAutoSaveScreenShotInPowerPoint && !_isPptClickingBtnTurned)
                        SavePPTScreenshot(Wn.Presentation.Name + "/" + Wn.View.CurrentShowPosition);
                    _isPptClickingBtnTurned = false;

                    ClearStrokes(true);
                    timeMachine.ClearStrokeHistory();

                    try
                    {
                        if (memoryStreams[Wn.View.CurrentShowPosition] != null && memoryStreams[Wn.View.CurrentShowPosition].Length > 0)
                        {
                            inkCanvas.Strokes.Add(new StrokeCollection(memoryStreams[Wn.View.CurrentShowPosition]));
                        }
                    }
                    catch { }

                    PptNavigationTextBlockBottom.Text = $"{Wn.View.CurrentShowPosition}/{Wn.Presentation.Slides.Count}";
                });
                previousSlideID = Wn.View.CurrentShowPosition;

            }
        }

        private bool _isPptClickingBtnTurned = false;

        private void BtnPPTSlidesUp_Click(object sender, RoutedEventArgs e)
        {
            if (currentMode == 1)
            {
                GridBackgroundCover.Visibility = Visibility.Collapsed;
                AnimationsHelper.HideWithSlideAndFade(BlackboardLeftSide);
                AnimationsHelper.HideWithSlideAndFade(BlackboardCenterSide);
                AnimationsHelper.HideWithSlideAndFade(BlackboardRightSide);
                currentMode = 0;
            }

            _isPptClickingBtnTurned = true;

            if (inkCanvas.Strokes.Count > Settings.Automation.MinimumAutomationStrokeNumber &&
                Settings.PowerPointSettings.IsAutoSaveScreenShotInPowerPoint)
                SavePPTScreenshot(pptApplication.SlideShowWindows[1].Presentation.Name + "/" + pptApplication.SlideShowWindows[1].View.CurrentShowPosition);

            try
            {
                new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        pptApplication.SlideShowWindows[1].Activate();
                    }
                    catch { }
                    try
                    {
                        pptApplication.SlideShowWindows[1].View.Previous();
                    }
                    catch { } // Without this catch{}, app will crash when click the pre-page button in the fir page in some special env.
                })).Start();
            }
            catch
            {
                BtnPPTSlideShowEnd.Visibility = Visibility.Collapsed;
                PPTNavigationBottomLeft.Visibility = Visibility.Collapsed;
                PPTNavigationBottomRight.Visibility = Visibility.Collapsed;
                PPTNavigationSidesLeft.Visibility = Visibility.Collapsed;
                PPTNavigationSidesRight.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnPPTSlidesDown_Click(object sender, RoutedEventArgs e)
        {
            if (currentMode == 1)
            {
                GridBackgroundCover.Visibility = Visibility.Collapsed;
                AnimationsHelper.HideWithSlideAndFade(BlackboardLeftSide);
                AnimationsHelper.HideWithSlideAndFade(BlackboardCenterSide);
                AnimationsHelper.HideWithSlideAndFade(BlackboardRightSide);
                currentMode = 0;
            }
            _isPptClickingBtnTurned = true;
            if (inkCanvas.Strokes.Count > Settings.Automation.MinimumAutomationStrokeNumber &&
                Settings.PowerPointSettings.IsAutoSaveScreenShotInPowerPoint)
                SavePPTScreenshot(pptApplication.SlideShowWindows[1].Presentation.Name + "/" + pptApplication.SlideShowWindows[1].View.CurrentShowPosition);
            try
            {
                new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        pptApplication.SlideShowWindows[1].Activate();
                    }
                    catch { }
                    try
                    {
                        pptApplication.SlideShowWindows[1].View.Next();
                    }
                    catch { }
                })).Start();
            }
            catch
            {
                BtnPPTSlideShowEnd.Visibility = Visibility.Collapsed;
                PPTNavigationBottomLeft.Visibility = Visibility.Collapsed;
                PPTNavigationBottomRight.Visibility = Visibility.Collapsed;
                PPTNavigationSidesLeft.Visibility = Visibility.Collapsed;
                PPTNavigationSidesRight.Visibility = Visibility.Collapsed;
            }
        }


        private async void PPTNavigationBtn_Click(object sender, MouseButtonEventArgs e)
        {
            if (lastBorderMouseDownObject != sender) return;
            Main_Grid.Background = new SolidColorBrush(StringToColor("#01FFFFFF"));
            CursorIcon_Click(null, null);
            try
            {
                pptApplication.SlideShowWindows[1].SlideNavigation.Visible = true;
            }
            catch { }
            // 控制居中
            if (!isFloatingBarFolded)
            {
                await Task.Delay(100);
                ViewboxFloatingBarMarginAnimation(60);
            }
        }

        /*
        private void BtnPPTSlideShow_Click(object sender, RoutedEventArgs e)
        {
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    presentation.SlideShowSettings.Run();
                }
                catch { }
            })).Start();
        }
        */

        private async void BtnPPTSlideShowEnd_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    MemoryStream ms = new MemoryStream();
                    inkCanvas.Strokes.Save(ms);
                    ms.Position = 0;
                    memoryStreams[pptApplication.SlideShowWindows[1].View.CurrentShowPosition] = ms;
                    timeMachine.ClearStrokeHistory();
                }
                catch { }
            });
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    pptApplication.SlideShowWindows[1].View.Exit();
                }
                catch { }
            })).Start();

            HideSubPanels("cursor");
            await Task.Delay(150);
            ViewboxFloatingBarMarginAnimation(100);
        }

        private void GridPPTControlPrevious_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (lastBorderMouseDownObject != sender) return;
            BtnPPTSlidesUp_Click(null, null);
        }

        private void GridPPTControlNext_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (lastBorderMouseDownObject != sender) return;
            BtnPPTSlidesDown_Click(null, null);
        }

        private void ImagePPTControlEnd_MouseUp(object sender, MouseButtonEventArgs e)
        {
            BtnPPTSlideShowEnd_Click(BtnPPTSlideShowEnd, null);
        }
    }
}
