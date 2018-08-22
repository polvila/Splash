using UnityEngine.UI;

public class HumanPlayer : Player
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
                    card.PlayAboveTo(boardCardDestination);
                }
            });
    }
}
