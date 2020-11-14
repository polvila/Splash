using Core.ScreenManagement;
using Modules.Game.Scripts.Result;
using TMPro;
using UnityEngine;
using Zenject;

public class ResultView : PopupScreenView
{
    [SerializeField] private TMP_Text _resultText;
    [SerializeField] private GameObject _newRecordBanner;

    private IStateManager _stateManager;
    
    [Inject]
    void Init(IStateManager stateManager)
    {
        _stateManager = stateManager;
    }

    public override void SetParams(object paramsObject)
    {
        ResultParams resultParams = (ResultParams) paramsObject;
        
        _resultText.text = $"{resultParams.Result} points";
        _newRecordBanner.SetActive(resultParams.NewRecord);
        gameObject.SetActive(true);
    }

    public void OnPlayAgainClicked()
    {
        _stateManager.TriggerEvent(Event.SHOW_GAME);
        ClosePopup();
    }

    public void OnMenuClicked()
    {
        _stateManager.TriggerEvent(Event.SHOW_MAIN_MENU);
        ClosePopup();
    }

    public void OnLeaderboardsClicked()
    {
    }

    public void OnShareClicked()
    {
    }
}