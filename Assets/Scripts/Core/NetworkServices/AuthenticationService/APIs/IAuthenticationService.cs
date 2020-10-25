using System;

public interface IAuthenticationService
{
    void InitService(Action onComplete, Action onError = null);
    void DeviceAuthentication(Action onComplete, Action onError = null);
}
