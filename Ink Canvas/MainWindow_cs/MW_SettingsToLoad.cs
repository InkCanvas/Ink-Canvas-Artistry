using Ink_Canvas.Helpers;
using Newtonsoft.Json;
using OSVersionExtension;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using File = System.IO.File;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        private void LoadSettings(bool isStartup = false)
        {
            try
            {
                if (File.Exists(App.RootPath + settingsFileName))
                {
                    try
                    {
                        string text = File.ReadAllText(App.RootPath + settingsFileName);
                        Settings = JsonConvert.DeserializeObject<Settings>(text);
                    }
                    catch { }
                }
                else
                {
                    BtnResetToSuggestion_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogToFile(ex.ToString(), LogHelper.LogType.Error);
            }
            // Startup
            if (isStartup)
            {
                CursorIcon_Click(null, null);
            }
            try
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\Ink Canvas Annotation.lnk"))
                {
                    StartAutomaticallyDel("Ink Canvas Annotation");
                    StartAutomaticallyCreate("Ink Canvas Artistry");
                    ToggleSwitchRunAtStartup.IsOn = true;
                }
                else if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\Ink Canvas Artistry.lnk"))
                {
                    ToggleSwitchRunAtStartup.IsOn = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogToFile(ex.ToString(), LogHelper.LogType.Error);
            }
            if (Settings.Startup != null)
            {
                if (isStartup)
                {
                    if (Settings.Automation.AutoDelSavedFiles)
                    {
                        DelAutoSavedFiles.DeleteFilesOlder(Settings.Automation.AutoSavedStrokesLocation, Settings.Automation.AutoDelSavedFilesDaysThreshold);
                    }
                    if (Settings.Startup.IsFoldAtStartup)
                    {
                        FoldFloatingBar_Click(Fold_Icon, null);
                    }
                }
                if (Settings.Startup.IsEnableNibMode)
                {
                    ToggleSwitchEnableNibMode.IsOn = true;
                    BoardToggleSwitchEnableNibMode.IsOn = true;
                    BoundsWidth = Settings.Advanced.NibModeBoundsWidth;
                }
                else
                {
                    ToggleSwitchEnableNibMode.IsOn = false;
                    BoardToggleSwitchEnableNibMode.IsOn = false;
                    BoundsWidth = Settings.Advanced.FingerModeBoundsWidth;
                }
                if (Settings.Startup.IsAutoUpdate)
                {
                    ToggleSwitchIsAutoUpdate.IsOn = true;
                    AutoUpdate();
                }
                ToggleSwitchIsAutoUpdateWithProxy.IsOn = Settings.Startup.IsAutoUpdateWithProxy;
                AutoUpdateWithProxy_Title.Visibility = Settings.Startup.IsAutoUpdateWithProxy ? Visibility.Visible : Visibility.Collapsed;
                AutoUpdateProxyTextBox.Text = Settings.Startup.AutoUpdateProxy;
                IsAutoUpdateWithSilenceBlock.Visibility = Settings.Startup.IsAutoUpdate ? Visibility.Visible : Visibility.Collapsed;
                if (Settings.Startup.IsAutoUpdateWithSilence)
                {
                    ToggleSwitchIsAutoUpdateWithSilence.IsOn = true;
                }
                AutoUpdateTimePeriodBlock.Visibility = Settings.Startup.IsAutoUpdateWithSilence ? Visibility.Visible : Visibility.Collapsed;

                AutoUpdateWithSilenceTimeComboBox.InitializeAutoUpdateWithSilenceTimeComboBoxOptions(AutoUpdateWithSilenceStartTimeComboBox, AutoUpdateWithSilenceEndTimeComboBox);
                AutoUpdateWithSilenceStartTimeComboBox.SelectedItem = Settings.Startup.AutoUpdateWithSilenceStartTime;
                AutoUpdateWithSilenceEndTimeComboBox.SelectedItem = Settings.Startup.AutoUpdateWithSilenceEndTime;

                ToggleSwitchFoldAtStartup.IsOn = Settings.Startup.IsFoldAtStartup;
            }
            else
            {
                Settings.Startup = new Startup();
            }
            // Appearance
            if (Settings.Appearance != null)
            {
                ComboBoxTheme.SelectedIndex = Settings.Appearance.Theme;

                if (Settings.Appearance.IsEnableDisPlayFloatBarText)
                {
                    FloatBarSelectIconTextBlock.Visibility = Visibility.Visible;
                    Icon_Pen.Height = 22;
                    Icon_Eraser1.Height = 22;
                    Icon_Eraser2.Height = 22;
                    Icon_Eraser2.Margin = new Thickness(5, -22, 0, -8);
                    Icon_EraserByStrokes1.Height = 22;
                    Icon_EraserByStrokes2.Height = 22;
                    Icon_EraserByStrokes2.Margin = new Thickness(12, -22, 0, -8);
                    Icon_Select1.Height = 22;
                    Icon_Select2.Height = 22;
                    Icon_Select2.Margin = new Thickness(6, -18, 0, -8);
                    Icon_Undo.Margin = new Thickness(0,1.5,0,-1.5);
                    Icon_Redo.Margin = new Thickness(0, 1.5, 0, -1.5);
                    ToggleSwitchEnableDisPlayFloatBarText.IsOn = true;
                }
                else
                {
                    FloatBarSelectIconTextBlock.Visibility = Visibility.Collapsed;
                    Icon_Pen.Height = 32;
                    Icon_Eraser1.Height = 32;
                    Icon_Eraser2.Height = 32;
                    Icon_Eraser2.Margin = new Thickness(5, -32, 0, -8);
                    Icon_EraserByStrokes1.Height = 32;
                    Icon_EraserByStrokes2.Height = 32;
                    Icon_EraserByStrokes2.Margin = new Thickness(12, -32, 0, -8);
                    Icon_Select1.Height = 32;
                    Icon_Select2.Height = 32;
                    Icon_Select2.Margin = new Thickness(6, -28, 0, -8);
                    Icon_Undo.Margin = new Thickness(0);
                    Icon_Redo.Margin = new Thickness(0);
                    ToggleSwitchEnableDisPlayFloatBarText.IsOn = false;
                }
                if (Settings.Appearance.IsEnableDisPlayNibModeToggler)
                {
                    NibModeSimpleStackPanel.Visibility = Visibility.Visible;
                    BoardNibModeSimpleStackPanel.Visibility = Visibility.Visible;
                    ToggleSwitchEnableDisPlayNibModeToggle.IsOn = true;
                }
                else
                {
                    NibModeSimpleStackPanel.Visibility = Visibility.Collapsed;
                    BoardNibModeSimpleStackPanel.Visibility = Visibility.Collapsed;
                    ToggleSwitchEnableDisPlayNibModeToggle.IsOn = false;
                }

                SystemEvents_UserPreferenceChanged(null, null);

                if (Settings.Appearance.IsColorfulViewboxFloatingBar) // 浮动工具栏背景色
                {
                    LinearGradientBrush gradientBrush = new LinearGradientBrush();
                    gradientBrush.StartPoint = new Point(0, 0);
                    gradientBrush.EndPoint = new Point(1, 1);
                    GradientStop blueStop = new GradientStop(Color.FromArgb(0x95, 0x80, 0xB0, 0xFF), 0);
                    GradientStop greenStop = new GradientStop(Color.FromArgb(0x95, 0xC0, 0xFF, 0xC0), 1);
                    gradientBrush.GradientStops.Add(blueStop);
                    gradientBrush.GradientStops.Add(greenStop);
                    EnableTwoFingerGestureBorder.Background = gradientBrush;
                    BorderFloatingBarMainControls.Background = gradientBrush;
                    BorderFloatingBarMoveControls.Background = gradientBrush;
                    BtnPPTSlideShowEnd.Background = gradientBrush;
                    ToggleSwitchColorfulViewboxFloatingBar.IsOn = true;
                }
                else
                {
                    ToggleSwitchColorfulViewboxFloatingBar.IsOn = false;
                }
                if (Settings.Appearance.EnableViewboxFloatingBarScaleTransform) // 浮动工具栏 UI 缩放 90%
                {
                    ViewboxFloatingBarScaleTransform.ScaleX = 0.9;
                    ViewboxFloatingBarScaleTransform.ScaleY = 0.9;

                    ToggleSwitchEnableViewboxFloatingBarScaleTransform.IsOn = true;
                }
                else
                {
                    ViewboxFloatingBarScaleTransform.ScaleX = 1;
                    ViewboxFloatingBarScaleTransform.ScaleY = 1;

                    ToggleSwitchEnableViewboxFloatingBarScaleTransform.IsOn = false;
                }
                if (Settings.Appearance.EnableViewboxBlackBoardScaleTransform) // 画板 UI 缩放 80%
                {
                    ViewboxBlackboardLeftSideScaleTransform.ScaleX = 0.8;
                    ViewboxBlackboardLeftSideScaleTransform.ScaleY = 0.8;
                    ViewboxBlackboardCenterSideScaleTransform.ScaleX = 0.8;
                    ViewboxBlackboardCenterSideScaleTransform.ScaleY = 0.8;
                    ViewboxBlackboardRightSideScaleTransform.ScaleX = 0.8;
                    ViewboxBlackboardRightSideScaleTransform.ScaleY = 0.8;

                    ToggleSwitchEnableViewboxBlackBoardScaleTransform.IsOn = true;
                }
                else
                {
                    ViewboxBlackboardLeftSideScaleTransform.ScaleX = 1;
                    ViewboxBlackboardLeftSideScaleTransform.ScaleY = 1;
                    ViewboxBlackboardCenterSideScaleTransform.ScaleX = 1;
                    ViewboxBlackboardCenterSideScaleTransform.ScaleY = 1;
                    ViewboxBlackboardRightSideScaleTransform.ScaleX = 1;
                    ViewboxBlackboardRightSideScaleTransform.ScaleY = 1;

                    ToggleSwitchEnableViewboxBlackBoardScaleTransform.IsOn = false;
                }
            }
            else
            {
                Settings.Appearance = new Appearance();
            }
            // PowerPointSettings
            if (Settings.PowerPointSettings != null)
            {
                PptNavigationBottomBtn.Visibility = Settings.PowerPointSettings.IsShowPPTNavigationBottom ? Visibility.Visible : Visibility.Collapsed;
                ToggleSwitchShowButtonPPTNavigationBottom.IsOn = Settings.PowerPointSettings.IsShowPPTNavigationBottom;
                ToggleSwitchShowButtonPPTNavigationSides.IsOn = Settings.PowerPointSettings.IsShowPPTNavigationSides;
                ToggleSwitchShowPPTNavigationPanelBottom.IsOn = Settings.PowerPointSettings.IsShowBottomPPTNavigationPanel;
                ToggleSwitchShowPPTNavigationPanelSide.IsOn = Settings.PowerPointSettings.IsShowSidePPTNavigationPanel;
                if (Settings.PowerPointSettings.PowerPointSupport)
                {
                    ToggleSwitchSupportPowerPoint.IsOn = true;
                    timerCheckPPT.Start();
                }
                else
                {
                    ToggleSwitchSupportPowerPoint.IsOn = false;
                    timerCheckPPT.Stop();
                }
                if (Settings.PowerPointSettings.IsShowCanvasAtNewSlideShow)
                {
                    ToggleSwitchShowCanvasAtNewSlideShow.IsOn = true;
                }
                else
                {
                    ToggleSwitchShowCanvasAtNewSlideShow.IsOn = false;
                }
                if (Settings.PowerPointSettings.IsEnableTwoFingerGestureInPresentationMode)
                {
                    ToggleSwitchEnableTwoFingerGestureInPresentationMode.IsOn = true;
                }
                else
                {
                    ToggleSwitchEnableTwoFingerGestureInPresentationMode.IsOn = false;
                }
                if (Settings.PowerPointSettings.IsEnableFingerGestureSlideShowControl)
                {
                    ToggleSwitchEnableFingerGestureSlideShowControl.IsOn = true;
                }
                else
                {
                    ToggleSwitchEnableFingerGestureSlideShowControl.IsOn = false;
                }


                if (Settings.PowerPointSettings.IsAutoSaveStrokesInPowerPoint)
                {
                    ToggleSwitchAutoSaveStrokesInPowerPoint.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoSaveStrokesInPowerPoint.IsOn = false;
                }

                if (Settings.PowerPointSettings.IsNotifyPreviousPage)
                {
                    ToggleSwitchNotifyPreviousPage.IsOn = true;
                }
                else
                {
                    ToggleSwitchNotifyPreviousPage.IsOn = false;
                }

                if (Settings.PowerPointSettings.IsNotifyHiddenPage)
                {
                    ToggleSwitchNotifyHiddenPage.IsOn = true;
                }
                else
                {
                    ToggleSwitchNotifyHiddenPage.IsOn = false;
                }
                if (Settings.PowerPointSettings.IsNotifyAutoPlayPresentation)
                {
                    ToggleSwitchNotifyAutoPlayPresentation.IsOn = true;
                }
                else
                {
                    ToggleSwitchNotifyAutoPlayPresentation.IsOn = false;
                }
                if (Settings.PowerPointSettings.IsSupportWPS)
                {
                    ToggleSwitchSupportWPS.IsOn = true;
                }
                else
                {
                    ToggleSwitchSupportWPS.IsOn = false;
                }
                if (Settings.PowerPointSettings.IsAutoSaveScreenShotInPowerPoint)
                {
                    ToggleSwitchAutoSaveScreenShotInPowerPoint.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoSaveScreenShotInPowerPoint.IsOn = false;
                }
            }
            else
            {
                Settings.PowerPointSettings = new PowerPointSettings();
            }
            // Gesture
            if (Settings.Gesture != null)
            {
                if (Settings.Gesture.IsEnableMultiTouchMode)
                {
                    ToggleSwitchEnableMultiTouchMode.IsOn = true;
                }
                else
                {
                    ToggleSwitchEnableMultiTouchMode.IsOn = false;
                }
                if (Settings.Gesture.IsEnableTwoFingerZoom)
                {
                    ToggleSwitchEnableTwoFingerZoom.IsOn = true;
                    BoardToggleSwitchEnableTwoFingerZoom.IsOn = true;
                }
                else
                {
                    ToggleSwitchEnableTwoFingerZoom.IsOn = false;
                    BoardToggleSwitchEnableTwoFingerZoom.IsOn = false;
                }
                if (Settings.Gesture.IsEnableTwoFingerTranslate)
                {
                    ToggleSwitchEnableTwoFingerTranslate.IsOn = true;
                    BoardToggleSwitchEnableTwoFingerTranslate.IsOn = true;
                }
                else
                {
                    ToggleSwitchEnableTwoFingerTranslate.IsOn = false;
                    BoardToggleSwitchEnableTwoFingerTranslate.IsOn = false;
                }
                if (Settings.Gesture.IsEnableTwoFingerRotation)
                {
                    ToggleSwitchEnableTwoFingerRotation.IsOn = true;
                    BoardToggleSwitchEnableTwoFingerRotation.IsOn = true;
                }
                else
                {
                    ToggleSwitchEnableTwoFingerRotation.IsOn = false;
                    BoardToggleSwitchEnableTwoFingerRotation.IsOn = false;
                }
                if (Settings.Gesture.AutoSwitchTwoFingerGesture)
                {
                    ToggleSwitchAutoSwitchTwoFingerGesture.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoSwitchTwoFingerGesture.IsOn = false;
                }
                if (Settings.Gesture.IsEnableTwoFingerRotation)
                {
                    ToggleSwitchEnableTwoFingerRotation.IsOn = true;
                }
                else
                {
                    ToggleSwitchEnableTwoFingerRotation.IsOn = false;
                }
                if (Settings.Gesture.IsEnableTwoFingerRotationOnSelection)
                {
                    ToggleSwitchEnableTwoFingerRotationOnSelection.IsOn = true;
                }
                else
                {
                    ToggleSwitchEnableTwoFingerRotationOnSelection.IsOn = false;
                }
                if (Settings.Gesture.AutoSwitchTwoFingerGesture)
                {
                    if (Topmost)
                    {
                        ToggleSwitchEnableTwoFingerTranslate.IsOn = false;
                        BoardToggleSwitchEnableTwoFingerTranslate.IsOn = false;
                        Settings.Gesture.IsEnableTwoFingerTranslate = false;
                        if (!isInMultiTouchMode) ToggleSwitchEnableMultiTouchMode.IsOn = true;
                    }
                    else
                    {
                        ToggleSwitchEnableTwoFingerTranslate.IsOn = true;
                        BoardToggleSwitchEnableTwoFingerTranslate.IsOn = true;
                        Settings.Gesture.IsEnableTwoFingerTranslate = true;
                        if (isInMultiTouchMode) ToggleSwitchEnableMultiTouchMode.IsOn = false;
                    }
                }
                CheckEnableTwoFingerGestureBtnColorPrompt();
            }
            else
            {
                Settings.Gesture = new Gesture();
            }
            // Canvas
            if (Settings.Canvas != null)
            {
                drawingAttributes.Height = Settings.Canvas.InkWidth;
                drawingAttributes.Width = Settings.Canvas.InkWidth;

                InkWidthSlider.Value = Settings.Canvas.InkWidth * 2;
                BoardInkWidthSlider.Value = Settings.Canvas.InkWidth * 2;
                InkAlphaSlider.Value = Settings.Canvas.InkAlpha;
                BoardInkAlphaSlider.Value = Settings.Canvas.InkAlpha;

                ComboBoxHyperbolaAsymptoteOption.SelectedIndex = (int)Settings.Canvas.HyperbolaAsymptoteOption;

                if (Settings.Canvas.UsingWhiteboard)
                {
                    GridBackgroundCover.Background = new SolidColorBrush(StringToColor("#FFF2F2F2"));
                    lastBoardInkColor = 0;
                }
                else
                {
                    GridBackgroundCover.Background = new SolidColorBrush(StringToColor("#FF1F1F1F"));
                    lastBoardInkColor = 5;
                }

                if (Settings.Canvas.IsShowCursor)
                {
                    ToggleSwitchShowCursor.IsOn = true;
                    inkCanvas.ForceCursor = true;
                }
                else
                {
                    ToggleSwitchShowCursor.IsOn = false;
                    inkCanvas.ForceCursor = false;
                }

                ComboBoxPenStyle.SelectedIndex = Settings.Canvas.InkStyle;
                BoardComboBoxPenStyle.SelectedIndex = Settings.Canvas.InkStyle;

                ComboBoxEraserSize.SelectedIndex = Settings.Canvas.EraserSize;

                if (Settings.Canvas.HideStrokeWhenSelecting)
                {
                    ToggleSwitchHideStrokeWhenSelecting.IsOn = true;
                }
                else
                {
                    ToggleSwitchHideStrokeWhenSelecting.IsOn = false;
                }
            }
            else
            {
                Settings.Canvas = new Canvas();
            }
            // Advanced
            if (Settings.Advanced != null)
            {
                TouchMultiplierSlider.Value = Settings.Advanced.TouchMultiplier;
                FingerModeBoundsWidthSlider.Value = Settings.Advanced.FingerModeBoundsWidth;
                NibModeBoundsWidthSlider.Value = Settings.Advanced.NibModeBoundsWidth;
                if (Settings.Advanced.IsLogEnabled)
                {
                    ToggleSwitchIsLogEnabled.IsOn = true;
                }
                else
                {
                    ToggleSwitchIsLogEnabled.IsOn = false;
                }
                if (Settings.Advanced.IsSecondConfimeWhenShutdownApp)
                {
                    ToggleSwitchIsSecondConfimeWhenShutdownApp.IsOn = true;
                }
                else
                {
                    ToggleSwitchIsSecondConfimeWhenShutdownApp.IsOn = false;
                }
                if (Settings.Advanced.EraserBindTouchMultiplier)
                {
                    ToggleSwitchEraserBindTouchMultiplier.IsOn = true;
                }
                else
                {
                    ToggleSwitchEraserBindTouchMultiplier.IsOn = false;
                }

                if (Settings.Advanced.IsSpecialScreen)
                {
                    ToggleSwitchIsSpecialScreen.IsOn = true;
                }
                else
                {
                    ToggleSwitchIsSpecialScreen.IsOn = false;
                }
                TouchMultiplierSlider.Visibility = ToggleSwitchIsSpecialScreen.IsOn ? Visibility.Visible : Visibility.Collapsed;

                ToggleSwitchIsQuadIR.IsOn = Settings.Advanced.IsQuadIR;

                ToggleSwitchIsEnableEdgeGestureUtil.IsOn = Settings.Advanced.IsEnableEdgeGestureUtil;
                if (Settings.Advanced.IsEnableEdgeGestureUtil)
                {
                    if (OSVersion.GetOperatingSystem() >= OSVersionExtension.OperatingSystem.Windows10) EdgeGestureUtil.DisableEdgeGestures(new WindowInteropHelper(this).Handle, true);
                }
            }
            else
            {
                Settings.Advanced = new Advanced();
            }
            // InkToShape
            if (Settings.InkToShape != null)
            {
                if (Settings.InkToShape.IsInkToShapeEnabled)
                {
                    ToggleSwitchEnableInkToShape.IsOn = true;
                }
                else
                {
                    ToggleSwitchEnableInkToShape.IsOn = false;
                }
            }
            else
            {
                Settings.InkToShape = new InkToShape();
            }
            // RandSettings
            if (Settings.RandSettings != null)
            {
            }
            else
            {
                Settings.RandSettings = new RandSettings();
            }
            // Automation
            if (Settings.Automation != null)
            {
                StartOrStoptimerCheckAutoFold();
                if (Settings.Automation.IsAutoFoldInEasiNote)
                {
                    ToggleSwitchAutoFoldInEasiNote.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoFoldInEasiNote.IsOn = false;
                }
                if (Settings.Automation.IsAutoFoldInEasiCamera)
                {
                    ToggleSwitchAutoFoldInEasiCamera.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoFoldInEasiCamera.IsOn = false;
                }
                if (Settings.Automation.IsAutoFoldInEasiNote3C)
                {
                    ToggleSwitchAutoFoldInEasiNote3C.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoFoldInEasiNote3C.IsOn = false;
                }
                if (Settings.Automation.IsAutoFoldInSeewoPincoTeacher)
                {
                    ToggleSwitchAutoFoldInSeewoPincoTeacher.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoFoldInSeewoPincoTeacher.IsOn = false;
                }
                if (Settings.Automation.IsAutoFoldInHiteTouchPro)
                {
                    ToggleSwitchAutoFoldInHiteTouchPro.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoFoldInHiteTouchPro.IsOn = false;
                }
                if (Settings.Automation.IsAutoFoldInHiteCamera)
                {
                    ToggleSwitchAutoFoldInHiteCamera.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoFoldInHiteCamera.IsOn = false;
                }
                if (Settings.Automation.IsAutoFoldInWxBoardMain)
                {
                    ToggleSwitchAutoFoldInWxBoardMain.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoFoldInWxBoardMain.IsOn = false;
                }
                if (Settings.Automation.IsAutoFoldInOldZyBoard)
                {
                    ToggleSwitchAutoFoldInOldZyBoard.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoFoldInOldZyBoard.IsOn = false;
                }
                if (Settings.Automation.IsAutoFoldInMSWhiteboard)
                {
                    ToggleSwitchAutoFoldInMSWhiteboard.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoFoldInMSWhiteboard.IsOn = false;
                }
                if (Settings.Automation.IsAutoFoldInPPTSlideShow)
                {
                    ToggleSwitchAutoFoldInPPTSlideShow.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoFoldInPPTSlideShow.IsOn = false;
                }
                if (Settings.Automation.IsAutoKillEasiNote || Settings.Automation.IsAutoKillPptService)
                {
                    timerKillProcess.Start();
                }
                else
                {
                    timerKillProcess.Stop();
                }

                if (Settings.Automation.IsAutoKillEasiNote)
                {
                    ToggleSwitchAutoKillEasiNote.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoKillEasiNote.IsOn = false;
                }
                if (Settings.Automation.IsAutoKillPptService)
                {
                    ToggleSwitchAutoKillPptService.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoKillPptService.IsOn = false;
                }
                if (Settings.Automation.IsAutoSaveStrokesAtClear)
                {
                    ToggleSwitchAutoSaveStrokesAtClear.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoSaveStrokesAtClear.IsOn = false;
                }
                if (Settings.Automation.IsSaveScreenshotsInDateFolders)
                {
                    ToggleSwitchSaveScreenshotsInDateFolders.IsOn = true;
                }
                else
                {
                    ToggleSwitchSaveScreenshotsInDateFolders.IsOn = false;
                }
                if (Settings.Automation.IsAutoSaveStrokesAtScreenshot)
                {
                    ToggleSwitchAutoSaveStrokesAtScreenshot.IsOn = true;
                }
                else
                {
                    ToggleSwitchAutoSaveStrokesAtScreenshot.IsOn = false;
                }
                SideControlMinimumAutomationSlider.Value = Settings.Automation.MinimumAutomationStrokeNumber;

                AutoSavedStrokesLocation.Text = Settings.Automation.AutoSavedStrokesLocation;
                ToggleSwitchAutoDelSavedFiles.IsOn = Settings.Automation.AutoDelSavedFiles;
                ComboBoxAutoDelSavedFilesDaysThreshold.Text = Settings.Automation.AutoDelSavedFilesDaysThreshold.ToString();
            }
            else
            {
                Settings.Automation = new Automation();
            }
            // auto align
            if (BtnPPTSlideShowEnd.Visibility == Visibility.Visible)
            {
                ViewboxFloatingBarMarginAnimation(60);
            }
            else
            {
                ViewboxFloatingBarMarginAnimation(100);
            }
        }
    }
}