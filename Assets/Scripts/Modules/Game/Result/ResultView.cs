using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class ResultView : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _resultText;

	private Dictionary<GameResult, string> resultTexts = new Dictionary<GameResult, string>()
	{
		{ GameResult.HumanWins, "You Win!"},	
		{ GameResult.EnemyWins, "You Lose!"},
		{ GameResult.Draw, "DRAW!"}
	};
	
	[Inject]
	void Init()
	{
		//gameStateModel.Result.PropertyChanged += result => SetResultText(resultTexts[result]);
	}

	private void SetResultText(string result)
	{
		_resultText.text = result;
		gameObject.SetActive(true);
	}
}
