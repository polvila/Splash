using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class ResultView : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _resultText;

	private IGameManagerService _gameManagerService;

	private Dictionary<GameResult, string> resultTexts = new Dictionary<GameResult, string>()
	{
		{ GameResult.HumanWins, "You Win!"},	
		{ GameResult.EnemyWins, "You Lose!"},
		{ GameResult.Draw, "DRAW!"}
	};
	
	[Inject]
	void Init(IGameManagerService gameManagerService)
	{
		_gameManagerService = gameManagerService;
		_gameManagerService.GameFinished += SetResultText;
	}

	private void SetResultText(GameResult result)
	{
		_resultText.text = resultTexts[result];
		gameObject.SetActive(true);
	}

	private void OnDestroy()
	{
		_gameManagerService.GameFinished -= SetResultText;
	}
}
