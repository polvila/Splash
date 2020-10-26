using TMPro;
using UnityEngine;

namespace Core.ScreenManagement
{
    public class LoadingSpinnerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _spinnerText;

        [SerializeField] private GameObject _loadingImage;
    
        private void Awake()
        {
            SetText(string.Empty);
        }
        
        void Start()
        {
            LeanTween.rotateAround(_loadingImage, Vector3.back, 360, 1f).setLoopClamp();
        }

        public void SetText(string text)
        {
            _spinnerText.text = text;
        }
    }
}