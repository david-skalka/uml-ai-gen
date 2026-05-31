using Avalonia.Controls;

public interface IActiveDialogTracker
{
    Window? ActiveDialog { get; set; }
}

public class ActiveDialogTracker : IActiveDialogTracker
{
    public Window? ActiveDialog { get; set; }
}