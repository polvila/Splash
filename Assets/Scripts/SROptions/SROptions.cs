using Core.StateManager;

public partial class SROptions {

	[DisplayName("God Mode")]
	public bool GodMode
	{
		get;
		set;
	}

	[DisplayName("End Game")]
	public void EndGame() 
	{
		//GetContainer().Resolve<IGameStateModel>().State.Property = GameState.Finished;
	}
}
