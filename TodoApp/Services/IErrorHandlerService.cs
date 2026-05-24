namespace TodoApp.Services;

public interface IErrorHandlerService
{
    Task Handle(Exception ex);
}
