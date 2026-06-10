using System.Windows;
using NetSweep.Views;

namespace NetSweep;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Keep the app alive while no window is open (welcome dialog -> main window gap).
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

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
