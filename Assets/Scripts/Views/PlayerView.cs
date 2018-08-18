using UnityEngine;
using UnityEngine.UI;

public class PlayerView : PlayersView
{
    protected override void FillSlotsWithCards()
    {
        base.FillSlotsWithCards();

        foreach (var card in Cards)
        {
            CardView boardCard;
            card.gameObject.AddComponent<Button>().onClick.AddListener(
                () =>
                {
                    if (PlayThisCard(card, out boardCard))
                    {
                        card.transform.SetAsLastSibling();
                        LeanTween.move(card.gameObject,
                            boardCard.transform.position, 0.2f);
                    }
                });
        }
    }
}
