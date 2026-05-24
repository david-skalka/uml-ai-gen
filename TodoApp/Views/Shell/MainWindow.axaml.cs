using System;
using Avalonia.Controls;

namespace TodoApp.Views.Shell;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (DataContext is IDisposable disposable)
            disposable.Dispose();
        base.OnClosing(e);
    }
}
