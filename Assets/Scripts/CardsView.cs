using UnityEngine;

public class CardsView : MonoBehaviour {

	private const int NumCards = 4;
	[HideInInspector] public CardView[] Cards = new CardView[NumCards];
	public Transform[] Slots;

	void Awake()
	{
		Slots = new Transform[transform.childCount];
		
		for (int i = 0; i < Slots.Length; ++i)
		{
			Slots[i] = transform.GetChild(i);
		}
	}
}
