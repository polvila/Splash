using System;
using GameSparks.Core;
using UnityEngine;

namespace Core.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        public void InitService(Action onComplete, Action onError)
        {
            GS.GameSparksAvailable += (isAvailable) =>
            {
                if (isAvailable)
                {
                    Debug.Log("GameSparks Connected...");
                    onComplete?.Invoke();
                }
                else
                {
                    Debug.Log("GameSparks Disconnected...");
                    onError?.Invoke();
                }
            };
        }

        public void DeviceAuthentication(Action onComplete, Action onError)
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
                    onError?.Invoke();
                }
            });
        }
    }
}