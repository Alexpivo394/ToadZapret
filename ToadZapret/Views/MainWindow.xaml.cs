using System.Windows;
using ToadZapret.ViewModels;
using Wpf.Ui.Controls;

namespace ToadZapret.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        
        Closing += MainWindow_Closing;
    }
    
    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}