using System;

public interface IScreenManager
{
    void ShowScreen(string screenName);
    void ShowSpinner(string text = "");
    void HideSpinner();
}