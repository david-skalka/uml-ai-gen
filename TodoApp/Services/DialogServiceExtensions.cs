using TodoApp.ViewModels.Dialogs;
using Prism.Dialogs;

namespace TodoApp.Services;

public static class DialogServiceExtensions
{
    public static Task<IDialogResult> ShowNotificationAsync(
        this IDialogService dialogService,
        string title,
        string message,
        List<DialogButton> buttons)
    {
        var parameters = new DialogParameters
        {
            { "title", title },
            { "message", message },
            { "buttons", buttons }
        };
        return dialogService.ShowDialogAsync("notification", parameters);
    }

    public static Task<IDialogResult> ShowValidationErrorsAsync(
        this IDialogService dialogService,
        IReadOnlyList<string> errors)
    {
        var parameters = new DialogParameters
        {
            { "title", "Validation Error" },
            { "errors", errors.ToList() },
            { "buttons", new List<DialogButton> { new("Ok", ButtonResult.OK) } }
        };
        return dialogService.ShowDialogAsync("notification", parameters);
    }
}
