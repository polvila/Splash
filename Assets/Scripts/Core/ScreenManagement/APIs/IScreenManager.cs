namespace Core.ScreenManagement
{
    public interface IScreenManager
    {
        void ShowScreen(string screenName);
        void ShowPopup(string popupName, object paramsObject = null);
        void ShowSpinner(string text = "");
        void HideSpinner();
    }
}