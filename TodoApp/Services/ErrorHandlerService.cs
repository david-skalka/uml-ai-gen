using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Api;
using TodoApp.Utils;
using TodoApp.ViewModels.Dialogs;
using Microsoft.Extensions.Logging;
using Prism.Dialogs;

namespace TodoApp.Services;

public class ErrorHandlerService(IDialogService dialogService, ILogger<ErrorHandlerService> logger)
    : IErrorHandlerService
{
    public async Task Handle(Exception ex)
    {
        if (ex is ApiException apiException && apiException.StatusCode == 400)
        {
            logger.LogWarning("Validation error: {Response}", apiException.Response);
            await dialogService.ShowValidationErrorsAsync(apiException.GetValidationErrors());
            return;
        }

        logger.LogError(ex, "Unhandled error");

        var buttons = new List<DialogButton>
        {
            new("Ok", ButtonResult.OK),
            new("Exit", ButtonResult.Abort)
        };
        var result = await dialogService.ShowNotificationAsync(ex.GetType().Name, ex.ToString(), buttons);

        if (result.Result == ButtonResult.Abort)
            Environment.Exit(0);
    }
}
