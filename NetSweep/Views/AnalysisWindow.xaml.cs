using System.Windows;
using NetSweep.Models;
using NetSweep.ViewModels;

namespace NetSweep.Views;

public partial class AnalysisWindow : Window
{
    public AnalysisWindow(StorageConnection connection)
    {
        InitializeComponent();
        DataContext = new AnalysisViewModel(connection);
    }
}
