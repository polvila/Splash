using UnityEngine;
using Random = UnityEngine.Random;

public class GameView : MonoBehaviour
{
	[SerializeField] private GameObject _card;

	[Space(10)] 
	[Header("Cards views")]
	[SerializeField] private EnemyView _enemyView;
	[SerializeField] private BoardView _boardView;
	[SerializeField] private PlayerView _playerView;
	
	private const int MaxRange = 12;
	private const int MinRange = 1;

	void Start()
	{
		FillCards(_enemyView);
		FillCards(_boardView);
		FillCards(_playerView);
	}
	
	private void FillCards(CardsView cardsView)
	{
		for (int i = 0; i < cardsView.Cards.Length; ++i)
		{
			GameObject instantiatedCard = Instantiate(_card, this.transform);
			cardsView.Cards[i] = instantiatedCard.GetComponent<CardView>();
			cardsView.Cards[i].Num = Random.Range(MinRange, MaxRange);
			cardsView.Cards[i].transform.position = cardsView.Slots[i].position;
		}
	}
}
