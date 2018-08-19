using UnityEngine;

public class Player : CardsZone
{
    [SerializeField] protected Board Board;
    
    protected bool PlayThisCard(CardView card, out CardView boardCard)
    {
        for (int i = 0; i < Board.Cards.Length; ++i)
        {
            if (IsACompatibleMove(card.Num, Board.Cards[i].Num))
            {
                boardCard = Board.Cards[i];
                Board.Cards[i] = card;
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
