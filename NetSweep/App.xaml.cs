using System;
using System.Windows;
using System.Windows.Threading;
using NetSweep.Helpers;
using NetSweep.Views;

namespace NetSweep;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Keep the app alive while no window is open (welcome dialog -> main window gap).
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // Headless screenshot mode for CI (see .github/workflows/screenshot.yml): skips the
        // welcome dialog, renders MainWindow off-screen to a PNG, then exits.
        if (e.Args.Length >= 2 && e.Args[0] == "--screenshot")
        {
            string screenshotPath = e.Args[1];
            var main = new MainWindow();
            MainWindow = main;
            main.Show();
            main.ContentRendered += (_, _) => Dispatcher.BeginInvoke(new Action(() =>
            {
                ScreenshotHelper.Capture(main, screenshotPath);
                Shutdown();
            }), DispatcherPriority.ApplicationIdle);
            return;
        }

        var welcome = new WelcomeWindow();
        bool? proceed = welcome.ShowDialog();

        if (proceed == true)
        {
            var main = new MainWindow();
            MainWindow = main;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            main.Show();
        }
        else
        {
            Shutdown();
        }
    }
}
