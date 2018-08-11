using System;
using UnityEngine;

public class GameView : MonoBehaviour
{
	[SerializeField] private GameObject _card;
	
	[Space(10)] 
	[Header("Cards setup")]
	[SerializeField] private float _marginViewport;
	[SerializeField] private float _rightMinimumPadding;
	[SerializeField] private float _enemyPosition;
	[SerializeField] private float _playerPosition;

	[Space(10)] 
	[Header("Cards views")]
	[SerializeField] private PlayerView _playerView;
	[SerializeField] private EnemyView _enemyView;

	private const int PlayerCards = 4;

	private float cardSpaceWidth;
	private Vector3 maxRightPoint;
	private Vector3 maxLeftPoint;
	
	void Awake()
	{
		maxRightPoint = Camera.main.ViewportToWorldPoint(new Vector3(1.0f - _marginViewport, 0.5f, 0.0f));
		maxLeftPoint = Camera.main.ViewportToWorldPoint(new Vector3(_marginViewport, 0.5f, 0.0f));
		
		var playerSpaceWidth = maxRightPoint.x + Math.Abs(maxLeftPoint.x);
		cardSpaceWidth = playerSpaceWidth / (PlayerCards - 1);
		
		FillCards(_playerView, _playerPosition);
		FillCards(_enemyView, _enemyPosition);
	}
	
	private void FillCards(CardsView cardsView, float position)
	{
		cardsView.Cards = new CardView[PlayerCards];
		
		var newScale = GetNewScale();
		
		for (int i = 0; i < PlayerCards; ++i)
		{
			GameObject instantiatedCard = Instantiate(_card, cardsView.transform);
			instantiatedCard.transform.position = new Vector2(maxLeftPoint.x + cardSpaceWidth * i, position);
			
			instantiatedCard.transform.localScale = new Vector3(newScale, newScale);
			cardsView.Cards[i] = instantiatedCard.GetComponent<CardView>();
		}
	}
	
	private float GetNewScale()
	{
		var cardWidth = _card.GetComponent<Renderer>().bounds.size.x;
		var cardWidthWithPadding = cardWidth + _rightMinimumPadding;
		if (cardSpaceWidth <= cardWidthWithPadding)
		{
			var newCardWidth = cardWidth - (cardWidthWithPadding - cardSpaceWidth);
			return newCardWidth / cardWidth;
		}

		return 1.0f;
	}
}
