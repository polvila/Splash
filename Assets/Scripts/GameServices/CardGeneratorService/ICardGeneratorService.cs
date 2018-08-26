public interface ICardGeneratorService
{
    int GetMaxRange { get; }
    int GetMinRange { get; }
    CardView GenerateCard();
}
