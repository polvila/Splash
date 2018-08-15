using UnityEngine;

public class PlayersView : CardsView
{
    [SerializeField] protected BoardView BoardView;

    protected bool PlayThisCard(CardView card, out CardView boardCard)
    {
        for (int i = 0; i < BoardView.Cards.Length; ++i)
        {
            if (IsACompatibleMove(card.Num, BoardView.Cards[i].Num))
            {
                boardCard = BoardView.Cards[i];
                BoardView.Cards[i] = card;
                return true;
            }
        }
        boardCard = card;
        return false;
    }

    private bool IsACompatibleMove(int originNum, int destinationNum)
    {
        return originNum == destinationNum + 1 
        || originNum == destinationNum - 1
        || originNum == _cardGeneratorService.GetMaxRange 
            && destinationNum == _cardGeneratorService.GetMinRange
        || originNum == _cardGeneratorService.GetMinRange 
            && destinationNum == _cardGeneratorService.GetMaxRange;
    }
}
