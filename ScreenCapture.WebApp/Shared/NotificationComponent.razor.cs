using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ScreenCapture.WebApp.Domain;

namespace ScreenCapture.WebApp.Shared;

public partial class NotificationComponent
{
    [Inject] public IJSRuntime? js { get; set; }
    private IJSObjectReference? module;

    [Parameter] public RenderFragment? ChildContent { get; set; }
    private NotificationLevel level;
    private string message = string.Empty;

    public async Task RaiseNotification(string notificationMessage, NotificationLevel notificationLevel)
    {
        message = notificationMessage;
        level = notificationLevel;
        StateHasChanged();

        if (js != null)
        {
            module ??= await js.InvokeAsync<IJSObjectReference>("import", "./Shared/NotificationComponent.razor.js");
            await module.InvokeVoidAsync("showToast");
        }
    }

    private string GetNotificationColor()
    {
        switch (level)
        {
            case NotificationLevel.Success:
                return "bg-success";
            case NotificationLevel.Warning:
                return "bg-warning";
            case NotificationLevel.Error:
                return "bg-danger";
            case NotificationLevel.Information:
                return "bg-info";
            default:
                return "bg-secondary";
        }
    }
}
