using System;
using TMPro;
using UnityEngine;
using Zenject;

public class PointsView : MonoBehaviour
{
    private const string PointsFormat = "000";
    
    [SerializeField] private TMP_Text _pointsText;
    [SerializeField] private TMP_Text _maxRecordText;

    private IGameManagerService _gameManagerService;

    private int _totalPoints;

    [Inject]
    private void Init(IGameManagerService gameManagerService)
    {
        _gameManagerService = gameManagerService;

        _gameManagerService.Splashed += OnSplashed;
        _gameManagerService.CardUpdate += OnCardUpdate;
        _pointsText.text = _totalPoints.ToString(PointsFormat);
        _maxRecordText.text = _gameManagerService.HumanRecord.ToString(PointsFormat);
    }
    
    private void OnSplashed(bool wasHuman, int newLeftNumber, int newRightNumber, int points)
    {
        if (wasHuman)
        {
            AddPoints(points);
        }
    }
    
    private void OnCardUpdate(int fromCardPosition, int toCardPosition, int? newNumber)
    {
        if (fromCardPosition > toCardPosition)
        {
            AddPoints(1);
        }
    }

    private void AddPoints(int points)
    {
        _totalPoints += points;
        _pointsText.text = _totalPoints.ToString(PointsFormat);
    }

    private void OnDestroy()
    {
        _gameManagerService.Splashed -= OnSplashed;
        _gameManagerService.CardUpdate -= OnCardUpdate;
    }
}