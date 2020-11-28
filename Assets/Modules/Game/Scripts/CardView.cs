using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Game
{
	public class CardView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _numText;
		[SerializeField] private RectTransform _additiveRect;
		public Button Button;

		private int _missAnimationId;
		private int _moveAnimationId;
		private int _num;
		private bool _shine;
		private Color _resetCardShineColor;
		private Image _additiveImage;

		public int Num
		{
			get { return _num; }
			set
			{
				_num = value;
				_numText.text = _num.ToString();
			}
		}

		public bool Shine
		{
			get { return _shine; }
			set
			{
				_shine = value;
				if (_shine)
				{
					LeanTween.alpha(_additiveRect, 0.5f, 0.35f).setLoopPingPong();
				}
				else
				{
					LeanTween.cancel(_additiveRect);
					_additiveImage.color = _resetCardShineColor;
				}
			}
		}

		private void Awake()
		{
			_additiveImage = _additiveRect.GetComponent<Image>();
			_resetCardShineColor = _additiveImage.color;
		}

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
}
