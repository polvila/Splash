using UnityEngine;

public class BoardView : MonoBehaviour {

	private const int NumCards = 2;
	[HideInInspector] public CardView[] Cards = new CardView[NumCards];
}
