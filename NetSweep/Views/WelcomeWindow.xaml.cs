using System.Windows;
using NetSweep.Helpers;

namespace NetSweep.Views;

public partial class WelcomeWindow : Window
{
    public WelcomeWindow()
    {
        InitializeComponent();
    }

    private void Start_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void LanguageToggle_Click(object sender, RoutedEventArgs e) => Localization.Instance.Toggle();
}
