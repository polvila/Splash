using System;
using UnityEngine;

public class AuthenticationService
{
    public void DeviceAuthentication(Action onComplete)
    {
        new GameSparks.Api.Requests.DeviceAuthenticationRequest().Send((response) =>
        {
            if (!response.HasErrors)
            {
                Debug.Log("Device Authenticated...");
                onComplete?.Invoke();
            }
            else
            {
                Debug.Log("Error Authenticating Device...");
            }
        });
    }
}