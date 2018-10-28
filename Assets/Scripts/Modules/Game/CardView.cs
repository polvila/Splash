using System;
using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _numText;

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
}
