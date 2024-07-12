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
            return (IsSystemThemeLight()) ? "Light" : "Dark";
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
                ResourceDictionary rd1 = new ResourceDictionary() { Source = new Uri("Resources/Styles/Light.xaml", UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(rd1);

                ResourceDictionary rd2 = new ResourceDictionary() { Source = new Uri("Resources/DrawShapeImageDictionary.xaml", UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(rd2);

                ResourceDictionary rd3 = new ResourceDictionary() { Source = new Uri("Resources/SeewoImageDictionary.xaml", UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(rd3);

                ResourceDictionary rd4 = new ResourceDictionary() { Source = new Uri("Resources/IconImageDictionary.xaml", UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(rd4);

                if (Settings.Canvas.UsingWhiteboard)
                {
                    ResourceDictionary rd5 = new ResourceDictionary() { Source = new Uri("Resources/Styles/Light-Board.xaml", UriKind.Relative) };
                    Application.Current.Resources.MergedDictionaries.Add(rd5);
                }
                else
                {
                    ResourceDictionary rd5 = new ResourceDictionary() { Source = new Uri("Resources/Styles/Dark-Board.xaml", UriKind.Relative) };
                    Application.Current.Resources.MergedDictionaries.Add(rd5);
                }

                ResourceDictionary rd6 = new ResourceDictionary() { Source = new Uri("Resources/BoardDrawShapeImageDictionary.xaml", UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(rd6);

                ThemeManager.SetRequestedTheme(window, ElementTheme.Light);
                FloatBarForegroundColor = (Color)Application.Current.FindResource("FloatBarForegroundColor");
                BoardBarForegroundColor = (Color)Application.Current.FindResource("BoardBarForegroundColor");
            }
            else if (theme == "Dark")
            {
                ResourceDictionary rd1 = new ResourceDictionary() { Source = new Uri("Resources/Styles/Dark.xaml", UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(rd1);

                ResourceDictionary rd2 = new ResourceDictionary() { Source = new Uri("Resources/DrawShapeImageDictionary.xaml", UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(rd2);

                ResourceDictionary rd3 = new ResourceDictionary() { Source = new Uri("Resources/SeewoImageDictionary.xaml", UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(rd3);

                ResourceDictionary rd4 = new ResourceDictionary() { Source = new Uri("Resources/IconImageDictionary.xaml", UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(rd4);

                if (Settings.Canvas.UsingWhiteboard)
                {
                    ResourceDictionary rd5 = new ResourceDictionary() { Source = new Uri("Resources/Styles/Light-Board.xaml", UriKind.Relative) };
                    Application.Current.Resources.MergedDictionaries.Add(rd5);
                }
                else
                {
                    ResourceDictionary rd5 = new ResourceDictionary() { Source = new Uri("Resources/Styles/Dark-Board.xaml", UriKind.Relative) };
                    Application.Current.Resources.MergedDictionaries.Add(rd5);
                }

                ResourceDictionary rd6 = new ResourceDictionary() { Source = new Uri("Resources/BoardDrawShapeImageDictionary.xaml", UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(rd6);

                ThemeManager.SetRequestedTheme(window, ElementTheme.Dark);
                FloatBarForegroundColor = (Color)Application.Current.FindResource("FloatBarForegroundColor");
                BoardBarForegroundColor = (Color)Application.Current.FindResource("BoardBarForegroundColor");
            }

            if (!Settings.Appearance.IsColorfulViewboxFloatingBar) // 还原浮动工具栏背景色
            {
                EnableTwoFingerGestureBorder.Background = BorderDrawShape.Background;
                BorderFloatingBarMainControls.Background = BorderDrawShape.Background;
                BorderFloatingBarMoveControls.Background = BorderDrawShape.Background;
                BorderFloatingBarExitPPTBtn.Background = BorderDrawShape.Background;
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