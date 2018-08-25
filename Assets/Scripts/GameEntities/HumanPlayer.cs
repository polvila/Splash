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
                if (card.CanIPlayAbove(out boardCardDestination) || SROptions.Current.GodMode)
                {
                    int cardIndex = card.Index;
                    GetNewCard(cardIndex);
                    Object.Destroy(card.GetComponent<Button>());
                    if (SROptions.Current.GodMode)
                    {
                        int i = Random.Range(0, GameStateModel.Board.Cards.Length);
                        boardCardDestination = GameStateModel.Board.Cards[i];
                    }
                    card.PlayAboveTo(boardCardDestination);
                    GameStateModel.HumanCounter.Property++;
                }
            });
    }
}
