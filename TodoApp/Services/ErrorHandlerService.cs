using Microsoft.Extensions.Logging;
using TodoApp.Api;
using TodoApp.Utils;
using TodoApp.ViewModels.Dialogs;

namespace TodoApp.Services;

public class ErrorHandlerService(IAppDialogService dialogService, ILogger<ErrorHandlerService> logger)
    : IErrorHandlerService
{
    public async Task Handle(Exception ex)
    {
        if (ex is ApiException { StatusCode: 400 } apiException)
        {
            logger.LogWarning("Validation error: {Response}", apiException.Response);
            await dialogService.ShowValidationErrorsAsync(apiException.GetValidationErrors());
            return;
        }

        logger.LogError(ex, "Unhandled error");

        var buttons = new List<DialogButton> { new("Ok", ButtonResult.OK), new("Exit", ButtonResult.Abort) };
        var result = await dialogService.ShowNotificationAsync(ex.GetType().Name, ex.ToString(), buttons);

        if (result.Result == ButtonResult.Abort)
            Environment.Exit(0);
    }
}
