using Ink_Canvas.Helpers;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Ink_Canvas
{
    public partial class MainWindow : Window
    {
        int lastNotificationShowTime = 0;
        int notificationShowTime = 2500;

        public static void ShowNewMessage(string notice, bool isShowImmediately = true)
        {
            (Application.Current?.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow)?.ShowNotificationAsync(notice, isShowImmediately);
        }

        public async Task ShowNotificationAsync(string notice, bool isShowImmediately = true)
        {
            try
            {
                TextBlockNotice.Text = notice;
                AnimationsHelper.ShowWithSlideFromBottomAndFade(GridNotifications);
                await Task.Delay(2000);
                AnimationsHelper.HideWithSlideAndFade(GridNotifications);
            }
            catch { }
        }
    }
}
