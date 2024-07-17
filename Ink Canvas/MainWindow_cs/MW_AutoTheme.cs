using Microsoft.Win32;
using iNKORE.UI.WPF.Modern;
using System;
using System.Windows;
using System.Windows.Media;
using Application = System.Windows.Application;
using System.Windows.Controls;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        public string GetMainWindowTheme()
        {
            if (currentMode != 0)
            {
                return Settings.Canvas.UsingWhiteboard ? "Light" : "Dark";
            }
            else
            {
                return (ThemeManager.GetRequestedTheme(window).ToString() == "Light") ? "Light" : "Dark";
            }
        }

        private void ComboBoxTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isLoaded) return;
            Settings.Appearance.Theme = ComboBoxTheme.SelectedIndex;
            SystemEvents_UserPreferenceChanged(null, null);
            SaveSettingsToFile();
        }
        Color FloatBarForegroundColor = Color.FromRgb(102, 102, 102);
        Color BoardBarForegroundColor = Color.FromRgb(102, 102, 102);
        private void SetTheme(string theme)
        {
            if (theme == "Light")
            {
                ResourceDictionary rd = new ResourceDictionary() { Source = new Uri("Resources/Styles/Light.xaml", UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(rd);

                if (Settings.Canvas.UsingWhiteboard)
                {
                    rd = new ResourceDictionary() { Source = new Uri("Resources/Styles/Light-Board.xaml", UriKind.Relative) };
                    Application.Current.Resources.MergedDictionaries.Add(rd);
                }
                else
                {
                    rd = new ResourceDictionary() { Source = new Uri("Resources/Styles/Dark-Board.xaml", UriKind.Relative) };
                    Application.Current.Resources.MergedDictionaries.Add(rd);
                }

                ThemeManager.SetRequestedTheme(window, ElementTheme.Light);
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                FloatBarForegroundColor = (Color)Application.Current.FindResource("FloatBarForegroundColor");
                BoardBarForegroundColor = (Color)Application.Current.FindResource("BoardBarForegroundColor");
            }
            else if (theme == "Dark")
            {
                ResourceDictionary rd = new ResourceDictionary() { Source = new Uri("Resources/Styles/Dark.xaml", UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(rd);

                if (Settings.Canvas.UsingWhiteboard)
                {
                    rd = new ResourceDictionary() { Source = new Uri("Resources/Styles/Light-Board.xaml", UriKind.Relative) };
                    Application.Current.Resources.MergedDictionaries.Add(rd);
                }
                else
                {
                    rd = new ResourceDictionary() { Source = new Uri("Resources/Styles/Dark-Board.xaml", UriKind.Relative) };
                    Application.Current.Resources.MergedDictionaries.Add(rd);
                }

                ThemeManager.SetRequestedTheme(window, ElementTheme.Dark);
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                FloatBarForegroundColor = (Color)Application.Current.FindResource("FloatBarForegroundColor");
                BoardBarForegroundColor = (Color)Application.Current.FindResource("BoardBarForegroundColor");
            }

            if (!Settings.Appearance.IsColorfulViewboxFloatingBar) // 还原浮动工具栏背景色
            {
                EnableTwoFingerGestureBorder.Background = BorderDrawShape.Background;
                BorderFloatingBarMainControls.Background = BorderDrawShape.Background;
                BorderFloatingBarMoveControls.Background = BorderDrawShape.Background;
                BtnPPTSlideShowEnd.Background = BorderDrawShape.Background;
            }
        }

        private void SystemEvents_UserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
        {
            switch (Settings.Appearance.Theme)
            {
                case 0:
                    SetTheme("Light");
                    break;
                case 1:
                    SetTheme("Dark");
                    break;
                case 2:
                    if (IsSystemThemeLight()) SetTheme("Light");
                    else SetTheme("Dark");
                    break;
            }
        }

        private bool IsSystemThemeLight()
        {
            bool light = false;
            try
            {
                RegistryKey registryKey = Registry.CurrentUser;
                RegistryKey themeKey = registryKey.OpenSubKey("software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize");
                int keyValue = 0;
                if (themeKey != null)
                {
                    keyValue = (int)themeKey.GetValue("SystemUsesLightTheme");
                }
                if (keyValue == 1) light = true;
            }
            catch { }
            return light;
        }
    }
}