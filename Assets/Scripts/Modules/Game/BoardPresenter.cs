public class BoardPresenter : Presenter<BoardView>
{
    private IGameManagerService _gameManagerService;
    
    public BoardPresenter(IGameManagerService gameManagerService)
    {
        _gameManagerService = gameManagerService;
    }
    
    public override void RegisterView(BoardView view)
    {
        base.RegisterView(view);
        _gameManagerService.NewGameReceived += OnNewGameReceived;
        _gameManagerService.CardUpdate += OnCardUpdate;
        _gameManagerService.GameFinished += OnGameFinished;
        view.CardSelected += cardPosition => _gameManagerService.PlayThisCard(cardPosition);
        _gameManagerService.Initialize();
    }

    private void OnNewGameReceived(int[] numbers, int seconds)
    {
        for (int i = 0; i < numbers.Length; ++i)
        {
            view.AddNewCardTo(i, numbers[i]);
        }
        view.SetInfo("");
        _gameManagerService.Start(Mode.IA);
    }
    
    private void OnCardUpdate(int fromCardPosition, int toCardPosition, int? newNumber)
    {
        if (fromCardPosition == toCardPosition)
        {
            //TODO: Card not playable
        }
        else
        {
            view.MoveCard(fromCardPosition, toCardPosition);
            if (newNumber != null)
            {
                view.AddNewCardTo(fromCardPosition, newNumber.Value);
            }
        }
    }

    private void OnGameFinished(GameResult result)
    {
        view.StopPlayableCards();
    }

    public override void Dispose()
    {
        base.Dispose();
        _gameManagerService.NewGameReceived -= OnNewGameReceived;
    }
}