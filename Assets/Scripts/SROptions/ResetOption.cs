using UnityEngine.SceneManagement;

public partial class SROptions {

	[DisplayName("Reset Scene")]
	public void ResetScene() {
		SceneManager.LoadScene("Main");
	}
	
}
