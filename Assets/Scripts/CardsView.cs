using UnityEngine;

public class CardsView : MonoBehaviour {

	[HideInInspector] public CardView[] Cards;
	[HideInInspector] public Transform[] Slots;

	void Awake()
	{
		Cards = new CardView[transform.childCount];
		Slots = new Transform[transform.childCount];
		
		for (int i = 0; i < Slots.Length; ++i)
		{
			Slots[i] = transform.GetChild(i);
		}
	}
}
