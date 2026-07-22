using Avalonia.Controls;

namespace TodoApp.UrsaPrism;

public static class UrsaDialogServiceExtension
{
    private const string UrsaDialogViewPrefix = "URSA_DIALOG_VIEW_";

    private static readonly Dictionary<string, Func<Control>> ViewFactories = new();

    public static void RegisterUrsaDialogService(this IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IUrsaOverlayDialogService, UrsaOverlayDialogService>();
    }

    public static void RegisterUrsaDialogView<T>(this IContainerRegistry _, string name)
        where T : Control, new()
    {
        ViewFactories[UrsaDialogViewPrefix + name] = () => new T();
    }

    internal static Control CreateView(string viewName) => ViewFactories[UrsaDialogViewPrefix + viewName]();
}
