using Ink_Canvas.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        bool isFloatingBarFolded = false, isFloatingBarChangingHideMode = false;

        private async void FoldFloatingBar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender == null)
            {
                foldFloatingBarByUser = false;
            }
            else
            {
                foldFloatingBarByUser = true;
            }
            unfoldFloatingBarByUser = false;

            if (isFloatingBarChangingHideMode) return;

            await Dispatcher.InvokeAsync(() =>
            {
                isFloatingBarChangingHideMode = true;
                isFloatingBarFolded = true;
                if (currentMode != 0) ImageBlackboard_MouseUp(null, null);
                if (StackPanelCanvasControls.Visibility == Visibility.Visible)
                {
                    if (foldFloatingBarByUser && inkCanvas.Strokes.Count > 2)
                    {
                        ShowNotification("正在清空墨迹并收纳至侧边栏，可进入批注模式后通过【撤销】功能来恢复原先墨迹。");
                    }
                }
                lastBorderMouseDownObject = sender;
                CursorWithDelIcon_Click(sender, null);
                SidePannelMarginAnimation(-16);
            });

            await Task.Delay(500);

            await Dispatcher.InvokeAsync(() =>
            {
                PPTNavigationBottomLeft.Visibility = Visibility.Collapsed;
                PPTNavigationBottomRight.Visibility = Visibility.Collapsed;
                PPTNavigationSidesLeft.Visibility = Visibility.Collapsed;
                PPTNavigationSidesRight.Visibility = Visibility.Collapsed;
                ViewboxFloatingBarMarginAnimation(-60);
                HideSubPanels("cursor");
                SidePannelMarginAnimation(-16);
            });
            isFloatingBarChangingHideMode = false;
        }

        private async void UnFoldFloatingBar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender == null || StackPanelPPTControls.Visibility == Visibility.Visible)
            {
                unfoldFloatingBarByUser = false;
            }
            else
            {
                unfoldFloatingBarByUser = true;
            }
            foldFloatingBarByUser = false;

            if (isFloatingBarChangingHideMode) return;

            await Dispatcher.InvokeAsync(() =>
            {
                isFloatingBarChangingHideMode = true;
                isFloatingBarFolded = false;
            });

            await Task.Delay(500);

            await Dispatcher.InvokeAsync(() =>
            {
                if (StackPanelPPTControls.Visibility == Visibility.Visible)
                {
                    if (Settings.PowerPointSettings.IsShowBottomPPTNavigationPanel)
                    {
                        AnimationsHelper.ShowWithScaleFromBottom(PPTNavigationBottomLeft);
                        AnimationsHelper.ShowWithScaleFromBottom(PPTNavigationBottomRight);
                    }
                    if (Settings.PowerPointSettings.IsShowSidePPTNavigationPanel)
                    {
                        AnimationsHelper.ShowWithScaleFromLeft(PPTNavigationSidesLeft);
                        AnimationsHelper.ShowWithScaleFromRight(PPTNavigationSidesRight);
                    }
                }
                if (BtnPPTSlideShowEnd.Visibility == Visibility.Visible)
                {
                    ViewboxFloatingBarMarginAnimation(60);
                }
                else
                {
                    ViewboxFloatingBarMarginAnimation(100);
                }
                SidePannelMarginAnimation(-40);
            });

            isFloatingBarChangingHideMode = false;
        }

        private async void SidePannelMarginAnimation(int MarginFromEdge) // Possible value: -40, -16
        {
            await Dispatcher.InvokeAsync(() =>
            {
                if (MarginFromEdge == -16) LeftSidePanel.Visibility = Visibility.Visible;

                ThicknessAnimation LeftSidePanelmarginAnimation = new ThicknessAnimation
                {
                    Duration = TimeSpan.FromSeconds(0.3),
                    From = LeftSidePanel.Margin,
                    To = new Thickness(MarginFromEdge, 0, 0, -150)
                };
                ThicknessAnimation RightSidePanelmarginAnimation = new ThicknessAnimation
                {
                    Duration = TimeSpan.FromSeconds(0.3),
                    From = RightSidePanel.Margin,
                    To = new Thickness(0, 0, MarginFromEdge, -150)
                };

                LeftSidePanel.BeginAnimation(FrameworkElement.MarginProperty, LeftSidePanelmarginAnimation);
                RightSidePanel.BeginAnimation(FrameworkElement.MarginProperty, RightSidePanelmarginAnimation);
            });

            await Task.Delay(600);

            await Dispatcher.InvokeAsync(() =>
            {
                LeftSidePanel.Margin = new Thickness(MarginFromEdge, 0, 0, -150);
                RightSidePanel.Margin = new Thickness(0, 0, MarginFromEdge, -150);

                if (MarginFromEdge == -40) LeftSidePanel.Visibility = Visibility.Collapsed;
            });
            isFloatingBarChangingHideMode = false;
        }
    }
}