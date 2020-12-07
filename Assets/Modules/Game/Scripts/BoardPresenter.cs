using Core.ScreenManagement;

namespace Modules.Game
{
    public class BoardPresenter : Presenter<IBoardView>
    {
        private IGameManagerService _gameManagerService;
        private IScreenManager _screenManager;

        public BoardPresenter(
            IGameManagerService gameManagerService,
            IScreenManager screenManager)
        {
            _gameManagerService = gameManagerService;
            _screenManager = screenManager;
        }

        public override void RegisterView(IBoardView view)
        {
            base.RegisterView(view);
            _gameManagerService.NewGameReceived += OnNewGameReceived;
            _gameManagerService.CardUpdate += OnCardUpdate;
            _gameManagerService.GameFinished += OnGameFinished;
            _gameManagerService.Splashed += OnSplashed;
            _gameManagerService.Unblocked += OnUnblocked;
            view.CardSelected += _gameManagerService.PlayThisCard;
            view.SplashZoneSelected += _gameManagerService.HumanSplash;
            view.StartGameEvent += _gameManagerService.StartGame;
            _screenManager.ShowSpinner();
            _gameManagerService.Initialize(view.GameMode);
        }

        private void OnNewGameReceived(int[] numbers)
        {
            for (int i = 0; i < BoardView.LeftStackPosition; ++i)
            {
                view.AddNewCardTo(i, numbers[i]);
            }

            for (int i = BoardView.RightStackPosition + 1; i < numbers.Length; ++i)
            {
                view.AddNewCardTo(i, numbers[i]);
            }

            _screenManager.HideSpinner();

            view.StartCountdown(() =>
            {
                view.UnblockMiddleCards(
                    numbers[BoardView.LeftStackPosition], 
                    numbers[BoardView.RightStackPosition]);
            });
        }
        
        private void OnCardUpdate(int fromCardPosition, int toCardPosition, int? newNumber)
        {
            if (fromCardPosition == toCardPosition || newNumber == null)
            {
                view.MissCardMove(fromCardPosition);
            }
            else
            {
                view.MoveCard(fromCardPosition, toCardPosition);
                view.AddNewCardTo(fromCardPosition, newNumber.Value);
            }
        }

        private void OnGameFinished(int result, bool newRecord)
        {
            view.FinishGame(() =>
                _screenManager.ShowPopup("ResultPopup", new ResultParams(result, newRecord)));
        }

        private void OnSplashed(bool wasHuman, int newLeftNumber, int newRightNumber, int points)
        {
            view.ShowSplash(wasHuman, points);
            view.DestroyCard(BoardView.LeftStackPosition);
            view.DestroyCard(BoardView.RightStackPosition);
        }

        private void OnUnblocked(int newLeftNumber, int newRightNumber)
        {
            view.UnblockMiddleCards(newLeftNumber, newRightNumber);
        }

        public override void Dispose()
        {
            base.Dispose();
            _gameManagerService.NewGameReceived -= OnNewGameReceived;
            _gameManagerService.CardUpdate -= OnCardUpdate;
            _gameManagerService.GameFinished -= OnGameFinished;
            _gameManagerService.Splashed -= OnSplashed;
            _gameManagerService.Unblocked -= OnUnblocked;

            _gameManagerService.Exit();

            view.CardSelected -= _gameManagerService.PlayThisCard;
            view.SplashZoneSelected -= _gameManagerService.HumanSplash;
            view.StartGameEvent -= _gameManagerService.StartGame;
        }
    }
}