using Core.CloudOnce;
using Core.ScreenManagement;
using Core.StateManager;
using TMPro;
using UnityEngine;
using Zenject;
using Event = Core.StateManager.Event;

namespace Modules.Game
{
    public class ResultView : PopupScreenView
    {
        [SerializeField] private TMP_Text _resultText;
        [SerializeField] private GameObject _newRecordBanner;

        private IStateManager _stateManager;
        private ICloudOnceService _cloudOnceService;

        private int _result;

        [Inject]
        void Init(IStateManager stateManager,
            ICloudOnceService cloudOnceService)
        {
            _stateManager = stateManager;
            _cloudOnceService = cloudOnceService;
        }

        public override void SetParams(object paramsObject)
        {
            ResultParams resultParams = (ResultParams) paramsObject;

            _result = resultParams.Result;
            _resultText.text = $"{_result} points";
            _newRecordBanner.SetActive(resultParams.NewRecord);
            gameObject.SetActive(true);
            _cloudOnceService.SubmitScoreToLeaderboard(resultParams.Result);
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

        public void OnShareClicked()
        {
            new NativeShare().SetTitle("Share your new score!")
                .SetText($"Hey, I just scored {_result} points at Splash!\nGive it a try: <google-store-link>")
                .SetSubject("Look my new Splash! score")
                .Share();
        }
    }
}