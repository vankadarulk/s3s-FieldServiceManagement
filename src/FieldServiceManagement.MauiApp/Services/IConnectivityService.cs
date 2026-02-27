namespace FieldServiceManagement.MauiApp.Services;

public interface IConnectivityService
{
    bool IsConnected { get; }
    event EventHandler<bool> ConnectivityChanged;
}
