using TMPro;
using UnityEngine;
using Zenject;

public class ResultView : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _resultText;
	[SerializeField] private GameObject _newRecordBanner;

	private IGameManagerService _gameManagerService;

	[Inject]
	void Init(IGameManagerService gameManagerService)
	{
		_gameManagerService = gameManagerService;
		_gameManagerService.GameFinished += SetResultText;
	}

	private void SetResultText(int result, bool newRecord)
	{
		_resultText.text = $"{result} points";
		_newRecordBanner.SetActive(newRecord);
		gameObject.SetActive(true);
	}

	private void OnDestroy()
	{
		_gameManagerService.GameFinished -= SetResultText;
	}
}
