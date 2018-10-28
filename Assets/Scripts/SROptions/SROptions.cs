using UnityEngine.SceneManagement;

public partial class SROptions {

	[DisplayName("God Mode")]
	public bool GodMode
	{
		get;
		set;
	}
	
	[DisplayName("Reset Scene")]
	public void ResetScene() 
	{
		SceneManager.LoadScene("Main");
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
