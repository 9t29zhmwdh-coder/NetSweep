using System.Windows;
using System.Windows.Input;
using NetSweep.ViewModels;

namespace NetSweep.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.OpenCommand.CanExecute(null))
            vm.OpenCommand.Execute(null);
    }
}
