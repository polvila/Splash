namespace Modules.Game
{
    public interface INumberGeneratorService
    {
        CardGeneratorMode GeneratorMode { get; set; }
        int GetNumber();
    }

    public enum CardGeneratorMode
    {
        Random,
        RandomExcluding,
        FTUE,
    }
}