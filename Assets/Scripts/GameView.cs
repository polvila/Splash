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
	[SerializeField] private float _boardCardsPadding;

	[Space(10)] 
	[Header("Cards views")]
	[SerializeField] private PlayerView _playerView;
	[SerializeField] private EnemyView _enemyView;
	[SerializeField] private BoardView _boardView;
	
	private const int PlayerCards = 4;
	private const int BoardCards = 2;
	private const int BoardPosition = 0;

	private float _cardSpaceWidth;
	private Vector3 _maxRightPoint;
	private Vector3 _maxLeftPoint;
	private float _newScale = 1.0f;
	private float _cardWidth;
	
	void Awake()
	{
		_maxRightPoint = Camera.main.ViewportToWorldPoint(new Vector3(1.0f - _marginViewport, 0.5f, 0.0f));
		_maxLeftPoint = Camera.main.ViewportToWorldPoint(new Vector3(_marginViewport, 0.5f, 0.0f));
		
		var playerSpaceWidth = _maxRightPoint.x + Math.Abs(_maxLeftPoint.x);
		_cardSpaceWidth = playerSpaceWidth / (PlayerCards - 1);
		
		_cardWidth = _card.GetComponent<Renderer>().bounds.size.x;
		_newScale = GetNewScale();
		
		FillCards(_playerView, _playerPosition);
		FillCards(_enemyView, _enemyPosition);
		FillBoard();
	}
	
	private void FillCards(CardsView cardsView, float position)
	{
		for (int i = 0; i < PlayerCards; ++i)
		{
			GameObject instantiatedCard = Instantiate(_card, cardsView.transform);
			cardsView.Cards[i] = instantiatedCard.GetComponent<CardView>();
			instantiatedCard.transform.localScale = new Vector3(_newScale, _newScale);
			instantiatedCard.transform.position = new Vector2(_maxLeftPoint.x + _cardSpaceWidth * i,
				position);
		}
	}

	private void FillBoard()
	{
		for (int i = 0; i < BoardCards; ++i)
		{
			GameObject instantiatedCard = Instantiate(_card, _boardView.transform);
			_boardView.Cards[i] = instantiatedCard.GetComponent<CardView>();
			instantiatedCard.transform.localScale = new Vector3(_newScale, _newScale);
			var maxPointBoard = _cardWidth / 2 + _boardCardsPadding;
			instantiatedCard.transform.position = new Vector2(maxPointBoard * (i == 0? -1 : 1),
				BoardPosition);
		}
	}
	
	private float GetNewScale()
	{
		var cardWidthWithPadding = _cardWidth + _rightMinimumPadding;
		if (_cardSpaceWidth <= cardWidthWithPadding)
		{
			var newCardWidth = _cardWidth - (cardWidthWithPadding - _cardSpaceWidth);
			return newCardWidth / _cardWidth;
		}

		return 1.0f;
	}
}
