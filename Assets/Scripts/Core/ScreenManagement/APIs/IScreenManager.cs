using System;

public interface IScreenManager
{
    void ShowScreen(string screenName);
    void ShowPopup(string popupName);
    void ShowSpinner(string text = "");
    void HideSpinner();
}