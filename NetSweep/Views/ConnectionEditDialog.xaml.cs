using System.Windows;
using Microsoft.Win32;
using NetSweep.Helpers;
using NetSweep.Models;
using NetSweep.Services;

namespace NetSweep.Views;

public partial class ConnectionEditDialog : Window
{
    private readonly StorageConnection _connection;

    public ConnectionEditDialog(StorageConnection connection)
    {
        InitializeComponent();
        _connection = connection;

        NameBox.Text = connection.Name;
        PathBox.Text = connection.Path;
        UserBox.Text = connection.Username;
        QuarantineBox.Text = connection.QuarantineFolder;
        PassBox.Password = CredentialService.Decrypt(connection.EncryptedPassword);
    }

    private void BrowsePath_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFolderDialog { Title = Localization.Instance.Get("ChooseFolderTitle") };
        if (dlg.ShowDialog() == true) PathBox.Text = dlg.FolderName;
    }

    private void BrowseQuarantine_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFolderDialog { Title = Localization.Instance.Get("ChooseQuarantineFolderTitle") };
        if (dlg.ShowDialog() == true) QuarantineBox.Text = dlg.FolderName;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PathBox.Text))
        {
            MessageBox.Show(Localization.Instance.Get("PathRequiredMessage"), Localization.Instance.Get("PathRequiredTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _connection.Name = string.IsNullOrWhiteSpace(NameBox.Text) ? PathBox.Text : NameBox.Text.Trim();
        _connection.Path = PathBox.Text.Trim();
        _connection.Username = UserBox.Text.Trim();
        _connection.QuarantineFolder = QuarantineBox.Text.Trim();
        _connection.EncryptedPassword = CredentialService.Encrypt(PassBox.Password);

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
