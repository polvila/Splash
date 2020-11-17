using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _numText;
	public Button Button;

	private int _missAnimationId;
	private int _moveAnimationId;

	private int _num;
	public int Num
	{
		get { return _num; }
		set
		{
			_num = value;
			_numText.text = _num.ToString();
		}
	}

	public int Index;

	public void TriggerMissAnimationFrom(Transform from, Action onComplete = null)
	{
		if (LeanTween.isTweening(_missAnimationId))
		{
			LeanTween.cancel(_missAnimationId);
		}
		transform.position = from.position;
		_missAnimationId = LeanTween.moveLocalY(gameObject, transform.localPosition.y + 30f, 1f)
			.setEasePunch()
			.setOnComplete(() => onComplete?.Invoke())
			.id;
	}

	public void MoveFrom(Transform from, Transform to, Action onComplete = null)
	{
		Button.enabled = false;
		transform.SetAsLastSibling();
		if (LeanTween.isTweening(_missAnimationId))
		{
			LeanTween.cancel(_missAnimationId);
		}
		transform.position = from.position;
		_moveAnimationId = LeanTween.move(gameObject, to, 0.2f).setOnComplete(() => onComplete?.Invoke()).id;
	}

	private void OnDestroy()
	{
		if (LeanTween.isTweening(_moveAnimationId))
		{
			LeanTween.cancel(_moveAnimationId, true);
		}
	}
}
