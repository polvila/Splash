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
        _gameManagerService.NewBoardReceived += OnNewBoardReceived;
        _gameManagerService.CardUpdate += OnCardUpdate;
    }
    
    private void OnNewBoardReceived(int[] numbers)
    {
        view.EnemyCounter = 0;
        view.HumanCounter = 0;
        view.Timer.Init(5);
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
}
