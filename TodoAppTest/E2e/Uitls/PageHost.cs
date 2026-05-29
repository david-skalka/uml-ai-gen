using Avalonia.Controls;
using TodoApp.Views.Shell;

namespace TodoAppTest.E2e.Uitls;

public sealed record PageHost<TPage>(MainWindow Window, TPage Page) where TPage : Control;
