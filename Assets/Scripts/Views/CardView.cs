using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI NumText;

	private static float _singleNumFontSize = 0.0f;
	
	private int num;

	public int Num
	{
		get { return num; }
		set
		{
			num = value;
			NumText.text = num.ToString();
		}
	}

	public int Index;
}
