using System;
using TMPro;
using UnityEngine;

public class CountdownView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    
    public void StartCountdown(Action onComplete)
    {
        var seq = LeanTween.sequence();
        seq.append(() => { _text.text = "3"; });
        seq.append(1f);
        seq.append(() => { _text.text = "2"; });
        seq.append(1f);
        seq.append(() => { _text.text = "1"; });
        seq.append(1f);
        seq.append(() =>
        {
            _text.text = "";
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }
}
