public class BoardPresenter : Presenter<BoardView>
{
    private const int LeftPilePosition = 4;
    private const int RightPilePosition = 5;

    private IGameManagerService _gameManagerService;
    private IScreenManager _screenManager;

    public BoardPresenter(
        IGameManagerService gameManagerService,
        IScreenManager screenManager)
    {
        _gameManagerService = gameManagerService;
        _screenManager = screenManager;
    }

    public override void RegisterView(BoardView view)
    {
        base.RegisterView(view);
        _gameManagerService.NewGameReceived += OnNewGameReceived;
        _gameManagerService.CardUpdate += OnCardUpdate;
        _gameManagerService.GameFinished += OnGameFinished;
        _gameManagerService.Splashed += OnSplashed;
        _gameManagerService.Unblocked += OnUnblocked;
        view.CardSelected += _gameManagerService.PlayThisCard;
        view.SplashZoneSelected += _gameManagerService.HumanSplash;
        _screenManager.ShowSpinner();
        _gameManagerService.Initialize();
    }

    private void OnNewGameReceived(int[] numbers, int seconds)
    {
        for (int i = 0; i < LeftPilePosition; ++i)
        {
            view.AddNewCardTo(i, numbers[i]);
        }
        
        for (int i = RightPilePosition + 1; i < numbers.Length; ++i)
        {
            view.AddNewCardTo(i, numbers[i]);
        }
        
        _screenManager.HideSpinner();
        
        view.StartCountdown(() =>
        {
            view.AddNewCardTo(LeftPilePosition, numbers[LeftPilePosition]);
            view.AddNewCardTo(RightPilePosition, numbers[RightPilePosition]);
            _gameManagerService.StartGame(Mode.IA);
        });
    }

    private void OnCardUpdate(int fromCardPosition, int toCardPosition, int? newNumber)
    {
        if (fromCardPosition == toCardPosition)
        {
            //TODO: Card not playable
            return;
        }

        view.MoveCard(fromCardPosition, toCardPosition);
        if (newNumber != null)
        {
            view.AddNewCardTo(fromCardPosition, newNumber.Value);
        }
    }

    private void OnGameFinished(GameResult result)
    {
        view.StopPlayableCards();
        view.SetInfo("");
    }

    private void OnSplashed(bool wasHuman, int newLeftNumber, int newRightNumber)
    {
        view.SetInfo(wasHuman ? "Splash!" : "IA Splash!");
        UpdatePiles(newLeftNumber, newRightNumber);
    }

    private void OnUnblocked(int newLeftNumber, int newRightNumber)
    {
        view.SetInfo("Unblocked");
        UpdatePiles(newLeftNumber, newRightNumber);
    }

    private void UpdatePiles(int newLeftNumber, int newRightNumber)
    {
        view.DestroyCard(LeftPilePosition, 0.3f);
        view.DestroyCard(RightPilePosition, 0.3f);
        view.AddNewCardTo(LeftPilePosition, newLeftNumber);
        view.AddNewCardTo(RightPilePosition, newRightNumber);
    }

    public override void Dispose()
    {
        base.Dispose();
        _gameManagerService.NewGameReceived -= OnNewGameReceived;
        _gameManagerService.CardUpdate -= OnCardUpdate;
        _gameManagerService.GameFinished -= OnGameFinished;
        _gameManagerService.Splashed -= OnSplashed;
        _gameManagerService.Unblocked += OnUnblocked;
        
        _gameManagerService.Exit();

        view.CardSelected -= _gameManagerService.PlayThisCard;
        view.SplashZoneSelected -= _gameManagerService.HumanSplash;
    }
}