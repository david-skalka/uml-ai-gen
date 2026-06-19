using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;

namespace TodoAppTest.E2e.Utils.ControlsExtensions;

public static class TextBoxExtensions
{
    public static void TypeText(this TextBox textBox, string text)
    {
        textBox.TypeText(text, TopLevel.GetTopLevel(textBox)!);
    }

    private static void TypeText(this TextBox textBox, string text, TopLevel topLevel)
    {
        textBox.Focus();
        topLevel.KeyTextInput(text);
        Dispatcher.UIThread.RunJobs();
    }

    public static void ReplaceText(this TextBox textBox, string text)
    {
        textBox.Text = text;
        Dispatcher.UIThread.RunJobs();
    }
}