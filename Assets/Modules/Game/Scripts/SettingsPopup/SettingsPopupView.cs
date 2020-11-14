using Core.ScreenManagement;
using Zenject;

public class SettingsPopupView : PopupScreenView
{
    private IAIManagerService _aiManagerService;
    private IStateManager _stateManager;

    [Inject]
    private void Init(IAIManagerService aiManagerService,
        IStateManager stateManager)
    {
        _aiManagerService = aiManagerService;
        _stateManager = stateManager;
        _aiManagerService.PauseAI(true);
        LeanTween.pauseAll();
    }

    public void OnResumeClicked()
    {
        _aiManagerService.PauseAI(false);
        LeanTween.resumeAll();
        ClosePopup();
    }

    public void OnLeaveMatchClicked()
    {
        LeanTween.resumeAll();
        _stateManager.TriggerEvent(Event.SHOW_MAIN_MENU);
    }
}