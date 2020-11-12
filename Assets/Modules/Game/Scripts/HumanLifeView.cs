using TMPro;
using UnityEngine;
using Zenject;

public class HumanLifeView : MonoBehaviour
{
    private const int TotalPoints = 100;

    [SerializeField] private RectTransform _life;
    [SerializeField] private TMP_Text _totalPointsText;

    private IGameManagerService _gameManagerService;

    private int _totalPoints = TotalPoints;
    private float _pointsToLifeWidth;

    [Inject]
    private void Init(IGameManagerService gameManagerService)
    {
        _gameManagerService = gameManagerService;

        _gameManagerService.Splashed += OnSplashed;
        _gameManagerService.CardUpdate += OnCardUpdate;
        _totalPointsText.text = $"{_totalPoints}%";
        _pointsToLifeWidth = _life.sizeDelta.x / TotalPoints;
    }
    
    private void OnSplashed(bool wasHuman, int newLeftNumber, int newRightNumber, int points)
    {
        if (!wasHuman)
        {
            Damage(points);
        }
    }
    
    private void OnCardUpdate(int fromCardPosition, int toCardPosition, int? newNumber)
    {
        if (fromCardPosition < toCardPosition)
        {
            Damage(1);
        }
    }

    private void Damage(int points)
    {
        _totalPoints -= points;
        if (_totalPoints <= 0)
        {
            _totalPoints = 0;
        }
        _totalPointsText.text = $"{_totalPoints}%";
        
        var sizeDelta = _life.sizeDelta;
        var newWidth = _totalPoints * _pointsToLifeWidth;
        sizeDelta = new Vector2(newWidth, sizeDelta.y);
        _life.sizeDelta = sizeDelta;
    }
}
