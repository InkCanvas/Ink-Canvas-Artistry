using Ink_Canvas.Helpers;
using System.Linq;
using System.Threading;
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

        private CancellationTokenSource ShowNotificationCancellationTokenSource = new CancellationTokenSource();

        public async void ShowNotificationAsync(string notice, bool isShowImmediately = true)
        {
            try
            {
                ShowNotificationCancellationTokenSource.Cancel();
                ShowNotificationCancellationTokenSource = new CancellationTokenSource();
                var token = ShowNotificationCancellationTokenSource.Token;

                TextBlockNotice.Text = notice;
                AnimationsHelper.ShowWithSlideFromBottomAndFade(GridNotifications);

                try
                {
                    await Task.Delay(2000, token);
                    AnimationsHelper.HideWithSlideAndFade(GridNotifications);
                }
                catch (TaskCanceledException) { }
            }
            catch { }
        }
    }
}
