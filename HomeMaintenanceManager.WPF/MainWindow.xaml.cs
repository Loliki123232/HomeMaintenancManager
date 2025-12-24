using HomeMaintenanceManager.Core.Models;
using HomeMaintenanceManager.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HomeMaintenanceManager.WPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TaskCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is MaintenanceTask task)
        {
            var viewModel = DataContext as MainViewModel;
            viewModel?.EditTaskCommand.Execute(task);
        }
    }
}