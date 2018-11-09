using System;
using UniRx;

public class BoardPresenter : Presenter<BoardView>
{
    private const int LeftPilePosition = 4;
    private const int RightPilePosition = 5;

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
        _gameManagerService.Splashed += OnSplashed;
        view.CardSelected += _gameManagerService.PlayThisCard;
        view.SplashZoneSelected += _gameManagerService.HumanSplash;
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
    }

    private void OnSplashed(bool wasHuman, int newLeftNumber, int newRightNumber)
    {
        view.SetInfo(wasHuman ? "Splash!" : "IA Splash!");
        Observable
            .Timer(TimeSpan.FromSeconds(1))
            .Subscribe(x =>
            {
                view.SetInfo("");
            });

        view.DestroyCard(LeftPilePosition);
        view.DestroyCard(RightPilePosition);
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

        view.CardSelected -= _gameManagerService.PlayThisCard;
        view.SplashZoneSelected -= _gameManagerService.HumanSplash;
    }
}