using System;

namespace Core.ScreenManagement
{
    public interface IPopupScreenView
    {
        event Action<IPopupScreenView> PopupClosed;
        void SetParams(object paramsObject);
    }
}