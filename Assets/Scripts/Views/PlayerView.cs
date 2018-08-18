using UnityEngine;
using UnityEngine.UI;

public class PlayerView : PlayersView
{
    protected override void FillSlotsWithCards()
    {
        base.FillSlotsWithCards();

        foreach (var card in Cards)
        {
            SetupAsPlayable(card);
        }
    }
    
    void SetupAsPlayable(CardView card)
    {
        card.gameObject.AddComponent<Button>().onClick.AddListener(
            () =>
            {
                CardView boardCard;
                if (PlayThisCard(card, out boardCard))
                {
                    int cardIndex = card.Index;
                    card.Index = boardCard.Index;
                    BoardView.Cards[boardCard.Index] = card;
                    Cards[cardIndex] = GetNewCard(cardIndex);
                    SetupAsPlayable(Cards[cardIndex]);
                    
                    card.transform.SetAsLastSibling();
                    LeanTween.move(card.gameObject,
                        boardCard.transform.position, 0.2f).setOnComplete(
                        () => Destroy(boardCard.gameObject));
                }
            });
    }
}
