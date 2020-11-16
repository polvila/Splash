using Core.StateManager;

public partial class SROptions {

	[DisplayName("God Mode")]
	public bool GodMode
	{
		get;
		set;
	}
	
	[DisplayName("Main Menu")]
	public void MainMenu() 
	{
		GetContainer().Resolve<IStateManager>().TriggerEvent(Event.SHOW_MAIN_MENU);
	}
	
	[DisplayName("End Game")]
	public void EndGame() 
	{
		//GetContainer().Resolve<IGameStateModel>().State.Property = GameState.Finished;
	}

//	[DisplayName("Card Generator Mode")]
//	public CardGeneratorMode CardGeneratorMode 
//	{
//		get { return GetContainer().Resolve<INumberGeneratorService>().GeneratorMode; }
//		set
//		{
//			GetContainer().Resolve<INumberGeneratorService>().GeneratorMode = value;
//		}
//	}
}
