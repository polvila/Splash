using TMPro;
using UnityEngine;
using Zenject;

public class LeftBarView : MonoBehaviour
{
	public TextMeshProUGUI EnemyCardsText;
	public TextMeshProUGUI TimerText;
	public TextMeshProUGUI HumanCardsText;
	
	[Inject]
	void Init(IGameStateModel gameStateModel)
	{
		gameStateModel.EnemyCounter.PropertyChanged += OnEnemyCardsUpdate;
		gameStateModel.HumanCounter.PropertyChanged += OnHumanCardsUpdate;
	}

	void OnEnemyCardsUpdate(int value)
	{
		EnemyCardsText.text = value.ToString();
	}
	
	void OnHumanCardsUpdate(int value)
	{
		HumanCardsText.text = value.ToString();
	}
}
