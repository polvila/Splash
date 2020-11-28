using Core.StateManager;
using UnityEngine;
using Zenject;
using Event = Core.StateManager.Event;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private GameObject _mainButtonsContainer;
    [SerializeField] private GameObject _aboutContainer;
    [SerializeField] private GameObject _madeBy;

    private IStateManager _stateManager;
    private IPlayerModel _playerModel;

    [Inject]
    private void Init(
        IStateManager stateManager,
        IPlayerModel playerModel)
    {
        _stateManager = stateManager;
        _playerModel = playerModel;
    }

    #region Main

    public void OnPlay()
    {
        _stateManager.TriggerEvent(_playerModel.FTUECompleted ? Event.SHOW_GAME : Event.SHOW_TUTORIAL);
    }

    public void OnAbout()
    {
        _aboutContainer.SetActive(true);
        _madeBy.SetActive(true);
        _mainButtonsContainer.SetActive(false);
    }

    public void OnTutorial()
    {
        _stateManager.TriggerEvent(Event.SHOW_TUTORIAL);
    }

    #endregion
    
    #region About

    public void OnBackToMain()
    {
        _aboutContainer.SetActive(false);
        _madeBy.SetActive(false);
        _mainButtonsContainer.SetActive(true);
    }

    public void OnRate()
    {
        var url = $"market://details?id={Application.identifier}";
        Debug.Log($"OnRate: {url}");
        Application.OpenURL(url);
    }

    public void OnShare()
    {
        new NativeShare().SetTitle("Splash!")
            .SetText($"Splash! is a brand new card game. I love to play it - why don’t you try it, too. market://details?id={Application.identifier}")
            .SetSubject("I've discovered an amazing mobile game!")
            .Share();
    }

    public void OnContact()
    {
        
    }

    #endregion

    
}