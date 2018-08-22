public interface ICardGeneratorService
{
    int GetMaxRange { get; }
    int GetMinRange { get; }
    CardView GetRandomCard();
    CardView GetRandomCardExcluding(CardView[] cards);
}
