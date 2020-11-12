using System;
using TMPro;
using UnityEngine;

public class SplashView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _pointsText;
    
    public void Show(int totalPoints)
    {
        _pointsText.text = totalPoints.ToString();
        _canvasGroup.alpha = 1;
        LeanTween.alphaCanvas(_canvasGroup, 0f, 2f);
    }
}
