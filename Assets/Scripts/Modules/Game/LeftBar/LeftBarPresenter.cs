public class LeftBarPresenter : Presenter<LeftBarView>
{
    private const int FirstPositionHumanCards = 6;
    private IGameManagerService _gameManagerService;
    
    public LeftBarPresenter(IGameManagerService gameManagerService)
    {
        _gameManagerService = gameManagerService;
    }
    
    public override void RegisterView(LeftBarView view)
    {
        base.RegisterView(view);
        _gameManagerService.NewGameReceived += OnNewGameReceived;
        _gameManagerService.CardUpdate += OnCardUpdate;
        _gameManagerService.Splashed += OnSplashed;
    }
    
    private void OnNewGameReceived(int[] numbers, int seconds)
    {
        view.EnemyCounter = 0;
        view.HumanCounter = 0;
        view.Timer.Init(seconds);
        view.Timer.Start();
    }

    private void OnCardUpdate(int fromCardPosition, int toCardPosition, int? newNumber)
    {
        if (fromCardPosition == toCardPosition) return;
        
        if (fromCardPosition >= FirstPositionHumanCards)
        {
            view.HumanCounter++;
        }
        else
        {
            view.EnemyCounter++;
        }
    }

    private void OnSplashed(bool wasHuman)
    {
        if (wasHuman)
        {
            view.HumanCounter += 10;
        }
        else
        {
            view.EnemyCounter += 10;
        }
    }
    
    public override void Dispose()
    {
        _gameManagerService.NewGameReceived -= OnNewGameReceived;
        _gameManagerService.CardUpdate -= OnCardUpdate;
        _gameManagerService.Splashed -= OnSplashed;
    }
}
