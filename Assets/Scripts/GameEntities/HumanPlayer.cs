using System;
using UnityEngine;
using UnityEngine.UI;

public class HumanPlayer : Player
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
                    Board.Cards[boardCard.Index] = card;
                    Cards[cardIndex] = GetNewCard(cardIndex);
                    SetupAsPlayable(Cards[cardIndex]);
                    
                    MoveCard(card, Board.Slots[boardCard.Index].position, 
                        () => Destroy(boardCard.gameObject));
                }
            });
    }

    void MoveCard(CardView card, Vector2 destination, Action onComplete)
    {
        card.transform.SetAsLastSibling();
        LeanTween.move(card.gameObject,
            destination, 0.2f).setOnComplete(() => onComplete?.Invoke());
    }
}
