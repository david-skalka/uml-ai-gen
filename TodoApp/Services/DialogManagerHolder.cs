using ShadUI;

namespace TodoApp.Services;

public sealed class DialogManagerHolder
{
    public DialogManager Manager { get; set; } = new();
}
