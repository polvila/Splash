using UnityEngine;

public class CardsView : MonoBehaviour {

	private const int NumCards = 4;
	[HideInInspector] public CardView[] Cards = new CardView[NumCards];
}
