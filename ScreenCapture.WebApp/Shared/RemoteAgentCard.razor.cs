using Core.Dtos;
using Microsoft.AspNetCore.Components;
using ScreenCapture.WebApp.Domain;
using ScreenCapture.WebApp.Services.Interfaces;

namespace ScreenCapture.WebApp.Shared;

// TODO options page
public partial class RemoteAgentCard : IDisposable
{
    #region Parameters
    [Inject] public IDtoFactory? Factory { get; set; }
    [Parameter] public IRemoteAgent? Agent { get; set; }
    [CascadingParameter] public NotificationComponent? Notification { get; set; }
    #endregion

    #region LifeTime
    protected override void OnAfterRender(bool firstRender)
    {
        if (Agent != null)
        {
            Agent.OnStatusChanged += UpdateAfterChange;
        }

        base.OnAfterRender(firstRender);
    }

    private async Task UpdateAfterChange()
    {
        await InvokeAsync(StateHasChanged);
    }
    #endregion

    #region Graphic support function
    private bool IsDisabled()
    {
        if (Agent == null)
        {
            return true;
        }

        return Agent.Status == RemoteAgentStatus.Offline || Agent.Status == RemoteAgentStatus.Error ? true : false;
    }

    private string GetStatusColor()
    {
        switch (Agent?.Status)
        {
            case (RemoteAgentStatus.Online):
                return "text-success";
            case (RemoteAgentStatus.Error): // yellow since red is already used to identify active recording
                return "text-warning";
            case (RemoteAgentStatus.Recording):
                return "text-danger";
            case (RemoteAgentStatus.Offline):
            default:
                return "text-dark";
        }
    }
    #endregion

    #region ButtonEventHandlers
    private async Task OnStartRecordingButtonClickAsync()
    {
        if (Agent == null || Factory == null)
        {
            return;
        }

        var dto = await Factory.CreateSettingDtoAsync<RecordingOptions>();
        var report = await Agent.StartRecordingAsync(dto);
        if (Notification == null)
        {
            return;
        }

        if (!report.IsSuccessful)
        {
            await Notification.RaiseNotification($"Starting the recording operation has failed with error: [{report.StatusCode}]", NotificationLevel.Error);
        }
    }

    private async Task OnStopRecordingButtonClickAsync()
    {
        if (Agent == null)
        {
            return;
        }

        var report = await Agent.StopRecordingAsync();
        if (Notification == null)
        {
            return;
        }

        if (report.IsSuccessful)
        {
            if (string.IsNullOrWhiteSpace(report.NewFileName))
            {
                await Notification.RaiseNotification($"The operation has been completed without errors but the video name has not been retrieved.", NotificationLevel.Warning);
            }

            await Notification.RaiseNotification($"Video file '{report.NewFileName}' has been successfully created.", NotificationLevel.Success);
        }
        else
        {
            await Notification.RaiseNotification($"The screen capture operation has failed with error: [{report.StatusCode}]", NotificationLevel.Error);
        }
    }

    private async Task OnTakeScreenshotButtonClickAsync()
    {
        if (Agent == null || Factory == null)
        {
            return;
        }

        var dto = await Factory.CreateSettingDtoAsync<ScreenshotOptions>();
        var report = await Agent.TakeScreenshotAsync(dto);
        if (Notification == null)
        {
            return;
        }

        if (report.IsSuccessful)
        {
            if (string.IsNullOrWhiteSpace(report.NewFileName))
            {
                await Notification.RaiseNotification($"The operation has been completed without errors but the screenshot name has not been retrieved.", NotificationLevel.Warning);
            }

            await Notification.RaiseNotification($"Screenshot file '{report.NewFileName}' has been successfully created.", NotificationLevel.Success);
        }
        else
        {
            await Notification.RaiseNotification($"The screenshot operation has failed with error: [{report.StatusCode}]", NotificationLevel.Error);
        }
    }
    #endregion

    public void Dispose()
    {
        if (Agent != null)
        {
            Agent.OnStatusChanged -= UpdateAfterChange;
        }
    }
}
