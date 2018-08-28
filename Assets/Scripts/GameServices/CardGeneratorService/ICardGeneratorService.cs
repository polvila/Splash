
public interface ICardGeneratorService
{
    int GetMaxRange { get; }
    int GetMinRange { get; }
    CardGeneratorMode GeneratorMode { get; set; }
    CardView GenerateCard();
}

public enum CardGeneratorMode
{
    Random,
    RandomExcluding,
}