using UnityEngine;
using Random = UnityEngine.Random;

public class GameView : MonoBehaviour
{
	[SerializeField] private GameObject _card;

	[Space(10)] 
	[Header("Cards views")]
	[SerializeField] private PlayerView _playerView;
	[SerializeField] private EnemyView _enemyView;
	[SerializeField] private BoardView _boardView;
	
	private const int PlayerCards = 4;
	private const int BoardCards = 2;
	private const int MaxRange = 12;
	private const int MinRange = 1;

	void Start()
	{
		//FillCards(_playerView);
		FillCards(_enemyView);
		//FillBoard();
	}
	
	private void FillCards(CardsView cardsView)
	{
		var newSize = cardsView.Slots[0].GetComponent<RectTransform>().sizeDelta;
		
		for (int i = 0; i < PlayerCards; ++i)
		{
			GameObject instantiatedCard = Instantiate(_card, this.transform);
			cardsView.Cards[i] = instantiatedCard.GetComponent<CardView>();
			cardsView.Cards[i].Num = Random.Range(MinRange, MaxRange);
			cardsView.Cards[i].transform.position = cardsView.Slots[i].position;
			cardsView.Cards[i].GetComponent<RectTransform>().sizeDelta = newSize;
		}
	}

	private void FillBoard()
	{
		for (int i = 0; i < BoardCards; ++i)
		{
			GameObject instantiatedCard = Instantiate(_card, _boardView.transform);
			_boardView.Cards[i] = instantiatedCard.GetComponent<CardView>();
			
		}
	}
}
