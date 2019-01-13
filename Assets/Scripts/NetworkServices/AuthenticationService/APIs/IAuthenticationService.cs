using System;

public interface IAuthenticationService
{
    void DeviceAuthentication(Action onComplete);
}
