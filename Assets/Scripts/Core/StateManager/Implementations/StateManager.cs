using Core.Authentication;
using Core.ScreenManagement;
using UnityEngine;
using Zenject;

namespace Core.StateManager
{
    public class StateManager : MonoBehaviour, IStateManager, IInitializable
    {
        [SerializeField] private PlayMakerFSM _mainFsm;
        [SerializeField] private GameObject _titleCanvas;

        private IAuthenticationService _authenticationService;
        private IScreenManager _screenManager;

        [Inject]
        private void Init(
            IAuthenticationService authenticationService,
            IScreenManager screenManager)
        {
            _authenticationService = authenticationService;
            _screenManager = screenManager;
        }

        public void Initialize()
        {
            TriggerEvent(Event.START_BOOTSTRAP);
        }

        public void OnBoostrap()
        {
            _authenticationService.InitService(() =>
            {
                _authenticationService.DeviceAuthentication(() =>
                {
                    Destroy(_titleCanvas);
                    TriggerEvent(Event.SHOW_MAIN_MENU);
                });
            });
        }

        public void OnMainMenu()
        {
            _screenManager.ShowScreen("MainMenuScreen");
        }

        public void OnGame()
        {
            _screenManager.ShowScreen("GameScreen");
        }

        public void TriggerEvent(string eventKey)
        {
            _mainFsm.SendEvent(eventKey);
        }
    }
}