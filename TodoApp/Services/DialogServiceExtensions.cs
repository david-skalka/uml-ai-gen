using TodoApp.ViewModels.Dialogs;
using TodoApp.Views.Dialogs;

namespace TodoApp.Services;

public static class DialogServiceExtensions
{
    public static Task<IDialogResult> ShowNotificationAsync(
        this IAppDialogService dialogService,
        string title,
        string message,
        List<DialogButton> buttons)
    {
        var parameters = new DialogParameters { { "title", title }, { "message", message }, { "buttons", buttons } };
        return dialogService.ShowAsync<DialogNotificationView, DialogNotificationViewModel>(parameters);
    }

    public static Task<IDialogResult> ShowValidationErrorsAsync(
        this IAppDialogService dialogService,
        IReadOnlyList<string> errors)
    {
        var parameters = new DialogParameters
        {
            { "title", "Validation Error" },
            { "errors", errors.ToList() },
            { "buttons", new List<DialogButton> { new("Ok", ButtonResult.OK) } }
        };
        return dialogService.ShowAsync<DialogNotificationView, DialogNotificationViewModel>(parameters);
    }
}
