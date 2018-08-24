using UnityEngine;
using UnityEngine.UI;

public class HumanPlayer : CardsZone
{
    protected override void GetNewCard(int index)
    {
        base.GetNewCard(index);
        SetupAsPlayable(Cards[index]); 
    }
    
    void SetupAsPlayable(CardView card)
    {
        card.gameObject.AddComponent<Button>().onClick.AddListener(
            () =>
            {
                CardView boardCardDestination;
                if (card.CanIPlayAbove(out boardCardDestination))
                {
                    int cardIndex = card.Index;
                    GetNewCard(cardIndex);
                    Object.Destroy(card.GetComponent<Button>());
                    card.PlayAboveTo(boardCardDestination);
                }
            });
    }
}
