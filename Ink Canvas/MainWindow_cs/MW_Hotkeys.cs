using System.Windows;
using System.Windows.Input;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        private void RegisterGlobalHotkeys()
        {
            Hotkey.Regist(this, HotkeyModifiers.MOD_SHIFT, Key.Escape, HotKey_ExitPPTSlideShow);
            Hotkey.Regist(this, HotkeyModifiers.MOD_CONTROL, Key.E, HotKey_Clear);
            Hotkey.Regist(this, HotkeyModifiers.MOD_ALT, Key.C, HotKey_Capture);
            Hotkey.Regist(this, HotkeyModifiers.MOD_ALT, Key.V, HotKey_Hide);
            Hotkey.Regist(this, HotkeyModifiers.MOD_ALT, Key.D, HotKey_DrawTool);
            Hotkey.Regist(this, HotkeyModifiers.MOD_ALT, Key.Q, HotKey_QuitDrawMode);
            Hotkey.Regist(this, HotkeyModifiers.MOD_ALT, Key.B, HotKey_Board);
        }

        private void HotKey_ExitPPTSlideShow()
        {
            if(BtnPPTSlideShowEnd.Visibility == Visibility.Visible)
            {
                BtnPPTSlideShowEnd_Click(null, null);
            }
        }

        private void HotKey_Clear()
        {
            SymbolIconDelete_MouseUp(null, null);
        }

        private void HotKey_Capture()
        {
            SaveScreenShotToDesktop();
        }
        
        private void HotKey_Hide()
        {
            SymbolIconEmoji_MouseUp(null, null);
        }

        private void HotKey_DrawTool()
        {
            PenIcon_Click(null, null);
        }

        private void HotKey_QuitDrawMode()
        {
            if (currentMode != 0)
            {
                ImageBlackboard_Click(null, null);
            }
            CursorIcon_Click(null, null);
        }

        private void HotKey_Board()
        {
            ImageBlackboard_Click(null, null);
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (BtnPPTSlideShowEnd.Visibility != Visibility.Visible || currentMode != 0) return;
            if (e.Delta >= 120)
            {
                BtnPPTSlidesUp_Click(null, null);
            }
            else if (e.Delta <= -120)
            {
                BtnPPTSlidesDown_Click(null, null);
            }
        }

        private void Main_Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (BtnPPTSlideShowEnd.Visibility != Visibility.Visible || currentMode != 0) return;

            if (e.Key == Key.Down || e.Key == Key.PageDown || e.Key == Key.Right || e.Key == Key.N || e.Key == Key.Space)
            {
                BtnPPTSlidesDown_Click(null, null);
            }
            if (e.Key == Key.Up || e.Key == Key.PageUp || e.Key == Key.Left || e.Key == Key.P)
            {
                BtnPPTSlidesUp_Click(null, null);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                KeyExit(null, null);
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void HotKey_Undo(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SymbolIconUndo_Click(null, null);
            }
            catch { }
        }

        private void HotKey_Redo(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SymbolIconRedo_Click(null, null);
            }
            catch { }
        }

        private void KeyExit(object sender, ExecutedRoutedEventArgs e)
        {
            BtnPPTSlideShowEnd_Click(null, null);
        }

        private void KeyChangeToSelect(object sender, ExecutedRoutedEventArgs e)
        {
            if (StackPanelCanvasControls.Visibility == Visibility.Visible)
            {
                SymbolIconSelect_Click(null, null);
            }
        }

        private void KeyChangeToEraser(object sender, ExecutedRoutedEventArgs e)
        {
            if (StackPanelCanvasControls.Visibility == Visibility.Visible)
            {
                if (Eraser_Icon.Background != null)
                {
                    EraserIconByStrokes_Click(null, null);
                }
                else
                {
                    EraserIcon_Click(null, null);
                }
            }
        }

        private void KeyDrawLine(object sender, ExecutedRoutedEventArgs e)
        {
            if (StackPanelCanvasControls.Visibility == Visibility.Visible)
            {
                BtnDrawLine_Click(lastMouseDownSender, null);
            }
        }
    }
}
