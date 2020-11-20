using System;
using TMPro;
using UnityEngine;

namespace Modules.Game
{
    public class SplashView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _pointsText;

        public void Show(int totalPoints, Action onComplete = null)
        {
            _pointsText.text = totalPoints.ToString();
            _canvasGroup.alpha = 1;
            LeanTween.alphaCanvas(_canvasGroup, 0f, 1.5f).setOnComplete(() => onComplete?.Invoke());
        }
    }
}