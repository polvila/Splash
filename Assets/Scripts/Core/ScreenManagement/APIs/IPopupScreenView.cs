using System;
using UnityEngine;

namespace Core.ScreenManagement
{
    public interface IPopupScreenView
    {
        GameObject OwnerGameObject { get; }
        event Action<IPopupScreenView> PopupClosed;
        void SetParams(object paramsObject);
    }
}