using System;

namespace Core.Authentication.Mock
{
    public class AuthenticationServiceMock : IAuthenticationService
    {
        public void InitService(Action onComplete, Action onError)
        {
            onComplete?.Invoke();
        }

        public void DeviceAuthentication(Action onComplete, Action onError)
        {
            onComplete?.Invoke();
        }
    }
}