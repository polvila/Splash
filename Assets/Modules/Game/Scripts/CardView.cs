using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _numText;

	private int _missAnimationId;

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

	public void TriggerMissAnimationFrom(Vector2 position)
	{
		if (LeanTween.isTweening(_missAnimationId))
		{
			LeanTween.cancel(_missAnimationId);
		}
		transform.position = position;
		_missAnimationId = LeanTween.moveLocalY(gameObject, transform.localPosition.y + 30f, 1f).setEasePunch().id;
	}
}
