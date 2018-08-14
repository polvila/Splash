using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI NumText;

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
	
}
