using UnityEngine.SceneManagement;

public partial class SROptions {

	[DisplayName("God Mode")]
	public bool GodMode
	{
		get;
		set;
	}
	
	[DisplayName("Reset Scene")]
	public void ResetScene() {
		SceneManager.LoadScene("Main");
	}
}
