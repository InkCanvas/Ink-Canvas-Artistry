using iNKORE.UI.WPF.Modern;
using System;
using System.Windows;

namespace Ink_Canvas
{
    public partial class YesOrNoNotificationWindow : Window
    {
        private readonly Action _yesAction;
        private readonly Action _noAction;

        public YesOrNoNotificationWindow(string text, Action yesAction = null, Action noAction = null)
        {
            _yesAction = yesAction;
            _noAction = noAction;
            InitializeComponent();
            Label.Text = text;
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                if (mainWindow.GetMainWindowTheme() == "Light")
                {
                    ThemeManager.SetRequestedTheme(this, ElementTheme.Light);
                }
                else
                {
                    ThemeManager.SetRequestedTheme(this, ElementTheme.Dark);
                }
            }
        }

        private void ButtonYes_Click(object sender, RoutedEventArgs e)
        {
            if (_yesAction == null)
            {
                Close();
                return;
            }
            _yesAction.Invoke();
            Close();

        }

        private void ButtonNo_Click(object sender, RoutedEventArgs e)
        {
            if (_noAction == null)
            {
                Close();
                return;
            }
            _noAction.Invoke();
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.IsShowingRestoreHiddenSlidesWindow = false;
        }
    }
}